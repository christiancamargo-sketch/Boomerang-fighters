// ─────────────────────────────────────────────────────────────
// PROTOCOLO de red (v1) — C# puro, sin UnityEngine.
// Mismo formato para LAN (UDP directo) y, más adelante, online
// (Relay / servidor dedicado): solo cambia el ITransport.
// ─────────────────────────────────────────────────────────────
using System;
using System.IO;
using BoomerangFighters.Core;

namespace BoomerangFighters.Net
{
    public static class Protocol
    {
        public const byte Version = 1;
        public const int DefaultGamePort = 7777;
        public const int DefaultBeaconPort = 47777;
        public const string BeaconMagic = "BFLAN";
        public const int SnapshotEveryTicks = 2;      // 30 snapshots/seg
        public const int InputRedundancy = 4;         // cada paquete repite las últimas 4 entradas
        public const int EventBatchRedundancy = 3;    // cada snapshot repite eventos de las últimas 3 fotos
        public const float PeerTimeoutSeconds = 5f;
    }

    public enum MsgType : byte
    {
        Hello = 1,       // cliente → host: versión + nombre
        Welcome = 2,     // host → cliente: tu índice en la sala
        Reject = 3,      // host → cliente: motivo (versión, sala llena)
        Lobby = 4,       // host → todos: lista de jugadores en la sala
        StartMatch = 5,  // host → todos: configuración + semilla
        Input = 6,       // cliente → host: entradas (con redundancia)
        Snapshot = 7,    // host → clientes: estado + eventos
        Bye = 8,
    }

    public static class Wire
    {
        public static void WriteConfig(BinaryWriter w, MatchConfig cfg)
        {
            w.Write(cfg.ArenaId ?? "callejon");
            w.Write((byte)cfg.PointsToWin);
            w.Write(cfg.Seed);
            w.Write((byte)cfg.Slots.Count);
            foreach (var s in cfg.Slots)
            {
                w.Write(s.Name ?? "");
                w.Write(s.IsBot);
                w.Write((byte)s.Difficulty);
                w.Write((byte)s.Species);
                w.Write(s.Shirt ?? "basica-blanca");
            }
        }

        public static MatchConfig ReadConfig(BinaryReader r)
        {
            var cfg = new MatchConfig
            {
                ArenaId = r.ReadString(),
                PointsToWin = r.ReadByte(),
                Seed = r.ReadUInt32(),
            };
            int n = r.ReadByte();
            for (int i = 0; i < n; i++)
            {
                cfg.Slots.Add(new PlayerSlot
                {
                    Name = r.ReadString(),
                    IsBot = r.ReadBoolean(),
                    Difficulty = (BotDifficulty)r.ReadByte(),
                    Species = r.ReadByte(),
                    Shirt = r.ReadString(),
                });
            }
            return cfg;
        }

        public static byte[] Build(MsgType type, Action<BinaryWriter> body)
        {
            using (var ms = new MemoryStream(256))
            using (var w = new BinaryWriter(ms))
            {
                w.Write((byte)type);
                body?.Invoke(w);
                w.Flush();
                return ms.ToArray();
            }
        }
    }
}
