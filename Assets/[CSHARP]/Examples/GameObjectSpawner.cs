using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameObjectSpawner : MonoBehaviour
{

    [SerializeField, ShowOnly] int _tempInt = 0;
    public List<GameObject> gameObjects = new List<GameObject>();

    public void Initialize()
    {
        _tempInt = Random.Range(0, 100);
    }

    public void SpawnCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "GAMEOBJECT_SPAWNER_CUBE";
        cube.transform.position = new Vector3(0, 0, 0);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameObjectSpawner))]
public class GameObjectSpawnerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    GameObjectSpawner _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (GameObjectSpawner)target;
        _script.Initialize();
    }

    // This is the method that is called when the inspector is drawn
    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        // Checking to see if any values have changed
        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI(); // Draw the default inspector

        List<GameObject> gameObjects = _script.gameObjects;
        if (GUILayout.Button("Spawn Cube"))
        {
            _script.SpawnCube();
        }

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }

    public void OnSceneGUI()
    {
        //Handles.Label(new Vector3(0, 0, 0), "This is the origin");
    }
}
#endif
