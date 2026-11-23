// ─────────────────────────────────────────────────────────────
// Guarda y carga el progreso local en JSON dentro de
// Application.persistentDataPath (sin servidor, sin cuenta).
// Android: /storage/emulated/0/Android/data/<paquete>/files
// iOS: <app>/Documents (se respalda con iCloud del celular)
// La escritura es "atómica": primero a un .tmp y luego se reemplaza,
// para no perder el progreso si la app se cierra a la mitad.
// ─────────────────────────────────────────────────────────────
using System;
using System.IO;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    public static class ProgressStore
    {
        public const string FileName = "gorrazo_progreso.json";
        static ProgressData _data;

        public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        /// <summary>El progreso en memoria (se carga la primera vez que se pide).</summary>
        public static ProgressData Data => _data ?? (_data = Load());

        public static ProgressData Load()
        {
            var d = new ProgressData();
            try
            {
                if (File.Exists(FilePath)) JsonUtility.FromJsonOverwrite(File.ReadAllText(FilePath), d);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GORRAZO] No se pudo leer el progreso ({e.Message}). Se empieza de cero.");
                d = new ProgressData();
            }
            d.Normalize();
            if (string.IsNullOrEmpty(d.PlayerId)) d.PlayerId = NewPlayerId();
            return d;
        }

        public static void Save()
        {
            if (_data == null) return;
            try
            {
                string path = FilePath, tmp = path + ".tmp";
                File.WriteAllText(tmp, JsonUtility.ToJson(_data, true));
                File.Copy(tmp, path, true);
                File.Delete(tmp);
            }
            catch (Exception e)
            {
                // sin almacenamiento el juego sigue funcionando; solo no se guarda
                Debug.LogWarning($"[GORRAZO] No se pudo guardar el progreso: {e.Message}");
            }
        }

        /// <summary>Borra todo el progreso (útil para probar el premio diario).</summary>
        public static void ResetAll()
        {
            _data = new ProgressData { PlayerId = NewPlayerId() };
            Save();
        }

        /// <summary>ID que el jugador escribe en el portal web de canje (GDD §11.5). Solo presentación.</summary>
        static string NewPlayerId()
        {
            const string abc = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            char L() => abc[UnityEngine.Random.Range(0, abc.Length)];
            return $"GZ-{UnityEngine.Random.Range(1000, 10000)}-{L()}{L()}";
        }
    }
}
