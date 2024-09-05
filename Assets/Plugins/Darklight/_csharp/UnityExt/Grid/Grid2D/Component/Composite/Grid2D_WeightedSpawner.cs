using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UnityExt.Game.Grid
{
    [RequireComponent(typeof(Grid2D), typeof(Grid2D_SpawnerComponent), typeof(Grid2D_WeightComponent))]
    public class Grid2D_WeightedSpawner : Grid2D_BaseComponent
    {
        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( COMPONENTS )) -------- ))
        Grid2D_SpawnerComponent _spawnerComponent;
        Grid2D_WeightComponent _weightComponent;

        // ======== [[ METHODS ]] ================================== >>>>
        // -- (( INTERFACE METHODS )) -------- ))
        public override void OnInitialize(Grid2D baseObj)
        {
            base.OnInitialize(baseObj);

            _spawnerComponent = baseObj.GetComponent<Grid2D_SpawnerComponent>();
            if (_spawnerComponent == null)
                _spawnerComponent = baseObj.gameObject.AddComponent<Grid2D_SpawnerComponent>();

            _weightComponent = baseObj.GetComponent<Grid2D_WeightComponent>();
            if (_weightComponent == null)
                _weightComponent = baseObj.gameObject.AddComponent<Grid2D_WeightComponent>();
        }

        public void SpawnAtRandomWeightedCell()
        {
            Cell2D cell = _weightComponent.GetRandomCellByWeight();
            Cell2D_SpawnerComponent cellSpawnerComponent = cell.ComponentReg.GetComponent<Cell2D_SpawnerComponent>();
            if (cellSpawnerComponent != null)
            {
                if (cellSpawnerComponent.ObjectSpawned)
                {
                    Debug.LogError("Object already spawned at cell");
                    return;
                }

                _spawnerComponent.SpawnObjectAtCell(cell);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Grid2D_WeightedSpawner))]
        public class Grid2D_WeightedSpawnerCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            Grid2D_WeightedSpawner _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (Grid2D_WeightedSpawner)target;
                _script.Awake();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (GUILayout.Button("Spawn At Random Weighted Cell"))
                {
                    _script.SpawnAtRandomWeightedCell();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif

    }
}
