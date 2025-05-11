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
    [HorizontalLine(color: EColor.Gray)]
    [Dropdown("dropdown_knotList"), SerializeField]
    string _sceneKnot = "scene_default";

    [Dropdown("dropdown_interactionStitchList"), SerializeField]
    string _interactionStitch = "interaction_default";

    [SerializeField]
    Sprite _sprite;

    protected List<string> dropdown_knotList
    {
        get
        {
            List<string> knots = new List<string>(100);
            if (MTRStoryManager.Instance != null)
            {
                knots = MTRStoryManager.Instance.KnotList;
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

    public string SceneKnot => _sceneKnot;
    public string InteractionStitch => _interactionStitch;
    public Sprite Sprite => _sprite;

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

        return uniqueName;
    }

    /// <summary>
    /// Sets the name of the ScriptableObject to the interaction stitch.
    /// </summary>
    public void Initialize(
        string sceneKnot = "scene_default",
        string interactionStitch = "interaction_default",
        Sprite sprite = null
    )
    {
        if (sceneKnot != "scene_default")
            _sceneKnot = sceneKnot;

        if (interactionStitch != "interaction_default")
            _interactionStitch = interactionStitch;

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
            string uniqueName = GenerateUniqueAssetName(_interactionStitch, directory);

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
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                _script.Initialize();
            }
        }
    }
#endif
}
