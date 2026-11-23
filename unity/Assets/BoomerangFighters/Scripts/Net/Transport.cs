// ─────────────────────────────────────────────────────────────
// TRANSPORTE: interfaz pequeña e intercambiable.
//   • UdpTransport  → LAN (mismo Wi‑Fi o hotspot), sin internet. Incluido.
//   • (Etapa 2) RelayTransport → Unity Transport + Unity Relay / Edgegap.
//   • (Etapa 3) el host corre como servidor dedicado headless.
// La simulación y las sesiones NO saben qué transporte usan.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace BoomerangFighters.Net
{
    public interface ITransport : IDisposable
    {
        /// <summary>Se llama cuando llega un datagrama: (peerId, datos).</summary>
        event Action<int, byte[]> Received;
        void Send(int peerId, byte[] data);
        void Poll();
        bool IsHost { get; }
        string Describe(int peerId);
    }

    /// <summary>
    /// UDP simple para LAN. El host escucha en un puerto; cada dirección IP:puerto
    /// que le escribe se vuelve un "peer". El cliente solo habla con el host (peer 0).
    /// Funciona en iOS/Android con sockets de .NET (Unity los soporta) — en iOS
    /// pide el permiso de Red local (NSLocalNetworkUsageDescription).
    /// </summary>
    public sealed class UdpTransport : ITransport
    {
        public event Action<int, byte[]> Received;
        public bool IsHost { get; }
        public int LocalPort => ((IPEndPoint)_udp.Client.LocalEndPoint).Port;

        readonly UdpClient _udp;
        readonly Dictionary<string, int> _peerByKey = new Dictionary<string, int>();
        readonly List<IPEndPoint> _peers = new List<IPEndPoint>();

        UdpTransport(UdpClient udp, bool isHost) { _udp = udp; IsHost = isHost; }

        public static UdpTransport Host(int port = Protocol.DefaultGamePort)
        {
            var udp = new UdpClient(AddressFamily.InterNetwork);
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udp.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            return new UdpTransport(udp, true);
        }

        public static UdpTransport Connect(string hostIp, int port = Protocol.DefaultGamePort)
        {
            var udp = new UdpClient(AddressFamily.InterNetwork);
            udp.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
            var t = new UdpTransport(udp, false);
            t.PeerFor(new IPEndPoint(IPAddress.Parse(hostIp), port)); // peer 0 = host
            return t;
        }

        int PeerFor(IPEndPoint ep)
        {
            string key = ep.ToString();
            if (_peerByKey.TryGetValue(key, out int id)) return id;
            id = _peers.Count;
            _peers.Add(ep);
            _peerByKey[key] = id;
            return id;
        }

        public void Send(int peerId, byte[] data)
        {
            if (peerId < 0 || peerId >= _peers.Count) return;
            try { _udp.Send(data, data.Length, _peers[peerId]); }
            catch (SocketException) { /* red caída: el timeout de la sesión lo detecta */ }
        }

        public void Poll()
        {
            try
            {
                while (_udp.Available > 0)
                {
                    var ep = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = _udp.Receive(ref ep);
                    int peer;
                    if (IsHost) peer = PeerFor(ep);
                    else if (!_peerByKey.TryGetValue(ep.ToString(), out peer)) continue; // ignora extraños
                    Received?.Invoke(peer, data);
                }
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }
        }

        public string Describe(int peerId) => peerId >= 0 && peerId < _peers.Count ? _peers[peerId].ToString() : "?";

        public void Dispose() => _udp.Dispose();
    }

    /// <summary>
    /// Transporte en memoria con latencia simulada: sirve para probar en el editor
    /// cómo se siente el juego con 50–150 ms de lag, sin red real.
    /// </summary>
    public sealed class LoopbackTransport : ITransport
    {
        public event Action<int, byte[]> Received;
        public bool IsHost { get; }
        public LoopbackTransport Other;
        public float LatencySeconds;
        public float LossRate;
        readonly int _peerIdOnOther;
        readonly List<(double at, int peer, byte[] data)> _inbox = new List<(double, int, byte[])>();
        double _clock;
        uint _rng = 2463534242u;

        public LoopbackTransport(bool isHost, int peerIdOnOther) { IsHost = isHost; _peerIdOnOther = peerIdOnOther; }

        public static (LoopbackTransport host, LoopbackTransport client) Pair(float latency = 0f, float loss = 0f)
        {
            var h = new LoopbackTransport(true, 0) { LatencySeconds = latency, LossRate = loss };
            var c = new LoopbackTransport(false, 0) { LatencySeconds = latency, LossRate = loss };
            h.Other = c; c.Other = h;
            return (h, c);
        }

        public void Advance(double seconds) => _clock += seconds;

        public void Send(int peerId, byte[] data)
        {
            if (Other == null) return;
            _rng ^= _rng << 13; _rng ^= _rng >> 17; _rng ^= _rng << 5;
            if (LossRate > 0f && (_rng % 10000) / 10000f < LossRate) return;
            Other._inbox.Add((Other._clock + LatencySeconds, _peerIdOnOther, (byte[])data.Clone()));
        }

        public void Poll()
        {
            for (int i = 0; i < _inbox.Count; i++)
            {
                if (_inbox[i].at > _clock) continue;
                var m = _inbox[i];
                _inbox.RemoveAt(i--);
                Received?.Invoke(m.peer, m.data);
            }
        }

        public string Describe(int peerId) => "loopback";
        public void Dispose() { }
    }
}
