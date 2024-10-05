using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{

    [CreateAssetMenu(menuName = "Darklight/Grid2D/SpawnerDataObject")]
    public class Grid2D_SpawnerDataObject : ScriptableObject
    {
        [Header("Flags")]
        [SerializeField] bool _inheritCellWidth = true;
        [SerializeField] bool _inheritCellHeight = true;
        [SerializeField] bool _inheritCellNormal = true;

        [Header("Anchor Points")]
        [SerializeField] Spatial2D.AnchorPoint _default_OriginAnchorPoint = Spatial2D.AnchorPoint.CENTER;
        [SerializeField] Spatial2D.AnchorPoint _default_TargetAnchorPoint = Spatial2D.AnchorPoint.CENTER;

        [Header("Serialized Data")]
        /// <summary>
        /// Serialized Data modified by user in the inspector. This data is used to update the data map.
        /// </summary>
        [SerializeField, NonReorderable] List<Cell2D.SpawnerComponent.InternalData> _serializedCellSpawnData = new List<Cell2D.SpawnerComponent.InternalData>();

        public bool InheritCellWidth => _inheritCellWidth;
        public bool InheritCellHeight => _inheritCellHeight;
        public bool InheritCellNormal => _inheritCellNormal;
        public Spatial2D.AnchorPoint DefaultOriginAnchor => _default_OriginAnchorPoint;
        public Spatial2D.AnchorPoint DefaultTargetAnchor => _default_TargetAnchorPoint;
        public List<Cell2D.SpawnerComponent.InternalData> SerializedSpawnData { get => _serializedCellSpawnData; set => _serializedCellSpawnData = value; }
    }
}