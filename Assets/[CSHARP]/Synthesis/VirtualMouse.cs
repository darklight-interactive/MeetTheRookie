using Darklight.UnityExt.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class VirtualMouse : MonoBehaviour, ISceneSingleton<VirtualMouse>, IPointerEvent
{
    public Vector3 position { get; protected set; }

    public int pointerId => 2;

    public string pointerType => "Virtual";

    public bool isPrimary => false;

    public int button => -1;

    public int pressedButtons => 0;

    Vector3 IPointerEvent.position => position;

    public Vector3 localPosition => position;

    public Vector3 deltaPosition {get; protected set; }

    public float deltaTime { get; protected set; }

    public int clickCount => 0;

    protected void Awake() {
        (this as ISceneSingleton<VirtualMouse>).Initialize();

        position = Vector2.zero;
    }

    protected void Start() {
        Invoke("Initialize", 0.1f);
    }

    void Initialize() {
        if (UniversalInputManager.Instance == null) { Debug.LogError("UniversalInputManager is not initialized"); return; }
        UniversalInputManager.MoveInputAction.performed += MoveCursor;
        UniversalInputManager.MoveInputAction.canceled += StopMoveCursor;
        UniversalInputManager.PrimaryInteractAction.performed += Click;
        UniversalInputManager.PrimaryInteractAction.canceled += StopClick;

        /*var e = new Event();
        e.type = EventType.MouseEnterWindow;
        e.mousePosition = position;
        e.delta = position;
        e.displayIndex = 0;
        var mouseEvent = PointerEnterEvent.GetPooled(e);
        mouseEvent.target = activeHook;
        activeHook.SendEvent(mouseEvent);*/
    }

    Vector2 intendedMoveDir;
    void MoveCursor(InputAction.CallbackContext context) {
        intendedMoveDir = UniversalInputManager.MoveInputAction.ReadValue<Vector2>();
        intendedMoveDir.y *= -1;
    }

    void StopMoveCursor(InputAction.CallbackContext context) {
        intendedMoveDir = Vector2.zero;
    }

    /// <summary>
    /// Can we send move events?
    /// </summary>
    bool downSent = false;
    bool sendDown = false;
    void Click(InputAction.CallbackContext context) {
        sendDown = true;
    }

    bool sendUp = false;
    void StopClick(InputAction.CallbackContext context) {
        sendUp = true;
    }

    VisualElement activeHook;
    void Update() {
        deltaTime = Time.deltaTime;

        Vector3 oldPos = position;
        if (intendedMoveDir != Vector2.zero) {
            position += new Vector3(intendedMoveDir.x, intendedMoveDir.y, 0);
            position = new Vector3(Mathf.Clamp(position.x, 0, Screen.width), Mathf.Clamp(position.y, 0, Screen.height));
        }
        deltaPosition = position - oldPos;

        if (sendUp) {
            downSent = false;
            activeHook.SendEvent(PointerUpEvent.GetPooled(this));
            sendUp = false;
        } else if (downSent) {
            // TODO: Why does this only work when moving left or up?
            activeHook.SendEvent(PointerMoveEvent.GetPooled(this));
        }

        if (sendDown) {
            activeHook.SendEvent(PointerDownEvent.GetPooled(this));
            downSent = true;
            sendDown = false;
        }
    }

    public void HookTo(VisualElement toHook) {
        activeHook = toHook;
    }

    #region Interface crap

    public float pressure => 0;

    public float tangentialPressure => 0;

    public float altitudeAngle => 0;

    public float azimuthAngle => 0;

    public float twist => 0;

    public Vector2 tilt => Vector2.zero;

    public PenStatus penStatus => PenStatus.None;

    public Vector2 radius => Vector2.zero;

    public Vector2 radiusVariance => Vector2.zero;

    public EventModifiers modifiers => EventModifiers.None;

    public bool shiftKey => false;

    public bool ctrlKey => false;

    public bool commandKey => false;

    public bool altKey => false;

    public bool actionKey => false;
    #endregion
}
