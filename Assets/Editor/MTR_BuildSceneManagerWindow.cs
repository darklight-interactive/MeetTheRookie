using UnityEditor;
using UnityEngine;

public class MTR_SceneDataObjectEditorWindow : EditorWindow
{
    private MTR_SceneDataObject sceneDataObject;
    private Vector2 scrollPos;

    [MenuItem("Window/MTR Scene Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<MTR_SceneDataObjectEditorWindow>("MTR Scene Data Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("MTR Scene Data Editor", EditorStyles.boldLabel);

        sceneDataObject = (MTR_SceneDataObject)EditorGUILayout.ObjectField("Scene Data Object", sceneDataObject, typeof(MTR_SceneDataObject), false);

        if (sceneDataObject != null)
        {
            if (GUILayout.Button("Initialize"))
            {
                string[] buildScenePaths = GetBuildScenePaths();
                sceneDataObject.Initialize(buildScenePaths);
                EditorUtility.SetDirty(sceneDataObject);
            }

            GUILayout.Space(10);



            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(sceneDataObject);
                AssetDatabase.SaveAssets();
            }
        }
    }

    private string[] GetBuildScenePaths()
    {
        var scenes = EditorBuildSettings.scenes;
        string[] scenePaths = new string[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenePaths[i] = scenes[i].path;
        }
        return scenePaths;
    }
}
