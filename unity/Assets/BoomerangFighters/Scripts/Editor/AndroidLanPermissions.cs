// ─────────────────────────────────────────────────────────────
// ANDROID (spec v0.2 §G): al generar el proyecto Gradle agrega al
// AndroidManifest.xml del módulo unityLibrary los permisos que necesita
// el multijugador por Wi‑Fi local. Así no hace falta un
// Assets/Plugins/Android/AndroidManifest.xml propio (que choca con los
// de otros plugins al fusionar). El merge de Gradle los pasa a la app.
//   INTERNET, ACCESS_NETWORK_STATE, ACCESS_WIFI_STATE,
//   CHANGE_WIFI_MULTICAST_STATE (para el MulticastLock de LanPlatform)
// Android 17 (API 37) bloquea la red local por defecto: cuando el juego
// apunte a esa API habrá que pedir en tiempo de ejecución el permiso
// ACCESS_LOCAL_NETWORK (ver README). Aquí no se agrega todavía.
// Solo corre en builds de Android (#if UNITY_ANDROID).
// ─────────────────────────────────────────────────────────────
#if UNITY_ANDROID
using System.IO;
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

namespace BoomerangFighters.EditorTools
{
    public sealed class AndroidLanPermissions : IPostGenerateGradleAndroidProject
    {
        const string AndroidNs = "http://schemas.android.com/apk/res/android";

        public static readonly string[] Permissions =
        {
            "android.permission.INTERNET",
            "android.permission.ACCESS_NETWORK_STATE",
            "android.permission.ACCESS_WIFI_STATE",
            "android.permission.CHANGE_WIFI_MULTICAST_STATE",
        };

        public int callbackOrder => 100;

        // "path" es la carpeta del módulo unityLibrary del proyecto Gradle generado.
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifestPath = Path.Combine(path, "src", "main", "AndroidManifest.xml");
            if (!File.Exists(manifestPath))
            {
                Debug.LogWarning($"[GORRAZO] No encontré {manifestPath}; agrega a mano los permisos de red local (README).");
                return;
            }

            var doc = new XmlDocument();
            doc.Load(manifestPath);
            XmlElement manifest = doc.DocumentElement; // <manifest xmlns:android="...">
            if (manifest == null) return;

            // permisos que ya están (pueden venir de Unity o de otros plugins)
            var present = new System.Collections.Generic.HashSet<string>();
            foreach (XmlNode n in manifest.ChildNodes)
            {
                if (n.NodeType != XmlNodeType.Element || n.Name != "uses-permission") continue;
                var attr = n.Attributes?["name", AndroidNs];
                if (attr != null) present.Add(attr.Value);
            }

            int added = 0;
            XmlNode application = null;
            foreach (XmlNode n in manifest.ChildNodes) if (n.NodeType == XmlNodeType.Element && n.Name == "application") { application = n; break; }
            foreach (var perm in Permissions)
            {
                if (present.Contains(perm)) continue;
                XmlElement el = doc.CreateElement("uses-permission");
                XmlAttribute name = doc.CreateAttribute("android", "name", AndroidNs);
                name.Value = perm;
                el.Attributes.Append(name);
                // por convención los permisos van antes de <application>
                if (application != null) manifest.InsertBefore(el, application);
                else manifest.AppendChild(el);
                added++;
            }

            if (added > 0)
            {
                doc.Save(manifestPath);
                Debug.Log($"[GORRAZO] AndroidManifest: {added} permiso(s) de red local agregado(s).");
            }
        }
    }
}
#endif
