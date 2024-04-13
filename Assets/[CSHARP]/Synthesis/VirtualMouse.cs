using Darklight.UnityExt.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class VirtualMouse : MonoBehaviour, ISceneSingleton<VirtualMouse>
{
    public Vector2 position { get; protected set; }

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
    /// We need to send events during the event tick.
    /// </summary>
    bool sendDown = false;
    void Click(InputAction.CallbackContext context) {
        sendDown = true;
    }

    /// <summary>
    /// Same with up events.
    /// </summary>
    bool sendUp = false;
    void StopClick(InputAction.CallbackContext context) {
        sendUp = true;
    }

    VisualElement activeHook;
    void Update() {
        Vector2 oldPos = position;
        if (intendedMoveDir != Vector2.zero) {
            position += intendedMoveDir;
            position = new Vector2(Mathf.Clamp(position.x, 0, Screen.width), Mathf.Clamp(position.y, 0, Screen.height));

            if (position - oldPos != Vector2.zero && activeHook != null) {
                var e = new Event();
                e.type = EventType.MouseMove;
                e.mousePosition = position;
                e.delta = position - oldPos;

                // TODO: Why is this only detected on SynthesisDraggable when we move left?
                var moveEvent = PointerMoveEvent.GetPooled(e);

                activeHook.SendEvent(moveEvent);
            }
        }
        if (sendDown) {
            var e = new Event();
            e.type = EventType.MouseDown;
            e.mousePosition = position;
            e.button = 0;
            e.delta = Vector2.zero;

            var mouseEvent = PointerDownEvent.GetPooled(e);
            if (activeHook != null) {
                activeHook.SendEvent(mouseEvent);
            }
            sendDown = false;
        }

        if (sendUp) {
            var e = new Event();
            e.type = EventType.MouseUp;
            e.mousePosition = position;
            e.delta = Vector2.zero;
            e.button = 0;

            var mouseEvent = PointerUpEvent.GetPooled(e);
            if (activeHook != null) {
                activeHook.SendEvent(mouseEvent);
            }
            sendUp = false;
        }
    }

    public void HookTo(VisualElement toHook) {
        activeHook = toHook;
    }
}
