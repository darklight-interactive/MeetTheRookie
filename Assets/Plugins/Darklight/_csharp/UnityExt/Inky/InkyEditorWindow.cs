using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Darklight.UnityExt.Inky
{
    public class InkyEditorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("DarklightExt/InkyEditorWindow")]
        public static void ShowWindow()
        {
            GetWindow<InkyEditorWindow>();
        }

        private void OnEnable()
        {
            EditorApplication.update += UpdateGUI;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateGUI;
        }

        private void UpdateGUI()
        {
            Repaint();
        }

        private void OnGUI()
        {
            InkyStoryManager storyManager = InkyStoryManager.Instance;
            if (storyManager == null || InkyStoryManager.GlobalStoryObject == null)
            {
                EditorGUILayout.HelpBox("Inky Story Manager is not initialized.", MessageType.Warning);
                return;
            }

            // --------------------- [[ DISPLAY STORY MANAGER ]] --------------------- >>
            SerializedObject serializedStoryManager = new SerializedObject(InkyStoryManager.Instance);
            serializedStoryManager.Update();
            InkyStoryManager.Console.DrawInEditor();
            EditorGUILayout.PropertyField(serializedStoryManager.FindProperty("_globalStoryObject"));
            EditorGUILayout.PropertyField(serializedStoryManager.FindProperty("_currentSpeaker"));
            serializedStoryManager.ApplyModifiedProperties();

            // --------------------- [[ DISPLAY STORY OBJECT ]] --------------------- >>
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null || storyObject.StoryValue == null)
            {
                EditorGUILayout.HelpBox("Story Object is not initialized.", MessageType.Warning);
                if (GUILayout.Button("Initialize Story Object"))
                {
                    InkyStoryManager.GlobalStoryObject.Initialize();
                }
                return;
            }
            SerializedObject serializedStoryObject = new SerializedObject(storyObject);
            serializedStoryObject.Update();
            serializedStoryObject.ApplyModifiedProperties();



            // Fetch the global variables from the story
            EditorGUILayout.Space(10);
            GUILayout.Label("Variables", EditorStyles.boldLabel);
            List<InkyVariable> variables = InkyStoryObject.GetVariables(storyObject.StoryValue);
            List<string> knowledge = storyObject.GetVariableByName("GLOBAL_KNOWLEDGE").ToStringList();

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
