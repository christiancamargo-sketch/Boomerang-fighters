// ─────────────────────────────────────────────────────────────
// HOST SIN GRÁFICOS
//   dotnet run -- balance 30 normal   → simula 30 partidas de bots por arena y da métricas
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
        return Balance(matches, diff);
    }

    static int Balance(int matches, BotDifficulty diff)
    {
        Console.WriteLine($"Balance: {matches} partidas de 4 bots ({diff}) por arena, a 5 puntos.\n");
        var sw = Stopwatch.StartNew();
        long totalTicks = 0;
        foreach (var arena in ArenaLibrary.All)
        {
            var rounds = new List<float>();
            var causes = new Dictionary<KoCause, int>();
            var powers = new Dictionary<PowerUp, int>();
            int clanks = 0, deflects = 0, shields = 0, ties = 0;
            var wins = new int[4];
            for (int g = 0; g < matches; g++)
            {
                var cfg = new MatchConfig { ArenaId = arena.Id, Seed = (uint)(1000 + g * 7919), PointsToWin = 5 };
                for (int i = 0; i < 4; i++) cfg.Slots.Add(new PlayerSlot { Name = $"Bot{i}", IsBot = true, Difficulty = diff, Species = i });
                var sim = new MatchSimulation(cfg);
                var bots = Enumerable.Range(0, 4).Select(i => new BotBrain(sim, i, diff)).ToArray();
                var inputs = new InputCommand[4];
                while (sim.Phase != MatchPhase.MatchEnd && sim.Tick < 60 * 60 * 20)
                {
                    for (int i = 0; i < 4; i++) inputs[i] = bots[i].Think(MatchSimulation.Dt);
                    sim.Step(inputs);
                    foreach (var e in sim.Events)
                    {
                        switch (e.Type)
                        {
                            case SimEventType.KO: causes[(KoCause)e.Extra] = causes.GetValueOrDefault((KoCause)e.Extra) + 1; break;
                            case SimEventType.PickupTaken: powers[(PowerUp)e.Extra] = powers.GetValueOrDefault((PowerUp)e.Extra) + 1; break;
                            case SimEventType.Clank when e.Extra == 2: clanks++; break;
                            case SimEventType.Deflect: deflects++; break;
                            case SimEventType.ShieldBreak: shields++; break;
                            case SimEventType.RoundEnd when e.A < 0: ties++; break;
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
            Console.WriteLine($"   rondas: {rounds.Count}  promedio {rounds.Average():0.0}s  mediana {p50:0.0}s  p90 {p90:0.0}s  empates {ties}");
            Console.WriteLine($"   KOs por causa: {string.Join(", ", causes.OrderByDescending(k => k.Value).Select(k => $"{k.Key} {k.Value}"))}");
            Console.WriteLine($"   power-ups tomados: {string.Join(", ", powers.OrderByDescending(k => k.Value).Select(k => $"{PowerUpInfo.Name(k.Key)} {k.Value}"))}");
            Console.WriteLine($"   clanks {clanks} · desvíos {deflects} · escudos rotos {shields} · victorias por asiento {string.Join("/", wins)}\n");
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
