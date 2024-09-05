using System;
using Darklight.UnityExt.Utility;
using UnityEngine;

namespace Darklight.UnityExt.Game.Grid
{
    public class Cell2D_SpawnerComponent : Cell2D.Component
    {
        GameObject _spawnedObject;

        public bool ObjectSpawned => _spawnedObject != null;

        public Cell2D_SpawnerComponent(Cell2D baseObj) : base(baseObj) { }

        public void SpawnObject(Func<GameObject, GameObject> initializer)
        {
            if (_spawnedObject != null) return;
            BaseCell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);

            _spawnedObject = new GameObject("SpawnedObject");
            _spawnedObject.transform.position = position;
            _spawnedObject.transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
            _spawnedObject.transform.rotation = Quaternion.LookRotation(normal, Vector3.up);

            _spawnedObject = initializer(_spawnedObject);
        }

        public void DestroyObject()
        {
            if (_spawnedObject == null) return;
            GameObjectUtility.DestroyAlways(_spawnedObject);
            _spawnedObject = null;
        }
    }
}