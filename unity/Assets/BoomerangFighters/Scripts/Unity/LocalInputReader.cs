// ─────────────────────────────────────────────────────────────
// Lee teclado, mouse, controles y pantalla táctil (Input System)
// y los convierte en InputCommand para la simulación.
// Los botones "de un cuadro" (presionar/soltar) se guardan hasta
// que la simulación los consume, para no perder toques rápidos.
// ─────────────────────────────────────────────────────────────
using BoomerangFighters.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using ETouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.InputSystem.EnhancedTouch;

namespace BoomerangFighters.UnityView
{
    public enum InputDevicePreset { KeyboardWASD, KeyboardIJKL, Gamepad, Touch }

    public sealed class LocalInputReader
    {
        public InputDevicePreset Preset;
        public int GamepadIndex;
        public Camera AimCamera;            // para apuntar con el mouse
        public System.Func<Vector3> PlayerWorldPos;

        InputCommand _latched;
        bool _hasLatched;

        // estado táctil (lo dibuja TouchOverlay)
        public int StickTouchId = -1, ThrowTouchId = -1;
        public Vector2 StickOrigin, StickPos, ThrowOrigin, ThrowPos;
        public Rect ThrowRect, MeleeRect, DashRect;

        public LocalInputReader(InputDevicePreset preset, int gamepadIndex = 0)
        {
            Preset = preset;
            GamepadIndex = gamepadIndex;
            if (preset == InputDevicePreset.Touch && !EnhancedTouchSupport.enabled) EnhancedTouchSupport.Enable();
        }

        /// <summary>Llamar en Update(): acumula lo que pasó en este cuadro.</summary>
        public void Sample()
        {
            var c = ReadDevice();
            if (!_hasLatched) { _latched = c; _hasLatched = true; return; }
            var edges = _latched.Buttons & (InputButtons.ThrowPressed | InputButtons.ThrowReleased | InputButtons.MeleePressed | InputButtons.DashPressed);
            _latched = c;
            _latched.Buttons |= edges;
        }

        /// <summary>La simulación toma la entrada del tick y se limpian los "bordes".</summary>
        public InputCommand ConsumeTick()
        {
            var c = _latched;
            _latched = _latched.WithoutEdges();
            return c;
        }

        InputCommand ReadDevice()
        {
            switch (Preset)
            {
                case InputDevicePreset.KeyboardWASD: return ReadKeyboard(Key.W, Key.S, Key.A, Key.D, Key.E, Key.Q, Key.F, true);
                case InputDevicePreset.KeyboardIJKL: return ReadKeyboard(Key.I, Key.K, Key.J, Key.L, Key.O, Key.U, Key.H, false);
                case InputDevicePreset.Gamepad: return ReadGamepad();
                default: return ReadTouch();
            }
        }

        static void Edge(ref InputCommand c, ButtonControl b, InputButtons pressed, InputButtons held = InputButtons.None, InputButtons released = InputButtons.None)
        {
            if (b == null) return;
            if (b.wasPressedThisFrame) c.Buttons |= pressed;
            if (held != InputButtons.None && b.isPressed) c.Buttons |= held;
            if (released != InputButtons.None && b.wasReleasedThisFrame) c.Buttons |= released;
        }

        InputCommand ReadKeyboard(Key up, Key down, Key left, Key right, Key thr, Key melee, Key dash, bool useMouse)
        {
            var c = new InputCommand();
            var kb = Keyboard.current;
            if (kb == null) return c;
            float x = (kb[right].isPressed ? 1f : 0f) - (kb[left].isPressed ? 1f : 0f);
            float z = (kb[up].isPressed ? 1f : 0f) - (kb[down].isPressed ? 1f : 0f);
            // en la simulación, +y = hacia la cámara (abajo en pantalla): "arriba" del teclado es -y
            c.Move = new Vec2(x, -z);
            Edge(ref c, kb[thr], InputButtons.ThrowPressed, InputButtons.ThrowHeld, InputButtons.ThrowReleased);
            Edge(ref c, kb[melee], InputButtons.MeleePressed);
            Edge(ref c, kb[dash], InputButtons.DashPressed);
            if (useMouse)
            {
                Edge(ref c, kb.spaceKey, InputButtons.DashPressed);
                var mouse = Mouse.current;
                if (mouse != null)
                {
                    Edge(ref c, mouse.leftButton, InputButtons.ThrowPressed, InputButtons.ThrowHeld, InputButtons.ThrowReleased);
                    Edge(ref c, mouse.rightButton, InputButtons.MeleePressed);
                    if (AimCamera != null && PlayerWorldPos != null)
                    {
                        var ray = AimCamera.ScreenPointToRay(mouse.position.ReadValue());
                        var ground = new Plane(Vector3.up, Vector3.zero);
                        if (ground.Raycast(ray, out float dist))
                        {
                            var hit = ray.GetPoint(dist) - PlayerWorldPos();
                            if (hit.sqrMagnitude > 0.16f) { c.Aim = new Vec2(hit.x, ToSimY(hit.z)); c.Buttons |= InputButtons.HasAim; }
                        }
                    }
                }
            }
            return c;
        }

        InputCommand ReadGamepad()
        {
            var c = new InputCommand();
            var all = Gamepad.all;
            if (GamepadIndex >= all.Count) return c;
            var gp = all[GamepadIndex];
            var ls = gp.leftStick.ReadValue();
            c.Move = new Vec2(ls.x, -ls.y);
            var rs = gp.rightStick.ReadValue();
            if (rs.sqrMagnitude > 0.12f) { c.Aim = new Vec2(rs.x, -rs.y); c.Buttons |= InputButtons.HasAim; }
            bool thrHeld = gp.buttonSouth.isPressed || gp.rightTrigger.isPressed;
            if (gp.buttonSouth.wasPressedThisFrame || gp.rightTrigger.wasPressedThisFrame) c.Buttons |= InputButtons.ThrowPressed;
            if (gp.buttonSouth.wasReleasedThisFrame || gp.rightTrigger.wasReleasedThisFrame) c.Buttons |= InputButtons.ThrowReleased;
            if (thrHeld) c.Buttons |= InputButtons.ThrowHeld;
            if (gp.buttonWest.wasPressedThisFrame || gp.rightShoulder.wasPressedThisFrame) c.Buttons |= InputButtons.MeleePressed;
            if (gp.buttonEast.wasPressedThisFrame || gp.leftTrigger.wasPressedThisFrame || gp.leftShoulder.wasPressedThisFrame) c.Buttons |= InputButtons.DashPressed;
            return c;
        }

        // Táctil: mitad izquierda = joystick flotante; botones a la derecha.
        // Arrastrar desde LANZAR apunta a mano; un toque usa ayuda de apuntado.
        InputCommand ReadTouch()
        {
            var c = new InputCommand();
            float w = Screen.width, h = Screen.height;
            float u = Mathf.Min(w, h);
            // coordenadas de pantalla del Input System: origen abajo-izquierda
            ThrowRect = new Rect(w - u * 0.30f, u * 0.05f, u * 0.25f, u * 0.25f);
            MeleeRect = new Rect(w - u * 0.52f, u * 0.07f, u * 0.17f, u * 0.17f);
            DashRect = new Rect(w - u * 0.24f, u * 0.33f, u * 0.17f, u * 0.17f);

            bool stickSeen = false, throwSeen = false;
            foreach (var t in ETouch.activeTouches)
            {
                var pos = t.screenPosition;
                bool began = t.phase == UnityEngine.InputSystem.TouchPhase.Began;
                if (began)
                {
                    if (ThrowRect.Contains(pos) && ThrowTouchId < 0) { ThrowTouchId = t.touchId; ThrowOrigin = pos; c.Buttons |= InputButtons.ThrowPressed; }
                    else if (MeleeRect.Contains(pos)) c.Buttons |= InputButtons.MeleePressed;
                    else if (DashRect.Contains(pos)) c.Buttons |= InputButtons.DashPressed;
                    else if (pos.x < w * 0.5f && StickTouchId < 0) { StickTouchId = t.touchId; StickOrigin = pos; }
                }
                if (t.touchId == StickTouchId) { stickSeen = true; StickPos = pos; }
                if (t.touchId == ThrowTouchId) { throwSeen = true; ThrowPos = pos; }
            }
            if (!stickSeen) StickTouchId = -1;
            if (ThrowTouchId >= 0 && !throwSeen) { ThrowTouchId = -1; c.Buttons |= InputButtons.ThrowReleased; }

            if (StickTouchId >= 0)
            {
                var d = (StickPos - StickOrigin) / (u * 0.12f);
                if (d.magnitude > 1f) d.Normalize();
                c.Move = new Vec2(d.x, -d.y);
            }
            if (ThrowTouchId >= 0)
            {
                c.Buttons |= InputButtons.ThrowHeld;
                var drag = ThrowPos - ThrowOrigin;
                if (drag.magnitude > u * 0.04f) { c.Aim = new Vec2(drag.x, -drag.y); c.Buttons |= InputButtons.HasAim; }
            }
            if ((c.Buttons & InputButtons.HasAim) == 0) c.Buttons |= InputButtons.WantsAssist;
            return c;
        }

        /// <summary>Unity Z (hacia el fondo) → simulación y (hacia la cámara).</summary>
        public static float ToSimY(float unityZ) => -unityZ;
    }
}
