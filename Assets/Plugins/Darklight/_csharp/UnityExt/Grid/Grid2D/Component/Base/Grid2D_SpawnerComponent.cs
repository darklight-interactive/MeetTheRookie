using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Game.Grid
{
    public class Grid2D_SpawnerComponent : Grid2D_BaseComponent
    {
        // ======== [[ FIELDS ]] ======================================================= >>>>
        [SerializeField] GameObject _objectToSpawn;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( BASE VISITORS )) -------- ))


        // -- (( CUSTOM VISITORS )) -------- ))
        private Cell2D.ComponentVisitor _spawnVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (ComponentTypeKey.SPAWNER, (Cell2D cell, ComponentTypeKey type) =>
            {
                // -- (( GET SPAWNER COMPONENT )) -------- ))
                Cell2D_SpawnerComponent spawnerComponent = cell.ComponentReg.GetComponent<Cell2D_SpawnerComponent>();
                if (spawnerComponent == null) return false;

                // -- (( SPAWN OBJECT )) -------- ))
                cell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);
                spawnerComponent.InstantiateObject(_objectToSpawn);

                return true;
            });
        private Cell2D.ComponentVisitor _destroyVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (ComponentTypeKey.SPAWNER, (Cell2D cell, ComponentTypeKey type) =>
            {
                // -- (( GET SPAWNER COMPONENT )) -------- ))
                Cell2D_SpawnerComponent spawnerComponent = cell.ComponentReg.GetComponent<Cell2D_SpawnerComponent>();
                if (spawnerComponent == null) return false;

                // -- (( DESTROY OBJECT )) -------- ))
                spawnerComponent.DestroyObject();
                return true;
            });

        // ======== [[ METHODS ]] ================================== >>>>
        public void SpawnObjectAtAllCells()
        {
            BaseGrid.SendVisitorToAllCells(_spawnVisitor);
        }

        public void SpawnObjectAtCell(Cell2D cell)
        {
            _spawnVisitor.Visit(cell);
        }

        public void DestroyObject()
        {
            BaseGrid.SendVisitorToAllCells(_destroyVisitor);
        }

        public bool SpawnersAvailable()
        {
            List<Cell2D_SpawnerComponent> spawners = BaseGrid.GetComponentsByType<Cell2D_SpawnerComponent>();
            foreach (Cell2D_SpawnerComponent spawner in spawners)
            {
                if (!spawner.ObjectSpawned)
                {
                    return true;
                }
            }
            return false;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Grid2D_SpawnerComponent))]
        public class Grid2D_SpawnerComponentCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            Grid2D_SpawnerComponent _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (Grid2D_SpawnerComponent)target;
                _script.Awake();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                /*
                if (GUILayout.Button("Spawn Object At All Cells"))
                {
                    _script.SpawnObjectAtAllCells();
                }
                if (GUILayout.Button("Destroy All Objects"))
                {
                    _script.DestroyObject();
                }
                */

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif

    }
}