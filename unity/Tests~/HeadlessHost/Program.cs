// ─────────────────────────────────────────────────────────────
// HOST SIN GRÁFICOS
//   dotnet run -- balance 30 normal   → simula 30 partidas de bots por arena y da métricas
//   dotnet run -- balance 30 normal lataCaliente → lo mismo en otro modo (gorrazo, lataCaliente, laReta, retoDelDia)
//   dotnet run -- lan "Sala de prueba" → abre una sala LAN con bots (otros pueden unirse)
// Demuestra que la simulación corre igual sin Unity: es la base del
// servidor dedicado de la Etapa 3.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BoomerangFighters.Core;
using BoomerangFighters.Net;

static class Program
{
    static int Main(string[] args)
    {
        string mode = args.Length > 0 ? args[0] : "balance";
        if (mode == "lan") return Lan(args.Length > 1 ? args[1] : "Sala headless");
        int matches = args.Length > 1 && int.TryParse(args[1], out var m) ? m : 20;
        var diff = args.Length > 2 && Enum.TryParse<BotDifficulty>(args[2], true, out var d) ? d : BotDifficulty.Normal;
        var gm = args.Length > 3 && Enum.TryParse<GameMode>(args[3], true, out var g) ? g : GameMode.Gorrazo;
        return Balance(matches, diff, gm);
    }

    static int Balance(int matches, BotDifficulty diff, GameMode gameMode)
    {
        int nBots = gameMode == GameMode.LaReta ? 5 : 4;
        Console.WriteLine($"Balance: {matches} partidas de {nBots} bots ({diff}) por arena, modo {gameMode}, a 5 puntos.\n");
        var sw = Stopwatch.StartNew();
        long totalTicks = 0;
        foreach (var arena in ArenaLibrary.All)
        {
            var rounds = new List<float>();
            var causes = new Dictionary<KoCause, int>();
            var powers = new Dictionary<PowerUp, int>();
            int clanks = 0, deflects = 0, returns = 0, shoves = 0, shields = 0, ties = 0, tiesDoubleKo = 0, tiesTime = 0, sudden = 0, hotPasses = 0;
            var wins = new int[nBots];
            for (int g = 0; g < matches; g++)
            {
                var cfg = new MatchConfig { ArenaId = arena.Id, Seed = (uint)(1000 + g * 7919), PointsToWin = 5, Mode = gameMode };
                for (int i = 0; i < nBots; i++) cfg.Slots.Add(new PlayerSlot { Name = $"Bot{i}", IsBot = true, Difficulty = diff, Species = i });
                var sim = new MatchSimulation(cfg);
                var bots = Enumerable.Range(0, nBots).Select(i => new BotBrain(sim, i, diff)).ToArray();
                var inputs = new InputCommand[nBots];
                while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 20)
                {
                    for (int i = 0; i < nBots; i++) inputs[i] = bots[i].Think(MatchSimulation.Dt);
                    sim.Step(inputs);
                    foreach (var e in sim.Events)
                    {
                        switch (e.Type)
                        {
                            case SimEventType.KO: causes[(KoCause)e.Extra] = causes.GetValueOrDefault((KoCause)e.Extra) + 1; break;
                            case SimEventType.PickupTaken: powers[(PowerUp)e.Extra] = powers.GetValueOrDefault((PowerUp)e.Extra) + 1; break;
                            case SimEventType.Clank when e.Extra == 2: clanks++; break;
                            case SimEventType.Deflect: deflects++; if (e.Extra == 1) returns++; break;
                            case SimEventType.Shove: shoves++; break;
                            case SimEventType.ShieldBreak: shields++; break;
                            case SimEventType.SuddenDeathWarn: sudden++; break;
                            case SimEventType.HotCanPass when e.B >= 0: hotPasses++; break;
                            case SimEventType.RoundEnd when e.A < 0:
                                ties++;
                                if (e.Extra == (byte)RoundEndReason.DoubleKo) tiesDoubleKo++;
                                if (e.Extra == (byte)RoundEndReason.TimeUp) tiesTime++;
                                break;
                        }
                    }
                }
                totalTicks += sim.Tick;
                rounds.AddRange(sim.RoundDurations);
                if (sim.MatchWinnerId >= 0) wins[sim.MatchWinnerId]++;
            }
            rounds.Sort();
            float p50 = rounds[rounds.Count / 2], p90 = rounds[(int)(rounds.Count * 0.9)];
            Console.WriteLine($"── {arena.Name}");
            int totalKos = causes.Values.Sum();
            Console.WriteLine($"   rondas: {rounds.Count}  promedio {rounds.Average():0.0}s  mediana {p50:0.0}s  p90 {p90:0.0}s");
            Console.WriteLine($"   muerte súbita: {sudden} rondas ({100.0 * sudden / rounds.Count:0.0} %) · empates {ties} ({100.0 * ties / rounds.Count:0.0} %: doble KO {tiesDoubleKo}, tiempo {tiesTime})");
            Console.WriteLine($"   KOs por causa: {string.Join(", ", causes.OrderByDescending(k => k.Value).Select(k => $"{k.Key} {k.Value} ({100.0 * k.Value / Math.Max(1, totalKos):0}%)"))}");
            Console.WriteLine($"   power-ups tomados: {string.Join(", ", powers.OrderByDescending(k => k.Value).Select(k => $"{PowerUpInfo.Name(k.Key)} {k.Value}"))}");
            Console.WriteLine($"   clanks {clanks} · desvíos {deflects} (te la regreso {returns}) · empujones {shoves} · escudos rotos {shields}{(gameMode == GameMode.LataCaliente ? $" · pases de lata {hotPasses}" : "")} · victorias por asiento {string.Join("/", wins)}\n");
        }
        double simSeconds = totalTicks / (double)MatchSimulation.TickRate;
        Console.WriteLine($"Simulados {simSeconds / 60:0} min de juego en {sw.Elapsed.TotalSeconds:0.0} s reales (×{simSeconds / sw.Elapsed.TotalSeconds:0} tiempo real).");
        return 0;
    }

    static int Lan(string roomName)
    {
        using var transport = UdpTransport.Host(Protocol.DefaultGamePort);
        using var beacon = new LanBeaconSender();
        var host = new HostSession(transport, "Host");
        host.Log += Console.WriteLine;
        host.AddBot("Bot Tinta", BotDifficulty.Normal);
        host.AddBot("Bot Neón", BotDifficulty.Normal);
        Console.WriteLine($"Sala \"{roomName}\" abierta en el puerto {transport.LocalPort}. Esperando 15 s a otros jugadores...");
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < 15)
        {
            host.Poll(0.05f);
            beacon.Update(0.05, roomName, transport.LocalPort, host.Lobby.Count, host.MaxPlayers);
            Thread.Sleep(50);
        }
        host.StartMatch(host.BuildConfig("callejon", 3, (uint)Environment.TickCount));
        host.EventsThisTick += evs =>
        {
            foreach (var e in evs)
                if (e.Type == SimEventType.KO || e.Type == SimEventType.RoundEnd || e.Type == SimEventType.MatchEnd)
                    Console.WriteLine($"[tick {host.Sim.Tick}] {e.Type} {e.A} {e.B}");
        };
        // el jugador del host es un bot más en modo headless
        var hostBot = new BotBrain(host.Sim, 0, BotDifficulty.Normal);
        var clock = Stopwatch.StartNew();
        double next = 0;
        while (host.Sim.Phase != MatchPhase.MatchEnd)
        {
            host.Poll(MatchSimulation.Dt);
            beacon.Update(MatchSimulation.Dt, roomName, transport.LocalPort, host.Lobby.Count, host.MaxPlayers);
            host.Tick(hostBot.Think(MatchSimulation.Dt));
            next += MatchSimulation.Dt;
            int wait = (int)((next - clock.Elapsed.TotalSeconds) * 1000);
            if (wait > 0) Thread.Sleep(wait);
        }
        Console.WriteLine($"Ganó {host.Sim.Players[host.Sim.MatchWinnerId].Name}.");
        return 0;
    }
}
