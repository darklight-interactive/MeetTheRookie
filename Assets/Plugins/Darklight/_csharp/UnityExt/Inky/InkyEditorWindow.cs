using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

namespace Darklight.UnityExt.Inky
{
    public class InkyEditorWindow : EditorWindow
    {
        public Vector2 _scrollPosition;

        [MenuItem("DarklightExt/InkyEditorWindow")]
        public static void ShowWindow()
        {
            GetWindow<InkyEditorWindow>();
        }

        private void OnGUI()
        {
            InkyStoryManager storyManager = InkyStoryManager.Instance;
            if (storyManager == null || InkyStoryManager.GlobalStoryObject == null)
            {
                EditorGUILayout.HelpBox("Inky Story Manager is not initialized.", MessageType.Warning);
                return;
            }
            SerializedObject serializedObject = new SerializedObject(InkyStoryManager.Instance);
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            List<string> globalKnots = InkyStoryObject.GetAllKnots(storyObject.StoryValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_globalStoryObject"));

            // --------------------- [[ DISPLAY KNOTS ]] --------------------- >>
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_speakerList"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_globalKnots"));

            // --------------------- [[ DISPLAY VARIABLES ]] --------------------- >>
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to see Inky runtime values.", MessageType.Info);
                return;
            }


            // Fetch the global variables from the story
            List<InkyVariable> variables = InkyStoryObject.GetVariables(storyObject.StoryValue);

            // Display the variables
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (InkyVariable variable in variables)
            {
                EditorGUILayout.LabelField(variable.Key, variable.Value?.ToString() ?? "null");
            }
            EditorGUILayout.EndScrollView();
        }
    }
}

