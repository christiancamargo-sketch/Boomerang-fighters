// ─────────────────────────────────────────────────────────────
// "GAME FEEL": reacciona a los SimEvent con partículas, manchas de
// tinta en el piso, temblor de cámara y sonido (opcional).
// v0.3: caída al vacío (polvito en la orilla y gotas que se van al
// hueco, sin mancha ni explosión) y caja DROP (aviso, golpe con polvo,
// pedazos de cartón y temblor).
// Todo es presentación: nunca cambia el resultado de la partida.
// ─────────────────────────────────────────────────────────────
using System.Collections.Generic;
using BoomerangFighters.Core;
using UnityEngine;

namespace BoomerangFighters.UnityView
{
    [System.Serializable]
    public sealed class SfxClips
    {
        public AudioClip Throw, Catch, Ko, Clank, Dash, Pickup, Explode, Beep, Win;
        [Tooltip("v0.3 (opcionales). Sin clip: Fall usa Ko, DropWarn usa Beep y DropLand usa Explode")]
        public AudioClip Fall, DropWarn, DropLand;
    }

    public sealed class FxDirector
    {
        sealed class Particle { public Transform T; public Vector3 V; public float Life, Max, Size, Gravity; public bool Sinks; }

        readonly Transform _root;
        readonly List<Particle> _live = new List<Particle>();
        readonly Stack<GameObject> _pool = new Stack<GameObject>();
        readonly Queue<GameObject> _splats = new Queue<GameObject>();
        readonly AudioSource _audio;
        readonly SfxClips _clips;
        public float Shake;
        public const int MaxSplats = 160;

        public FxDirector(Transform parent, AudioSource audio, SfxClips clips)
        {
            _root = new GameObject("FX").transform;
            _root.SetParent(parent, false);
            _audio = audio;
            _clips = clips ?? new SfxClips();
        }

        public void Handle(IReadOnlyList<SimEvent> events)
        {
            foreach (var e in events)
            {
                var pos = Greybox.ToUnity(e.Pos, 0.8f);
                Color pc = e.A >= 0 ? Greybox.Hex(Greybox.PlayerColors[e.A % Greybox.PlayerColors.Length]) : Color.white;
                switch (e.Type)
                {
                    case SimEventType.KO:
                        if ((KoCause)e.Extra == KoCause.Vacio)
                        {
                            // V7: no explota ni mancha el piso; el muñequito cae (WorldView) y aquí va el "fuuu"
                            Burst(Greybox.ToUnity(e.Pos, 0.2f), Greybox.Hex(Greybox.Bone), 8, 2.5f, 0.09f);
                            Sink(Greybox.ToUnity(e.Pos, 0.4f), pc, 10);
                            Shake = Mathf.Max(Shake, 0.27f);
                            Play(_clips.Fall != null ? _clips.Fall : _clips.Ko);
                            break;
                        }
                        Burst(pos, pc, 26, 8f);
                        Splat(e.Pos, 1.1f, pc);
                        Shake = Mathf.Max(Shake, 0.45f);
                        Play(_clips.Ko);
                        break;
                    // ── v0.3: caja DROP (Pop-up Store) ──
                    case SimEventType.DropWarn:
                        Play(_clips.DropWarn != null ? _clips.DropWarn : _clips.Beep);
                        break;
                    case SimEventType.DropLand:
                    {
                        float radius = e.Value > 0f ? e.Value : 1.1f;
                        Ring(e.Pos, radius, Greybox.Hex(0xD9D2C4), 18);                                     // polvo que sale rasante
                        Burst(Greybox.ToUnity(e.Pos, 0.3f), Greybox.Hex(0xD9D2C4), 14, 5f, 0.14f);          // polvo
                        Burst(Greybox.ToUnity(e.Pos, 0.5f), Greybox.Hex(0xC89B5A), 12, 7f, 0.16f);          // pedazos de cartón
                        if (e.Extra != 0) Burst(Greybox.ToUnity(e.Pos, 0.6f), Greybox.Hex(PowerUpInfo.Color((PowerUp)e.Extra)), 8, 4f, 0.1f); // deja una lata
                        Shake = Mathf.Max(Shake, 0.5f);
                        Play(_clips.DropLand != null ? _clips.DropLand : _clips.Explode);
                        break;
                    }
                    case SimEventType.Clank:
                    case SimEventType.Deflect:
                        Burst(Greybox.ToUnity(e.Pos, 1.05f), new Color(1f, 0.96f, 0.6f), 10, 6f, 0.05f);
                        // "Te la regreso" (Extra=1): chispazo del color de quien desvió
                        if (e.Type == SimEventType.Deflect && e.Extra == 1) Burst(Greybox.ToUnity(e.Pos, 1.05f), pc, 14, 7f, 0.08f);
                        if (e.Type == SimEventType.Deflect || e.Extra == 2) Shake = Mathf.Max(Shake, e.Extra == 1 ? 0.26f : 0.18f);
                        Play(_clips.Clank);
                        break;
                    // ── v0.2 ──
                    case SimEventType.Shove:
                        Burst(Greybox.ToUnity(e.Pos, 0.6f), Color.white, e.Extra == 0 ? 4 : 12, e.Extra == 0 ? 2f : 5f, 0.08f);
                        if (e.Extra == 1) Shake = Mathf.Max(Shake, 0.15f);
                        Play(_clips.Dash);
                        break;
                    case SimEventType.Marked:
                        Burst(Greybox.ToUnity(e.Pos, 2.2f), Greybox.Hex(Greybox.Magenta), 8, 2.5f, 0.07f);
                        Play(_clips.Beep);
                        break;
                    case SimEventType.PuddleStart:
                        Burst(Greybox.ToUnity(e.Pos, 0.3f), new Color(0.07f, 0.07f, 0.07f), 14, 4f, 0.1f);
                        break;
                    case SimEventType.TagWall:
                        Burst(Greybox.ToUnity(e.Pos, 0.9f), pc, 16, 4f, 0.08f);
                        Play(_clips.Pickup);
                        break;
                    case SimEventType.HotCanPass:
                        Burst(Greybox.ToUnity(e.Pos, 2.4f), Greybox.Hex(0xD55E00), 10, 3f, 0.08f);
                        Play(_clips.Pickup);
                        break;
                    case SimEventType.HotCanExplode:
                        Burst(pos, Greybox.Hex(0xD55E00), 26, 10f);
                        Burst(pos, new Color(0.07f, 0.07f, 0.07f), 20, 8f);
                        Splat(e.Pos, 1.8f, Greybox.Hex(0xD55E00));
                        Shake = Mathf.Max(Shake, 0.55f);
                        Play(_clips.Explode);
                        break;
                    case SimEventType.SuddenDeathWarn:
                        Shake = Mathf.Max(Shake, 0.2f);
                        Play(_clips.Beep);
                        break;
                    case SimEventType.SuddenDeathStart:
                        Shake = Mathf.Max(Shake, 0.3f);
                        break;
                    case SimEventType.Explode:
                        Burst(pos, new Color(0.07f, 0.07f, 0.07f), 30, 9f);
                        Splat(e.Pos, 1.6f, new Color(0.07f, 0.07f, 0.07f));
                        Shake = Mathf.Max(Shake, 0.45f);
                        Play(_clips.Explode);
                        break;
                    case SimEventType.Paint:
                        Splat(e.Pos, 0.35f, Greybox.Hex(0x00E676, 0.7f));
                        break;
                    case SimEventType.Throw: Play(_clips.Throw); break;
                    case SimEventType.Catch: Play(_clips.Catch); break;
                    case SimEventType.Dash: Burst(Greybox.ToUnity(e.Pos, 0.3f), Color.white, 6, 2f); Play(_clips.Dash); break;
                    case SimEventType.PickupTaken: Burst(pos, Greybox.Hex(PowerUpInfo.Color((PowerUp)e.Extra)), 12, 3f); Play(_clips.Pickup); break;
                    case SimEventType.Countdown: Play(_clips.Beep); break;
                    case SimEventType.MatchEnd: Play(_clips.Win); break;
                }
            }
        }

        void Play(AudioClip clip) { if (clip != null && _audio != null) _audio.PlayOneShot(clip); }

        GameObject Take(Color c)
        {
            var go = _pool.Count > 0 ? _pool.Pop() : Greybox.Prim(PrimitiveType.Sphere, _root, c, Vector3.zero, Vector3.one, "Partícula");
            go.SetActive(true);
            go.GetComponent<Renderer>().material.color = c;
            return go;
        }

        public void Burst(Vector3 at, Color c, int n, float speed, float size = 0.12f)
        {
            for (int i = 0; i < n && _live.Count < 300; i++)
            {
                var go = Take(c);
                go.transform.position = at;
                var dir = Random.insideUnitSphere; dir.y = Mathf.Abs(dir.y) + 0.3f;
                _live.Add(new Particle { T = go.transform, V = dir.normalized * speed * Random.Range(0.4f, 1.2f), Life = Random.Range(0.4f, 0.9f), Max = 0.9f, Size = size * Random.Range(0.7f, 1.6f), Gravity = 18f });
            }
        }

        /// <summary>Gotas que caen DENTRO de un vacío: no rebotan en el piso, siguen bajando hasta perderse.</summary>
        public void Sink(Vector3 at, Color c, int n)
        {
            for (int i = 0; i < n && _live.Count < 300; i++)
            {
                var go = Take(c);
                go.transform.position = at;
                var v = Random.insideUnitSphere * 1.2f;
                v.y = Random.Range(0.5f, 2.5f);
                _live.Add(new Particle { T = go.transform, V = v, Life = Random.Range(0.5f, 0.8f), Max = 0.8f, Size = 0.12f * Random.Range(0.7f, 1.5f), Gravity = 22f, Sinks = true });
            }
        }

        /// <summary>Anillo de polvo que sale rasante desde la orilla de un círculo (la caja DROP al caer).</summary>
        public void Ring(Vec2 center, float radius, Color c, int n)
        {
            for (int i = 0; i < n && _live.Count < 300; i++)
            {
                float a = (i + Random.value) / n * Mathf.PI * 2f;
                var dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
                var go = Take(c);
                go.transform.position = Greybox.ToUnity(center, 0.15f) + dir * (radius * 0.6f);
                _live.Add(new Particle { T = go.transform, V = dir * Random.Range(3.5f, 6f) + Vector3.up * Random.Range(0.4f, 1.4f), Life = Random.Range(0.35f, 0.6f), Max = 0.6f, Size = 0.2f * Random.Range(0.7f, 1.3f), Gravity = 6f });
            }
        }

        /// <summary>Mancha de tinta en el piso: un disco principal + gotitas (el escenario se vuelve mural).</summary>
        public void Splat(Vec2 at, float radius, Color c)
        {
            var parent = new GameObject("Mancha").transform;
            parent.SetParent(_root, false);
            parent.position = Greybox.ToUnity(at, 0.012f + _splats.Count * 0.00002f);
            Greybox.Prim(PrimitiveType.Cylinder, parent, c, Vector3.zero, new Vector3(radius * 2f, 0.002f, radius * 1.7f), "Disco");
            for (int i = 0; i < 6; i++)
            {
                var a = Random.value * Mathf.PI * 2f;
                float d = radius * Random.Range(1.1f, 1.9f), s = radius * Random.Range(0.15f, 0.35f);
                Greybox.Prim(PrimitiveType.Cylinder, parent, c, new Vector3(Mathf.Cos(a) * d, 0f, Mathf.Sin(a) * d), new Vector3(s, 0.002f, s), "Gota");
            }
            parent.rotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
            _splats.Enqueue(parent.gameObject);
            while (_splats.Count > MaxSplats) Greybox.DestroyTree(_splats.Dequeue());
        }

        public void Tick(float dt)
        {
            for (int i = _live.Count - 1; i >= 0; i--)
            {
                var p = _live[i];
                p.Life -= dt;
                p.V.y -= p.Gravity * dt;
                p.T.position += p.V * dt;
                if (!p.Sinks && p.T.position.y < 0.05f) { var pos = p.T.position; pos.y = 0.05f; p.T.position = pos; p.V = new Vector3(p.V.x * 0.6f, -p.V.y * 0.3f, p.V.z * 0.6f); }
                p.T.localScale = Vector3.one * (p.Size * Mathf.Max(0f, p.Life / p.Max));
                if (p.Life <= 0f) { p.T.gameObject.SetActive(false); _pool.Push(p.T.gameObject); _live.RemoveAt(i); }
            }
            Shake = Mathf.Max(0f, Shake - dt * 2.2f);
        }

        public void ClearSplats()
        {
            while (_splats.Count > 0) Greybox.DestroyTree(_splats.Dequeue());
        }
    }

    /// <summary>Encuadra toda la arena (como el prototipo web) y aplica el temblor.</summary>
    public sealed class CameraRig
    {
        readonly Camera _cam;
        public CameraRig(Camera cam) { _cam = cam; }

        public void Frame(ArenaDef arena, float shake, float elevationDeg = 63f)
        {
            float elev = elevationDeg * Mathf.Deg2Rad;
            float vfov = _cam.fieldOfView * Mathf.Deg2Rad;
            float hfov = 2f * Mathf.Atan(Mathf.Tan(vfov / 2f) * _cam.aspect);
            float W = arena.Width + 1.5f, H = arena.Depth + 2.2f;
            float dW = W / 2f / Mathf.Tan(hfov / 2f);
            float dH = H / 2f * Mathf.Sin(elev) / Mathf.Tan(vfov / 2f) + H * 0.12f;
            float D = Mathf.Max(dW, dH) * 1.02f;
            var target = new Vector3(0f, 0f, -0.6f);
            var pos = target + new Vector3(0f, Mathf.Sin(elev) * D, -Mathf.Cos(elev) * D);
            float s = shake * shake * 0.8f;
            pos += new Vector3(Random.Range(-s, s), Random.Range(-s, s), Random.Range(-s, s));
            _cam.transform.position = pos;
            _cam.transform.LookAt(target);
        }
    }
}
