using Darklight.UnityExt.SceneManagement;
using FMODUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTR_SceneDataObject : BuildSceneDataObject<MTR_SceneData>
{
    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return GetData().Find(x => x.knot == knot);
    }

    public EventReference GetActiveBackgroundMusicEvent()
    {
        MTR_SceneData data = GetActiveSceneData();
        return data.backgroundMusicEvent;
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(MTR_SceneDataObject))]
public class MTR_SceneDataObjectCustomEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    MTR_SceneDataObject _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTR_SceneDataObject)target;
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

        Repaint();
    }
}
#endif