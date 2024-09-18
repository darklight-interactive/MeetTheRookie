using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;


#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(menuName = "MeetTheRookie/Library/DialogueBubbleLibrary")]
public class DialogueBubbleLibrary : EnumObjectScriptableLibrary<Spatial2D.AnchorPoint, Sprite>
{
    public UXML_UIDocumentPreset dialogueBubblePreset;
    public Sprite defaultSprite;

    public override void AddKeys(IEnumerable<Spatial2D.AnchorPoint> keys)
    {
        foreach (var key in keys)
        {
            if (!ContainsKey(key))
            {
                Add(key, this.CreateDefaultValue());
            }
            else if (this[key] == null)
            {
                this[key] = this.CreateDefaultValue();
            }
        }
    }

    public override Sprite CreateDefaultValue()
    {
        return defaultSprite;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueBubbleLibrary))]
public class DialogueBubbleLibraryCustomEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    DialogueBubbleLibrary _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (DialogueBubbleLibrary)target;
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (GUILayout.Button("Set To Defaults"))
        {
            _script.SetToDefaults();
        }

        if (GUILayout.Button("Update Serialized Values"))
        {
            _script.UpdateSerializedValues();
        }

        if (GUILayout.Button("Clear"))
        {
            _script.Clear();
        }


        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

