using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Darklight.UnityExt.Inky;

namespace Darklight.UnityExt.Inky.Editor
{
    [CustomEditor(typeof(InkyStoryManager), true)]
    public class InkyStoryManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        InkyStoryManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (InkyStoryManager)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Initialize"))
            {
                _script.Initialize();
            }
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
}