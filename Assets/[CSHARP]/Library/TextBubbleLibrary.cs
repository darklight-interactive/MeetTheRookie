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
    protected override EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite> CreateNewLibrary()
    {
        return library = new EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite>()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = false,
            RequiredKeys = new List<Spatial2D.AnchorPoint>()
            {
                Spatial2D.AnchorPoint.CENTER,
                Spatial2D.AnchorPoint.TOP_LEFT,
                Spatial2D.AnchorPoint.TOP_RIGHT,
                Spatial2D.AnchorPoint.BOTTOM_LEFT,
                Spatial2D.AnchorPoint.BOTTOM_RIGHT,
            }
        };
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TextBubbleLibrary))]
    public class TextBubbleLibraryCustomEditor : UnityEditor.Editor
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

            if (GUILayout.Button("CreateNewLibrary"))
            {
                _script.CreateNewLibrary();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}

