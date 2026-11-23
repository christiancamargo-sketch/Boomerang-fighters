// ─────────────────────────────────────────────────────────────
// iOS (spec v0.2 §G): después de generar el proyecto de Xcode agrega al
// Info.plist lo que iOS 14+ exige para usar la red local:
//   • NSLocalNetworkUsageDescription: el texto del aviso de permiso.
//   • NSBonjourServices: el servicio que anunciamos/buscamos.
// Solo corre en builds de iOS (todo va dentro de #if UNITY_IOS, así el
// proyecto compila aunque no esté instalado el módulo de iOS).
// Nota: el descubrimiento por broadcast UDP en iOS además necesita el
// entitlement com.apple.developer.networking.multicast, que Apple da por
// solicitud. Mientras llega, en iOS se une por IP o por el texto bf://.
// ─────────────────────────────────────────────────────────────
#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace BoomerangFighters.EditorTools
{
    public sealed class IosLocalNetworkPostBuild : IPostprocessBuildWithReport
    {
        public const string UsageDescription = "GORRAZO usa tu red Wi‑Fi para jugar con amigos cerca";
        public const string BonjourService = "_gorrazo._udp";

        public int callbackOrder => 100;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS) return;
            string plistPath = Path.Combine(report.summary.outputPath, "Info.plist");
            if (!File.Exists(plistPath))
            {
                Debug.LogWarning($"[GORRAZO] No encontré {plistPath}; agrega a mano NSLocalNetworkUsageDescription y NSBonjourServices.");
                return;
            }

            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElementDict root = plist.root;

            root.SetString("NSLocalNetworkUsageDescription", UsageDescription);

            // conservar servicios que ya tenga (de otros plugins) y no duplicar el nuestro
            PlistElementArray services = null;
            if (root.values.TryGetValue("NSBonjourServices", out PlistElement existing)) services = existing as PlistElementArray;
            if (services == null) services = root.CreateArray("NSBonjourServices");
            bool present = false;
            foreach (PlistElement v in services.values)
                if (v is PlistElementString s && s.value == BonjourService) { present = true; break; }
            if (!present) services.AddString(BonjourService);

            plist.WriteToFile(plistPath);
            Debug.Log("[GORRAZO] Info.plist: agregado NSLocalNetworkUsageDescription y NSBonjourServices (_gorrazo._udp).");
        }
    }
}
#endif
