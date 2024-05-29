using System;
using System.Collections.Generic;

using UnityEngine;

using Darklight.UnityExt;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Scene;

using Ink.Runtime;
using Darklight.Console;
using UnityEngine.SceneManagement;



#if UNITY_EDITOR
using UnityEditor;
#endif




/// <summary>
///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data. 
/// </summary>
[RequireComponent(typeof(InkyStoryLoader))]
public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>
{


    public InkyStoryLoader loader => GetComponent<InkyStoryLoader>();
    public List<string> storyKnots => InkyStoryObject.GetAllKnots(_globalStoryObject.StoryValue);

    [Tooltip("The global Inky Story Object.")]
    [SerializeField] InkyStoryObject _globalStoryObject;
    public InkyStoryObject GlobalStoryObject => _globalStoryObject;

    private InkyStoryIterator _storyIterator;
    public InkyStoryIterator Iterator
    {
        get
        {
            if (_storyIterator == null)
            {
                _storyIterator = new InkyStoryIterator(_globalStoryObject);
            }
            return _storyIterator;
        }
    }

#region ---------------- [[ SCENE DATA ]] ---------------- >>
    [System.Serializable]
    public class InkySceneData
    {
        public SceneObject sceneObject;

        [Dropdown("storyKnots")]
        public string knotName;
    }

    [HideInInspector] public List<InkySceneData> sceneData = new List<InkySceneData>();
    public string GetSceneKnot(string sceneName)
    {
        foreach (InkySceneData data in sceneData)
        {
            if (data.sceneObject == sceneName)
            {
                return data.knotName;
            }
        }
        return null;
    }
#endregion

    #region ----- [[ SPEAKER HANDLING ]] ------------------------ >>
    /// <summary>
    /// List of all the speakers in the Inky Story.
    /// </summary>
    public static List<string> SpeakerList;

    [Tooltip("The current speaker in the story.")]
    [ShowOnly, SerializeField] string _currentSpeaker;
    public string CurrentSpeaker => _currentSpeaker;

    public delegate void SpeakerSet(string speaker);
    public event SpeakerSet OnSpeakerSet;

    /// <summary>
    /// This is the external function that is called from the Ink story to set the current speaker.
    /// </summary>
    public object SetSpeaker(object[] args)
    {
        string speaker = (string)args[0];
        _currentSpeaker = speaker;
        OnSpeakerSet?.Invoke(speaker);
        Debug.Log($"{Prefix} >> Set Speaker: {speaker}");
        return false;
    }

    /// <summary>
    /// This is the forceful way to set the speaker value.
    /// </summary>
    /// <param name="speaker"></param>
    public void SetSpeaker(string speaker)
    {
        _currentSpeaker = speaker;
        OnSpeakerSet?.Invoke(speaker);
    }

    #endregion


    // ------------------------ [[ METHODS ]] ------------------------ >>

    public override void Initialize()
    {
        if (_globalStoryObject == null) return;
        _globalStoryObject.Initialize(); // << INITIALIZE STORY DATA >>
        _storyIterator = new InkyStoryIterator(_globalStoryObject);

        // << GET VARIABLES >>
        SpeakerList = _globalStoryObject.GetVariableByName("Speaker").ToStringList();
        Debug.Log($"{Prefix} >> Speaker List Count : {SpeakerList.Count}");

        // << LOAD SCENE DATA >>
        string currentSceneName = SceneManager.GetActiveScene().name;
        Iterator.GoToKnotOrStitch(GetSceneKnot(currentSceneName));

        // << BINDING FUNCTIONS >>
        _globalStoryObject.BindExternalFunction("QuestStarted", QuestStarted);
        _globalStoryObject.BindExternalFunction("GoToScene", GoToScene);
        _globalStoryObject.BindExternalFunction("SetSpeaker", SetSpeaker);

        // << OBSERVE VARIABLES >>
        _globalStoryObject.StoryValue.ObserveVariable("CURRENT_SPEAKER", (string varName, object newValue) =>
        {
            if (_currentSpeaker == newValue.ToString()) return;
            _currentSpeaker = newValue.ToString();
            OnSpeakerSet?.Invoke(_currentSpeaker);
            Debug.Log($"{Prefix} >> Current Speaker: {_currentSpeaker}");
        });

        _globalStoryObject.StoryValue.ObserveVariable("ACTIVE_QUEST_CHAIN", (string varName, object newValue) =>
        {
            Debug.Log($"[InkyStoryManager] >> Active Quest Chain: {newValue}");
        });
    }

    object GoToScene(object[] args)
    {
        Debug.Log("Go To Scene! >> " + args[0].ToString());
        return false;
    }

    object QuestStarted(object[] args)
    {
        Debug.Log("Quest Started! >> " + args[0].ToString());
        return false;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(InkyStoryManager))]
public class InkyStoryManagerCustomEditor : Editor
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
        if (GUILayout.Button("Initialize InkyStoryObjects"))
        {
            _script.loader.LoadAllStories();
            _script.Awake(); // << InitializeSingleton
        }

        if (GUILayout.Button("Show Editor Window"))
        {
            InkyStoryManagerEditorWindow.ShowWindow();
        }

        base.OnInspectorGUI();
    }
}

public class InkyStoryManagerEditorWindow : EditorWindow
{
    private InkyStoryManager storyManager;
    private SerializedObject serializedStoryManager;
    private SerializedProperty sceneDataProperty;

    [MenuItem("Window/Inky Story Manager")]
    public static void ShowWindow()
    {
        GetWindow<InkyStoryManagerEditorWindow>("Inky Story Manager");
    }

    private void OnEnable()
    {
        storyManager = FindFirstObjectByType<InkyStoryManager>();
        if (storyManager != null)
        {
            serializedStoryManager = new SerializedObject(storyManager);
            sceneDataProperty = serializedStoryManager.FindProperty("sceneData");
        }
    }

    private void OnGUI()
    {
        if (storyManager == null)
        {
            EditorGUILayout.HelpBox("InkyStoryManager not found in the scene.", MessageType.Warning);
            if (GUILayout.Button("Retry"))
            {
                storyManager = FindFirstObjectByType<InkyStoryManager>();
                if (storyManager != null)
                {
                    serializedStoryManager = new SerializedObject(storyManager);
                }
            }
            return;
        }

        serializedStoryManager.Update();

        // << GET PROPERTIES >>
        SerializedProperty globalStoryObjectProperty = serializedStoryManager.FindProperty("_globalStoryObject");
        sceneDataProperty = serializedStoryManager.FindProperty("sceneData");

        // << DRAW CONSOLE >>>
        ConsoleGUI consoleGUI = InkyStoryManager.Console;
        consoleGUI.DrawInEditor();

        // << DRAW STORY MANAGER >>
        EditorGUILayout.LabelField("Inky Story Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // >> Global Story Object
        EditorGUILayout.PropertyField(globalStoryObjectProperty);

        // >> Scene Data
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scene Data", EditorStyles.boldLabel);

        for (int i = 0; i < sceneDataProperty.arraySize; i++)
        {
            SerializedProperty dataProperty = sceneDataProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(dataProperty.FindPropertyRelative("sceneObject"));
            EditorGUILayout.PropertyField(dataProperty.FindPropertyRelative("knotName"));
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Add Scene Knot"))
        {
            sceneDataProperty.InsertArrayElementAtIndex(sceneDataProperty.arraySize);
        }

        if (GUILayout.Button("Remove Last Scene Knot"))
        {
            if (sceneDataProperty.arraySize > 0)
            {
                sceneDataProperty.DeleteArrayElementAtIndex(sceneDataProperty.arraySize - 1);
            }
        }

        serializedStoryManager.ApplyModifiedProperties();

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(storyManager);
        }
    }
}
#endif
