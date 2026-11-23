// Stubs SOLO para compilar fuera de Unity. En Unity se usa el paquete real com.unity.inputsystem.
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.InputSystem.Controls
{
    public class ButtonControl { public bool isPressed => false; public bool wasPressedThisFrame => false; public bool wasReleasedThisFrame => false; }
    public class KeyControl : ButtonControl { }
    public class StickControl { public Vector2 ReadValue() => default; }
    public class Vector2Control { public Vector2 ReadValue() => default; }
}
namespace UnityEngine.InputSystem
{
    using UnityEngine.InputSystem.Controls;
    public enum Key { None, Space, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z }
    public enum TouchPhase { None, Began, Moved, Ended, Canceled, Stationary }
    public class Keyboard { public static Keyboard current => null; public KeyControl this[Key k] => null; public KeyControl spaceKey => null; }
    public class Mouse { public static Mouse current => null; public Vector2Control position => null; public ButtonControl leftButton => null; public ButtonControl rightButton => null; }
    public class Gamepad
    {
        public static IReadOnlyList<Gamepad> all => new List<Gamepad>();
        public StickControl leftStick => null; public StickControl rightStick => null;
        public ButtonControl buttonSouth => null; public ButtonControl buttonWest => null; public ButtonControl buttonEast => null;
        public ButtonControl rightTrigger => null; public ButtonControl leftTrigger => null; public ButtonControl rightShoulder => null; public ButtonControl leftShoulder => null;
    }
}
namespace UnityEngine.InputSystem.EnhancedTouch
{
    public struct Touch { public static IReadOnlyList<Touch> activeTouches => new List<Touch>(); public int touchId => 0; public Vector2 screenPosition => default; public UnityEngine.InputSystem.TouchPhase phase => default; }
    public static class EnhancedTouchSupport { public static bool enabled => false; public static void Enable() { } }
}
