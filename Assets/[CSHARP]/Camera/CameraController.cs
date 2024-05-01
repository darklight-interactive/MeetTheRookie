using Darklight.Game;
using Darklight.Game.Camera;
using UnityEngine;
using Darklight.Game.Utility;
using System.Collections.Generic;
using static Darklight.UnityExt.CustomInspectorGUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum CameraState
{
    DEFAULT,
    FOLLOW_TARGET,
    CLOSE_UP
}

[ExecuteAlways]
public class CameraController : CameraRig
{
    [Space(10), Header("Camera Controller")]
    [SerializeField] private FiniteStateMachine<CameraState> _stateMachine;
    [SerializeField] List<FiniteCameraState> _states;


    public void Awake()
    {

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    SerializedObject _serializedObject;
    CameraController _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (CameraController)target;
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif


// ============================ CAMERA STATE MACHINE ============================ >>



/// <summary>
/// This state is the default state for the camera. It does not follow any target and is in a fixed position.
/// </summary>
public class FiniteCameraState : FiniteState<CameraState>
{
    protected CameraRig _cameraRig;
    private float _initialFOV;

    /// <param name="args">
    /// args[0] = CameraRig
    /// </param>
    public FiniteCameraState(CameraState stateType, params object[] args) : base(stateType, args)
    {
        _cameraRig = (CameraRig)args[0];
    }

    public override void Enter()
    {
        _initialFOV = _cameraRig.cameraFov;
    }

    public override void Exit()
    {
        _cameraRig.cameraFov = _initialFOV;
    }

    public override void Execute()
    {
        // Do nothing
    }
}


