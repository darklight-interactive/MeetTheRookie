using System.Collections.Generic;
using System.IO;
using System.Linq;

using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.BuildScene
{
    /// <summary>
    /// A generic version of the BuildSceneManager class that uses the BuildSceneData class as the data type.
    /// </summary>
    public class DefaultBuildSceneManager : BuildSceneManager<BuildSceneData> { }

    // ==================== [[ EDITOR ]] ====================================
#if UNITY_EDITOR
    [CustomEditor(typeof(DefaultBuildSceneManager), true)]
    public class BuildSceneManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        DefaultBuildSceneManager _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (DefaultBuildSceneManager)target;
            _script.Initialize();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Initialize")) { _script.Initialize(); }
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
