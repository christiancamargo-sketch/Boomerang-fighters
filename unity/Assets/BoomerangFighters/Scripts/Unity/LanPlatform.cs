// ─────────────────────────────────────────────────────────────
// Detalles de cada plataforma para el multijugador local (spec §G).
// ANDROID: por defecto el Wi‑Fi descarta los paquetes broadcast/multicast
// para ahorrar batería; sin un WifiManager.MulticastLock el celular no ve
// el anuncio de las salas. Lo pedimos con AndroidJavaObject (sin plugin
// Java) al anunciar o buscar salas y lo soltamos al salir.
// Permiso necesario: CHANGE_WIFI_MULTICAST_STATE (lo agrega el script de
// editor AndroidLanPermissions al generar el proyecto Gradle).
// iOS: el permiso de red local lo pide el sistema la primera vez; el texto
// lo agrega Editor/IosLocalNetworkPostBuild al Info.plist.
// ─────────────────────────────────────────────────────────────
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    public static class LanPlatform
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        static AndroidJavaObject _lock;
#endif

        /// <summary>true mientras el MulticastLock de Android está tomado (siempre false en otras plataformas).</summary>
        public static bool MulticastHeld { get; private set; }

        /// <summary>Pide el MulticastLock (Android). Se puede llamar varias veces: solo lo toma una.</summary>
        public static void AcquireMulticast()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_lock != null) return;
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var context = activity.Call<AndroidJavaObject>("getApplicationContext"))
                using (var wifi = context.Call<AndroidJavaObject>("getSystemService", "wifi"))
                {
                    if (wifi == null) { Debug.LogWarning("[GORRAZO] Este dispositivo no tiene Wi‑Fi."); return; }
                    _lock = wifi.Call<AndroidJavaObject>("createMulticastLock", "gorrazo-lan");
                    _lock.Call("setReferenceCounted", false);
                    _lock.Call("acquire");
                    MulticastHeld = true;
                }
            }
            catch (System.Exception e)
            {
                // sin el permiso CHANGE_WIFI_MULTICAST_STATE aquí sale SecurityException
                Debug.LogWarning($"[GORRAZO] No se pudo tomar el MulticastLock: {e.Message}. Usa unirse por IP.");
                ReleaseMulticast();
            }
#endif
        }

        /// <summary>Suelta el MulticastLock (Android). Seguro de llamar aunque no se haya tomado.</summary>
        public static void ReleaseMulticast()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_lock != null)
            {
                try { if (_lock.Call<bool>("isHeld")) _lock.Call("release"); }
                catch (System.Exception e) { Debug.LogWarning($"[GORRAZO] Al soltar el MulticastLock: {e.Message}"); }
                _lock.Dispose();
                _lock = null;
            }
#endif
            MulticastHeld = false;
        }
    }
}
