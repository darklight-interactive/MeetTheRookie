using System;
using System.Collections.Generic;

using Darklight.UnityExt.Editor;

using UnityEngine;
using System.Linq;


namespace Darklight.UnityExt.Game.Grid
{

    partial class Cell2D
    {
        [Serializable]
        public class SerializedData
        {
            // ======== [[ SERIALIZED FIELDS ]] ======================================================= >>>>
            [SerializeField, ShowOnly] int _guid = Guid.NewGuid().GetHashCode();
            [SerializeField, ShowOnly] Vector2Int _key = Vector2Int.zero;
            [SerializeField, ShowOnly] Vector2Int _coordinate = Vector2Int.zero;
            [SerializeField, ShowOnly] Vector2 _dimensions = Vector2.one;
            [SerializeField, ShowOnly] Vector3 _position = Vector3.zero;
            [SerializeField, ShowOnly] Vector3 _normal = Vector3.up;
            [SerializeField, ShowOnly] bool _isDisabled = false;

            // ======== [[ PROPERTIES ]] ======================================================= >>>>
            public Vector2Int Key { get => _key; }
            public Vector2Int Coordinate { get => _coordinate; }
            public Vector2 Dimensions { get => _dimensions; }
            public Vector3 Position { get => _position; }
            public Vector3 Normal { get => _normal; }
            public bool Disabled { get => _isDisabled; }

            // ======== [[ CONSTRUCTORS ]] ======================================================= >>>>
            public SerializedData()
            {
                _key = Vector2Int.zero;
                _coordinate = Vector2Int.zero;
                _dimensions = Vector2.one;
                _position = Vector3.zero;
                _normal = Vector3.up;
                _isDisabled = false;
            }

            public SerializedData(Vector2Int key)
            {
                _key = key;
                _coordinate = key;
                _dimensions = Vector2.one;
                _position = Vector3.zero;
                _normal = Vector3.up;
                _isDisabled = false;
            }

            public SerializedData(SerializedData originData)
            {
                _key = originData._key;
                _coordinate = originData._coordinate;
                _dimensions = originData._dimensions;
                _position = originData._position;
                _normal = originData._normal;
                _isDisabled = originData._isDisabled;
            }

            // ======== [[ METHODS ]] ============================================================ >>>>


            // (( GETTERS )) -------- )))


            // (( SETTERS )) -------- ))
            public void SetCoordinate(Vector2Int coordinate) => _coordinate = coordinate;
            public void SetPosition(Vector3 position) => _position = position;
            public void SetNormal(Vector3 normal) => _normal = normal;
            public void SetDimensions(Vector2 dimensions) => _dimensions = dimensions;
            public void SetDisabled(bool disabled) => _isDisabled = disabled;
            public virtual void CopyFrom(SerializedData data)
            {
                if (data == null)
                {
                    Debug.LogError("Cannot copy data from null object.");
                    return;
                }

                _key = data.Key;
                _coordinate = data.Coordinate;
                _dimensions = data.Dimensions;
                _position = data.Position;
                _normal = data.Normal;
                _isDisabled = data.Disabled;
            }




        }
    }
}