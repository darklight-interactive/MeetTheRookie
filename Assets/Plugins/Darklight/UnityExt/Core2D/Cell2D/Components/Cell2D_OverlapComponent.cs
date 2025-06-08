using Darklight.UnityExt.Editor;
using UnityEngine;
using System.Collections.Generic;

namespace Darklight.UnityExt.Core2D
{

    public partial class Cell2D
    {

        [System.Serializable]
        public class OverlapComponent : BaseComponent
        {
            // ======== [[ FIELDS ]] =========================== >>>>
            LayerMask _layerMask;
            Collider2D[] _colliders;
            HashSet<Collider2D> _currentColliders = new HashSet<Collider2D>();
            HashSet<Collider2D> _previousColliders = new HashSet<Collider2D>();


            // ======== [[ PROPERTIES ]] ================================== >>>>
            public LayerMask LayerMask { get => _layerMask; set => _layerMask = value; }
            public int ColliderCount => GetColliderCount();

            // ======== [[ EVENTS ]] ================================== >>>>
            public delegate void OverlapEvent(Cell2D cell, Collider2D collider);
            public OverlapEvent OnColliderEnter;
            public OverlapEvent OnColliderExit;

            // ======== [[ CONSTRUCTORS ]] =========================== >>>>
            public OverlapComponent(Cell2D cell) : base(cell) { }
            public OverlapComponent(Cell2D cell, LayerMask layerMask) : base(cell)
            {
                _layerMask = layerMask;
            }

            // ======== [[ INHERITED METHODS ]] ================================== >>>>
            public override void OnUpdate()
            {
                base.OnUpdate();
                UpdateColliders();
            }
            public override void DrawGizmos()
            {
                BaseCell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);
                GetColor(out Color color);
                int colliderCount = GetColliderCount();
#if UNITY_EDITOR
                // << DRAW LABEL >>
                string label = $"Overlap Colliders : {colliderCount}";
                Vector3 labelPosition = position + (new Vector3(-dimensions.x, dimensions.y, 0) * 0.5f);
                CustomGizmos.DrawLabel(label, labelPosition, new GUIStyle()
                {
                    fontSize = 12,
                    normal = new GUIStyleState() { textColor = Color.white }
                });

                // << DRAW OUTLINE >>
                CustomGizmos.DrawWireRect(position, dimensions, normal, color);

                // << DRAW SOLID IF OVERLAPED >>
                if (colliderCount > 0)
                {
                    Color alphaColor = new Color(color.r, color.g, color.b, 0.3f);
                    CustomGizmos.DrawSolidRect(position, dimensions, normal, alphaColor);
                }
#endif
            }

            public override void DrawSelectedGizmos() { }
            public override void DrawEditorGizmos() { }
            // ======== [[ PUBLIC METHODS ]] =========================== >>>>
            public int GetColliderCount()
            {
                if (_colliders == null) return 0;
                return _colliders.Length;
            }


            // ======== [[ PRIVATE METHODS ]] =========================== >>>>
            void UpdateColliders()
            {
                if (_previousColliders == null)
                {
                    _previousColliders = new HashSet<Collider2D>();
                }

                // Clear the currentColliders set to prepare for new detections
                _currentColliders = new HashSet<Collider2D>();


                BaseCell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);
                Vector3 halfExtents = new Vector3(dimensions.x * 0.5f, dimensions.y * 0.5f, 0);

                // Use Physics2D.OverlapBoxAll to detect colliders within the cell dimensions
                _colliders = Physics2D.OverlapBoxAll(position, halfExtents, 0, _layerMask);

                // Add detected colliders to the currentColliders set
                foreach (var collider in _colliders)
                {
                    _currentColliders.Add(collider);
                }

                // Detect colliders that have entered
                HashSet<Collider2D> enteredColliders = new HashSet<Collider2D>(_currentColliders);
                enteredColliders.ExceptWith(_previousColliders); // Elements in current but not in previous
                foreach (var collider in enteredColliders)
                {
                    OnColliderEnter?.Invoke(BaseCell, collider);
                }

                // Detect colliders that have exited
                HashSet<Collider2D> exitedColliders = new HashSet<Collider2D>(_previousColliders);
                exitedColliders.ExceptWith(_currentColliders); // Elements in previous but not in current
                foreach (var collider in exitedColliders)
                {
                    OnColliderExit?.Invoke(BaseCell, collider);
                }

                // Swap sets: previous becomes current, reuse the set
                (_currentColliders, _previousColliders) = (_previousColliders, _currentColliders);
            }

            void GetColor(out Color color)
            {
                if (_currentColliders == null || _currentColliders.Count == 0)
                {
                    color = Color.grey;
                    return;
                }

                color = Color.red;
            }

        }
    }
}
