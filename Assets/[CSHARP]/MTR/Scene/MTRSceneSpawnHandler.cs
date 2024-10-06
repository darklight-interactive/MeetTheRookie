using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTRSceneSpawnHandler : MonoBehaviourSingleton<MTRSceneSpawnHandler>
{
    public static MTRSceneManager GameSceneManager => MTRSceneManager.Instance;

    Scene _activeScene;
    Dictionary<string, SceneSpawnInfo> _sceneSpawnInfoDict = new Dictionary<string, SceneSpawnInfo>();

    private GameObject Lupe;
    private GameObject Misra;

    [SerializeField, ShowOnly] string _activeSceneName;
    [SerializeField] SceneSpawnInfo _activeSceneInfo;

    void RefreshActiveSceneInfo()
    {
        _activeScene = SceneManager.GetActiveScene();
        _activeSceneName = _activeScene.name;

        if (_sceneSpawnInfoDict.ContainsKey(_activeSceneName))
        {
            _activeSceneInfo = _sceneSpawnInfoDict[_activeSceneName];
        }
        else
        {
            _activeSceneInfo = new SceneSpawnInfo(_activeSceneName);
            _sceneSpawnInfoDict[_activeSceneName] = _activeSceneInfo;
        }
    }

    public override void Initialize()
    {
        GameSceneManager.OnSceneChanged += SceneChanged;
        RefreshActiveSceneInfo();
    }

    private void SceneChanged(Scene oldScene, Scene newScene)
    {
        RefreshActiveSceneInfo();

        List<GameObject> interactableObjects = GetAllInteractables();

        /*
        foreach (GameObject interactable in interactableObjects)
        {
            // ensure the interactable knows where its DestinationPoints are
            interactable.GetComponent<Interactable>().FindDestinationPoints();
        }
        */

        if (!_sceneSpawnInfoDict.ContainsKey(_activeScene.name))
        {
            SceneSpawnInfo sceneData = new SceneSpawnInfo(_activeScene.name);
            _sceneSpawnInfoDict[_activeScene.name] = sceneData;
            SetSpawnPoints(interactableObjects);
        }

        // Change locations of Lupe and Misra to Spawn Points
        SceneSpawnInfo sceneInfo = _sceneSpawnInfoDict[_activeScene.name];

        var tempLupe = FindFirstObjectByType<MTRPlayerInput>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        MTR_Misra_Controller tempMisra = FindFirstObjectByType<MTR_Misra_Controller>();
        if (tempMisra != null)
        {
            Misra = tempMisra.gameObject;
        }

        /*
        if (Lupe != null && Lupe.GetComponent<SpriteRenderer>().enabled)
        {
            if (sceneInfo.spawnPoints.Count == 0)
            {
                Debug.LogError("Cannot spawn Lupe. No Spawn Points", this);
                return;
            }
            Lupe.transform.position = new Vector3(sceneInfo._spawnPoints[0], Lupe.transform.position.y, Lupe.transform.position.z);
        }

        if (Misra != null && Misra.GetComponent<SpriteRenderer>().enabled)
        {
            if (sceneInfo.spawnPoints.Count <= 1)
            {
                Debug.LogError("Cannot spawn Misra. No available Spawn Points");
                return;
            }
            Misra.transform.position = new Vector3(sceneInfo._spawnPoints[1], Misra.transform.position.y, Misra.transform.position.z);
        }
        */
    }

    public void SetSpawnPoints(List<GameObject> interactables)
    {
        _sceneSpawnInfoDict[_activeScene.name].SetSpawnPoints(interactables);
    }

    public List<GameObject> GetAllInteractables()
    {
        BaseInteractable[] sceneInteractables = FindObjectsByType<BaseInteractable>(FindObjectsSortMode.None);
        List<GameObject> interactableObjects = new List<GameObject>();

        foreach (var interactable in sceneInteractables)
        {
            interactableObjects.Add(interactable.gameObject);
        }

        return interactableObjects;
    }

    void OnDrawGizmosSelected()
    {
        if (_activeSceneInfo != null)
        {
            _activeSceneInfo.DrawInScene();
        }
    }

    [Serializable]
    public class SceneSpawnInfo
    {
        [SerializeField, ShowOnly] string _sceneName;
        [SerializeField, NonReorderable] List<float> _spawnPoints = new List<float>();

        public SceneSpawnInfo(string sceneName)
        {
            this._sceneName = sceneName;
        }

        public void SetSpawnPoints(List<GameObject> interactables)
        {
            _spawnPoints.Clear();

            _spawnPoints.AddRange(FindSpawnPoints(interactables));
        }

        public void DrawInScene()
        {
            for (int i = 0; i < _spawnPoints.Count; i++)
            {
                DrawSpawnPoint(i, _spawnPoints[i]);
            }
        }

        List<float> FindSpawnPoints(List<GameObject> interactables)
        {
            List<float> spawnPoints = new List<float>();

            foreach (var interactable in interactables)
            {
                BaseInteractable script = interactable.GetComponent<BaseInteractable>();

                /*
                if (!script.isSpawn) { continue; }

                List<GameObject> destinationPoints = script.GetDestinationPoints();
                foreach (var destinationPoint in destinationPoints)
                {
                    spawnPoints.Add(destinationPoint.transform.position.x);
                }
                */
            }
            return spawnPoints;
        }

        void DrawSpawnPoint(int index, float x_value)
        {
            // Draw a line at the spawn point
            Vector3 start = new Vector3(x_value, 0, 0);
            Vector3 end = new Vector3(x_value, 10, 0);


            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(start, 0.05f);
            CustomGizmos.DrawLabel($"SpawnPoint{index}", start + (Vector3.up * 0.1f), new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    textColor = Color.yellow
                }
            });
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRSceneSpawnHandler))]
    public class MTRSpawnHandlerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRSceneSpawnHandler _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRSceneSpawnHandler)target;
            _script.Initialize();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}

