using System;
using System.Collections.Generic;
using System.IO;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(
    fileName = "New Interactable Data",
    menuName = "MeetTheRookie/MTR_InteractableData"
)]
[Serializable]
public class MTRInteractableDataSO : ScriptableObject
{
    [SerializeField, ShowOnly]
    string _internalSceneKnot = "";

    [SerializeField, ShowOnly]
    string _internalInteractionStitch = "";

    [Tooltip("The knot to read from when the interactable is interacted with")]
    [Dropdown("dropdown_knotList"), SerializeField, HideIf("InternalSceneKnotSet")]
    string _sceneKnot = "scene_default";

    [Tooltip("The stitch to read from when the interactable is interacted with")]
    [
        Dropdown("dropdown_interactionStitchList"),
        SerializeField,
        HideIf("InternalInteractionStitchSet")
    ]
    string _interactionStitch = "interaction_default";

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField, ShowOnly]
    string _internalClue = "None";

    [SerializeField, AllowNesting, HideIf("InternalMysterySet")]
    MTRMystery _mystery = MTRMystery.UNKNOWN;

    [Dropdown("dropdown_clueList"), SerializeField, HideIf("InternalClueSet")]
    string _clue = "";

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField]
    bool _isSpawnPoint = false;

    [SerializeField, EnableIf("IsSpawnPoint")]
    [Range(0, 3)]
    int _spawnIndex = 0;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField]
    Sprite _sprite;

    protected List<string> dropdown_knotList
    {
        get
        {
            List<string> knots = new List<string>(100);
            if (MTRStoryManager.Instance != null)
            {
                knots = MTRStoryManager.Instance.SceneKnotList;
            }
            return knots;
        }
    }
    List<string> dropdown_interactionStitchList
    {
        get
        {
            List<string> stitches = new List<string>(100);
            if (MTRStoryManager.Instance != null)
            {
                stitches = MTRStoryManager.GetAllStitchesInKnot(_sceneKnot);
            }
            return stitches;
        }
    }

    List<string> dropdown_clueList
    {
        get
        {
            List<string> clues = new List<string>(100);
            if (MTRStoryManager.Instance != null)
            {
                clues = MTRStoryManager.GetClueList(_mystery);
                if (clues.Count == 0)
                    clues = new List<string>(100) { "None" };
            }
            return clues;
        }
    }

    public bool InternalSceneKnotSet => _internalSceneKnot != "";
    public bool InternalInteractionStitchSet => _internalInteractionStitch != "";
    public bool InternalMysterySet => _mystery != MTRMystery.UNKNOWN;
    public bool InternalClueSet => _internalClue != "" && _internalClue != "None";
    public bool IsSpawnPoint => _isSpawnPoint;
    public int SpawnIndex => _spawnIndex;

    public string SceneKnot
    {
        get
        {
            if (_internalSceneKnot == "" && _sceneKnot != "scene_default")
                _internalSceneKnot = _sceneKnot;
            return _internalSceneKnot;
        }
    }
    public string InteractionStitch
    {
        get
        {
            if (_internalInteractionStitch == "" && _interactionStitch != "interaction_default")
                _internalInteractionStitch = _interactionStitch;
            return _internalInteractionStitch;
        }
    }
    public string Mystery => _mystery.ToString();
    public string ClueTag
    {
        get
        {
            if (_internalClue == "" && _clue != "" && _clue != "None")
                _internalClue = _clue;
            return _internalClue;
        }
    }
    public Sprite Sprite => _sprite;

    public string Name
    {
        // Get the last part of the interaction stitch
        get
        {
            string[] nameParts = InteractionStitch.Split('.');
            string lastPart = nameParts[nameParts.Length - 1];
            return lastPart.Replace("_", " "); // Replace underscores with spaces
        }
    }

    /// <summary>
    /// Generates a unique asset name by appending an incremental number if the base name already exists
    /// </summary>
    /// <param name="baseName">The base name to check</param>
    /// <param name="directory">The directory to check for existing assets</param>
    /// <returns>A unique asset name</returns>
    private string GenerateUniqueAssetName(string baseName, string directory)
    {
        string uniqueName = baseName;
        int counter = 1;

#if UNITY_EDITOR
        // If the original name is taken by this asset, return the original name
        string currentPath = AssetDatabase.GetAssetPath(this);
        string basePath = Path.Combine(directory, $"{uniqueName}.asset").Replace('\\', '/');
        if (currentPath == basePath)
        {
            return uniqueName;
        }

        // If the original name is taken by another asset, generate a unique name
        while (
            AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                Path.Combine(directory, $"{uniqueName}.asset")
            ) != null
        )
        {
            uniqueName = $"{baseName}_{counter}";
            counter++;

            if (counter > 10)
            {
                Debug.LogError($"Failed to generate a unique asset name for {baseName}");
                return baseName;
            }
        }
#endif
        return uniqueName;
    }

    /// <summary>
    /// Sets the name of the ScriptableObject to the interaction stitch.
    /// </summary>
    [Button]
    public void Initialize(
        string sceneKnot = "scene_default",
        string interactionStitch = "interaction_default",
        Sprite sprite = null
    )
    {
        if (sceneKnot != "scene_default")
            _sceneKnot = sceneKnot;
        if (_sceneKnot != "scene_default")
            _internalSceneKnot = _sceneKnot;

        if (interactionStitch != "interaction_default")
            _interactionStitch = interactionStitch;
        if (_interactionStitch != "interaction_default")
            _internalInteractionStitch = _interactionStitch;

        if (_mystery != MTRMystery.UNKNOWN && _clue != "" && _clue != "None")
            _internalClue = _clue;
        else
            _internalClue = "None";

        if (sprite != null)
            _sprite = sprite;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        // Get the current asset path
        string assetPath = AssetDatabase.GetAssetPath(this);
        if (!string.IsNullOrEmpty(assetPath))
        {
            // Get the directory and extension
            string directory = Path.GetDirectoryName(assetPath);
            string extension = Path.GetExtension(assetPath);

            // Generate a unique name if needed
            string uniqueName = GenerateUniqueAssetName(InteractionStitch, directory);

            // Create new path with the updated name
            string newPath = Path.Combine(directory, $"{uniqueName}{extension}");

            // Rename the asset if the path is different
            if (newPath != assetPath)
            {
                AssetDatabase.RenameAsset(assetPath, uniqueName);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }

    [Button]
    public void ResetStitch()
    {
        _internalSceneKnot = "";
        _internalInteractionStitch = "";
    }

    [Button]
    public void ResetClue()
    {
        _mystery = MTRMystery.UNKNOWN;
        _internalClue = "";
    }

    /*
    #if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(MTRInteractableDataSO))]
        public class MTRInteractableDataSOCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            MTRInteractableDataSO _script;
    
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (MTRInteractableDataSO)target;
                _script.Initialize();
            }
    
                    public override void OnInspectorGUI()
                    {
                        _serializedObject.Update();
            
                        EditorGUI.BeginChangeCheck();
            
                        base.OnInspectorGUI();
            
                        if (GUILayout.Button("Initialize"))
                        {
                            _script.Initialize();
                        }
            
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Reset Stitch"))
                        {
                            _script.ResetStitch();
                        }
            
                        if (GUILayout.Button("Reset Clue"))
                        {
                            _script.ResetClue();
                        }
                        EditorGUILayout.EndHorizontal();
            
                        if (EditorGUI.EndChangeCheck())
                        {
                            _serializedObject.ApplyModifiedProperties();
                            //_script.Initialize();
                        }
                    }
        }
    #endif
            */
}
