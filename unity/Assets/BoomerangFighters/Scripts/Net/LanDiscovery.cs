// ─────────────────────────────────────────────────────────────
// DESCUBRIMIENTO EN LAN: el host anuncia su sala por UDP broadcast
// cada segundo; los demás celulares en el mismo Wi‑Fi la ven en una lista.
//
// Importante (ver docs/investigacion/04-multijugador-local-y-online.md):
//  • iOS: broadcast/multicast requiere el entitlement
//    com.apple.developer.networking.multicast (se pide a Apple). Mientras
//    llega, usar Bonjour/mDNS (plugin nativo) o unirse por QR / IP.
//  • Android: adquirir un WifiManager.MulticastLock mientras se busca.
//  • Muchos Wi‑Fi públicos aíslan a los clientes: ofrecer hotspot + QR.
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BoomerangFighters.Net
{
    public sealed class RoomInfo
    {
        public string Name;
        public string HostIp;
        public int Port;
        public int Players;
        public int MaxPlayers;
        public double LastSeen;
        public override string ToString() => $"{Name} ({Players}/{MaxPlayers}) @ {HostIp}:{Port}";

        /// <summary>Texto para un código QR de "unirse": bf://IP:PUERTO/NOMBRE</summary>
        public string ToJoinUri() => $"bf://{HostIp}:{Port}/{Uri.EscapeDataString(Name)}";

        public static bool TryParseJoinUri(string uri, out string ip, out int port)
        {
            ip = null; port = Protocol.DefaultGamePort;
            if (string.IsNullOrEmpty(uri) || !uri.StartsWith("bf://", StringComparison.Ordinal)) return false;
            var rest = uri.Substring(5);
            int slash = rest.IndexOf('/');
            if (slash >= 0) rest = rest.Substring(0, slash);
            var parts = rest.Split(':');
            if (!IPAddress.TryParse(parts[0], out _)) return false;
            ip = parts[0];
            if (parts.Length > 1 && !int.TryParse(parts[1], out port)) return false;
            return true;
        }
    }

    /// <summary>Lo usa el host: anuncia "BFLAN|v|nombre|puerto|jugadores|max".</summary>
    public sealed class LanBeaconSender : IDisposable
    {
        readonly UdpClient _udp;
        readonly IPEndPoint _target;
        double _timer;

        public LanBeaconSender(int beaconPort = Protocol.DefaultBeaconPort, string targetAddress = "255.255.255.255")
        {
            _udp = new UdpClient(AddressFamily.InterNetwork) { EnableBroadcast = true };
            _target = new IPEndPoint(IPAddress.Parse(targetAddress), beaconPort);
        }

        public void Update(double dt, string roomName, int gamePort, int players, int maxPlayers)
        {
            _timer -= dt;
            if (_timer > 0) return;
            _timer = 1.0;
            var msg = Encoding.UTF8.GetBytes($"{Protocol.BeaconMagic}|{Protocol.Version}|{roomName.Replace("|", "/")}|{gamePort}|{players}|{maxPlayers}");
            try { _udp.Send(msg, msg.Length, _target); } catch (SocketException) { }
        }

        public void Dispose() => _udp.Dispose();
    }

    /// <summary>Lo usan los que buscan sala.</summary>
    public sealed class LanBeaconListener : IDisposable
    {
        readonly UdpClient _udp;
        readonly Dictionary<string, RoomInfo> _rooms = new Dictionary<string, RoomInfo>();
        double _clock;

        public LanBeaconListener(int beaconPort = Protocol.DefaultBeaconPort)
        {
            _udp = new UdpClient(AddressFamily.InterNetwork);
            _udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udp.Client.Bind(new IPEndPoint(IPAddress.Any, beaconPort));
        }

        /// <summary>Lee anuncios y devuelve las salas vistas en los últimos 3 segundos.</summary>
        public List<RoomInfo> Update(double dt)
        {
            _clock += dt;
            try
            {
                while (_udp.Available > 0)
                {
                    var ep = new IPEndPoint(IPAddress.Any, 0);
                    var data = _udp.Receive(ref ep);
                    var parts = Encoding.UTF8.GetString(data).Split('|');
                    if (parts.Length < 6 || parts[0] != Protocol.BeaconMagic) continue;
                    if (!byte.TryParse(parts[1], out byte ver) || ver != Protocol.Version) continue;
                    var key = ep.Address + ":" + parts[3];
                    if (!_rooms.TryGetValue(key, out var room)) _rooms[key] = room = new RoomInfo();
                    room.Name = parts[2];
                    room.HostIp = ep.Address.ToString();
                    int.TryParse(parts[3], out room.Port);
                    int.TryParse(parts[4], out room.Players);
                    int.TryParse(parts[5], out room.MaxPlayers);
                    room.LastSeen = _clock;
                }
            }
            catch (SocketException) { }
            var list = new List<RoomInfo>();
            foreach (var r in _rooms.Values) if (_clock - r.LastSeen < 3.0) list.Add(r);
            return list;
        }

        public void Dispose() => _udp.Dispose();
    }
}
