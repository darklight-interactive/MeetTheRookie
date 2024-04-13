using Darklight.UnityExt.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class VirtualMouse : PointerInputModule, ISceneSingleton<VirtualMouse>
{
    public Vector2 position { get; private set; }

    protected override void Awake() {
        base.Awake();
        (this as ISceneSingleton<VirtualMouse>).Initialize();

        position = Vector2.zero;
    }

    protected override void Start() {
        base.Start();
        Invoke("Initialize", 0.1f);

    }

    void Initialize() {
        if (UniversalInputManager.Instance == null) { Debug.LogError("UniversalInputManager is not initialized"); return; }
        UniversalInputManager.MoveInputAction.performed += MoveCursor;
        UniversalInputManager.MoveInputAction.canceled += StopMoveCursor;
    }

    Vector2 intendedMoveDir;
    void MoveCursor(InputAction.CallbackContext context) {
        intendedMoveDir = UniversalInputManager.MoveInputAction.ReadValue<Vector2>();
        intendedMoveDir.y *= -1;
    }

    void StopMoveCursor(InputAction.CallbackContext context) {
        intendedMoveDir = Vector2.zero;
    }

    public override void Process() {
        position += intendedMoveDir;
    }

    void Update() {
        Process();
    }
}
