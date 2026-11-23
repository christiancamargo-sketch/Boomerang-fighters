// ─────────────────────────────────────────────────────────────
// SNAPSHOTS: foto del estado que el host manda a los clientes.
// El cliente no simula reglas: copia la foto en su MatchSimulation
// local y la presentación la dibuja (con interpolación).
// v1: estado completo en ~0.6–1 KB. En online se puede pasar a
// "deltas" sin cambiar la simulación.
// ─────────────────────────────────────────────────────────────
using System.Collections.Generic;
using System.IO;

namespace BoomerangFighters.Core
{
    public sealed partial class MatchSimulation
    {
        public const byte SnapshotVersion = 1;

        public void WriteSnapshot(BinaryWriter w)
        {
            w.Write(SnapshotVersion);
            w.Write(Tick);
            w.Write((byte)Phase);
            w.Write((byte)Round);
            w.Write(PhaseTimer);
            w.Write((sbyte)RoundWinnerId);
            w.Write((sbyte)MatchWinnerId);
            w.Write(SlowMo);
            w.Write((byte)FreezeTicks);

            w.Write((byte)Arena.Hazards.Count);
            foreach (var hz in Arena.Hazards) { w.Write(hz.T); w.Write(hz.Z); w.Write((byte)hz.State); }

            w.Write((byte)Players.Count);
            foreach (var p in Players)
            {
                w.Write(p.Pos.x); w.Write(p.Pos.y);
                w.Write(p.Vel.x); w.Write(p.Vel.y);
                w.Write(p.Facing.Angle);
                w.Write(p.AimDir.Angle);
                byte flags = (byte)((p.Alive ? 1 : 0) | (p.HasCap ? 2 : 0) | (p.Charging ? 4 : 0) | (p.Aiming ? 8 : 0) | (p.Shield ? 16 : 0));
                w.Write(flags);
                w.Write(p.ChargeT);
                w.Write(p.DashT);
                w.Write(p.SpawnInvuln);
                w.Write((byte)p.Power);
                w.Write((byte)p.Score);
                w.Write((byte)p.Kos);
                w.Write((byte)p.RoundWins);
                w.Write(p.Cap.Id);
            }

            w.Write((byte)Caps.Count);
            foreach (var c in Caps)
            {
                w.Write(c.Id);
                w.Write((byte)c.OwnerId);
                w.Write(c.Ghost);
                w.Write((byte)c.Mode);
                w.Write(c.Pos.x); w.Write(c.Pos.y);
                w.Write(c.Y);
                w.Write(c.Radius);
                w.Write((byte)c.Power);
            }

            w.Write((byte)Pickups.Count);
            foreach (var pu in Pickups)
            {
                w.Write(pu.Id);
                w.Write((byte)pu.Type);
                w.Write(pu.Pos.x); w.Write(pu.Pos.y);
            }
        }

        public void ReadSnapshot(BinaryReader r)
        {
            byte ver = r.ReadByte();
            if (ver != SnapshotVersion) throw new InvalidDataException($"Snapshot v{ver} no soportado (esperaba v{SnapshotVersion}).");
            Tick = r.ReadInt32();
            Phase = (MatchPhase)r.ReadByte();
            Round = r.ReadByte();
            PhaseTimer = r.ReadSingle();
            RoundWinnerId = r.ReadSByte();
            MatchWinnerId = r.ReadSByte();
            SlowMo = r.ReadSingle();
            FreezeTicks = r.ReadByte();

            int hzCount = r.ReadByte();
            for (int i = 0; i < hzCount; i++)
            {
                float t = r.ReadSingle(), z = r.ReadSingle();
                var st = (LaserState)r.ReadByte();
                if (i < Arena.Hazards.Count) { Arena.Hazards[i].T = t; Arena.Hazards[i].Z = z; Arena.Hazards[i].State = st; }
            }

            int pc = r.ReadByte();
            var capIds = new int[pc];
            for (int i = 0; i < pc; i++)
            {
                var p = i < Players.Count ? Players[i] : null;
                var pos = new Vec2(r.ReadSingle(), r.ReadSingle());
                var vel = new Vec2(r.ReadSingle(), r.ReadSingle());
                var facing = Vec2.FromAngle(r.ReadSingle());
                var aim = Vec2.FromAngle(r.ReadSingle());
                byte flags = r.ReadByte();
                float chargeT = r.ReadSingle(), dashT = r.ReadSingle(), inv = r.ReadSingle();
                var power = (PowerUp)r.ReadByte();
                int score = r.ReadByte(), kos = r.ReadByte(), rw = r.ReadByte();
                capIds[i] = r.ReadInt32();
                if (p == null) continue;
                p.Pos = pos; p.Vel = vel; p.Facing = facing; p.AimDir = aim;
                p.Alive = (flags & 1) != 0; p.HasCap = (flags & 2) != 0; p.Charging = (flags & 4) != 0;
                p.Aiming = (flags & 8) != 0; p.Shield = (flags & 16) != 0;
                p.ChargeT = chargeT; p.DashT = dashT; p.SpawnInvuln = inv;
                p.Power = power; p.Score = score; p.Kos = kos; p.RoundWins = rw;
            }

            // gorras: se reutilizan por Id para que la vista no parpadee
            var byId = new Dictionary<int, CapState>();
            foreach (var c in Caps) byId[c.Id] = c;
            Caps.Clear();
            int cc = r.ReadByte();
            for (int i = 0; i < cc; i++)
            {
                int id = r.ReadInt32();
                if (!byId.TryGetValue(id, out var c)) c = new CapState { Id = id };
                c.OwnerId = r.ReadByte();
                c.Ghost = r.ReadBoolean();
                c.Mode = (CapMode)r.ReadByte();
                c.Pos = new Vec2(r.ReadSingle(), r.ReadSingle());
                c.Y = r.ReadSingle();
                c.Radius = r.ReadSingle();
                c.Power = (PowerUp)r.ReadByte();
                Caps.Add(c);
                byId[id] = c;
            }
            for (int i = 0; i < pc && i < Players.Count; i++)
                if (byId.TryGetValue(capIds[i], out var own)) Players[i].Cap = own;

            Pickups.Clear();
            int puc = r.ReadByte();
            for (int i = 0; i < puc; i++)
            {
                Pickups.Add(new Pickup { Id = r.ReadInt32(), Type = (PowerUp)r.ReadByte(), Pos = new Vec2(r.ReadSingle(), r.ReadSingle()) });
            }
        }

        public static void WriteEvent(BinaryWriter w, SimEvent e)
        {
            w.Write((byte)e.Type); w.Write(e.A); w.Write(e.B); w.Write(e.Extra);
            w.Write(e.Pos.x); w.Write(e.Pos.y); w.Write(e.Value);
        }

        public static SimEvent ReadEvent(BinaryReader r)
        {
            return new SimEvent
            {
                Type = (SimEventType)r.ReadByte(), A = r.ReadSByte(), B = r.ReadSByte(), Extra = r.ReadByte(),
                Pos = new Vec2(r.ReadSingle(), r.ReadSingle()), Value = r.ReadSingle(),
            };
        }
    }
}
