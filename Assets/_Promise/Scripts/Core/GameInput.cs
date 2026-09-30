using UnityEngine;
using UnityEngine.InputSystem;

// Single source of player input for keyboard and gamepad (Xbox / PlayStation / Switch pads all map to
// the generic Gamepad layout). Gameplay code reads from here instead of the legacy Input Manager.
public static class GameInput
{
    static InputActionMap _map;
    static InputAction _move, _jump, _grab, _pause, _peek, _navigate, _submit, _cancel;

    const float k_StickDeadZone = 0.35f;

    static InputActionMap Map
    {
        get
        {
            if (_map == null)
                Create();
            return _map;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnPlay()
    {
        // Supports entering Play mode with domain reload disabled.
        _map?.Disable();
        _map?.Dispose();
        _map = null;
    }

    static void Create()
    {
        _map = new InputActionMap("Promise");

        _move = _map.AddAction("Move", InputActionType.Value);
        _move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        _move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
        _move.AddBinding("<Gamepad>/leftStick");
        _move.AddBinding("<Gamepad>/dpad");

        _jump = _map.AddAction("Jump", InputActionType.Button);
        _jump.AddBinding("<Keyboard>/space");
        _jump.AddBinding("<Gamepad>/buttonSouth");

        // Grab / push / pull boxes. Held.
        _grab = _map.AddAction("Grab", InputActionType.Button);
        _grab.AddBinding("<Keyboard>/e");
        _grab.AddBinding("<Keyboard>/leftShift");
        _grab.AddBinding("<Mouse>/leftButton");
        _grab.AddBinding("<Gamepad>/buttonWest");
        _grab.AddBinding("<Gamepad>/rightTrigger");

        _pause = _map.AddAction("Pause", InputActionType.Button);
        _pause.AddBinding("<Keyboard>/escape");
        _pause.AddBinding("<Gamepad>/start");

        // Camera peek (hold to look further ahead).
        _peek = _map.AddAction("Peek", InputActionType.Button);
        _peek.AddBinding("<Mouse>/rightButton");
        _peek.AddBinding("<Keyboard>/q");
        _peek.AddBinding("<Gamepad>/leftShoulder");

        // Menus
        _navigate = _map.AddAction("Navigate", InputActionType.Value);
        _navigate.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        _navigate.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
        _navigate.AddBinding("<Gamepad>/leftStick");
        _navigate.AddBinding("<Gamepad>/dpad");

        _submit = _map.AddAction("Submit", InputActionType.Button);
        _submit.AddBinding("<Keyboard>/enter");
        _submit.AddBinding("<Keyboard>/numpadEnter");
        _submit.AddBinding("<Keyboard>/space");
        _submit.AddBinding("<Gamepad>/buttonSouth");

        _cancel = _map.AddAction("Cancel", InputActionType.Button);
        _cancel.AddBinding("<Keyboard>/escape");
        _cancel.AddBinding("<Keyboard>/backspace");
        _cancel.AddBinding("<Gamepad>/buttonEast");

        _map.Enable();
    }

    static float Axis(float v) => Mathf.Abs(v) < k_StickDeadZone ? 0f : Mathf.Sign(v);

    // Gameplay
    /// <summary>Horizontal movement snapped to -1, 0 or 1 (digital, like the old GetAxisRaw).</summary>
    public static float MoveX { get { var _ = Map; return Axis(_move.ReadValue<Vector2>().x); } }
    public static bool JumpPressed { get { var _ = Map; return _jump.WasPressedThisFrame(); } }
    public static bool JumpHeld { get { var _ = Map; return _jump.IsPressed(); } }
    public static bool GrabPressed { get { var _ = Map; return _grab.WasPressedThisFrame(); } }
    public static bool GrabHeld { get { var _ = Map; return _grab.IsPressed(); } }
    public static bool GrabReleased { get { var _ = Map; return _grab.WasReleasedThisFrame(); } }
    public static bool PausePressed { get { var _ = Map; return _pause.WasPressedThisFrame(); } }
    public static bool PeekHeld { get { var _ = Map; return _peek.IsPressed(); } }

    // Menus
    /// <summary>Menu direction snapped to -1, 0 or 1 per axis.</summary>
    public static Vector2Int Navigate
    {
        get
        {
            var _ = Map;
            Vector2 v = _navigate.ReadValue<Vector2>();
            return new Vector2Int((int)Axis(v.x), (int)Axis(v.y));
        }
    }
    public static bool SubmitPressed { get { var _ = Map; return _submit.WasPressedThisFrame(); } }
    public static bool CancelPressed { get { var _ = Map; return _cancel.WasPressedThisFrame(); } }
}
