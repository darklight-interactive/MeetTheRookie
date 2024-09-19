using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;


#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(menuName = "MeetTheRookie/Library/DialogueBubbleLibrary")]
public class TextBubbleLibrary : EnumObjectScriptableLibrary<Spatial2D.AnchorPoint, Sprite>
{


}

#if UNITY_EDITOR
[CustomEditor(typeof(TextBubbleLibrary))]
public class DialogueBubbleLibraryCustomEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    TextBubbleLibrary _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (TextBubbleLibrary)target;
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (GUILayout.Button("Set To Defaults"))
        {
            _script.Library.SetToDefaults();
        }


        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

