using Darklight.UnityExt.Editor;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        [Serializable]
        public class WeightComponent : BaseComponent, IWeightedData
        {
            const int MIN_WEIGHT = 0;
            const int MAX_WEIGHT = 100;

            [SerializeField, ShowOnly] int _weight;
            public int Weight => _weight;

            // ======== [[ CONSTRUCTORS ]] =========================== >>>>
            public WeightComponent(Cell2D cell) : base(cell)
            {
                _weight = 0;
            }

            // ======== [[ METHODS ]] ================================== >>>>
            // -- (( INTERFACE METHODS )) -------- ))
            public override void OnUpdate() { }
            public override ComponentTypeKey GetTypeKey() => ComponentTypeKey.WEIGHT;

            public override void DrawGizmos()
            {
                BaseCell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);
                Color color = GetColor();
                color = new Color(color.r, color.g, color.b, 0.5f);

#if UNITY_EDITOR
                // << DRAW RECT >>
                Vector2 smallerDimensions = dimensions * 0.75f;
                CustomGizmos.DrawSolidRect(position, smallerDimensions, normal, color);
#endif
            }
            public override void DrawSelectedGizmos() { }

            public override void DrawEditorGizmos() { }

            // -- (( GETTERS )) -------- ))
            public int GetWeight()
            {
                return _weight;
            }

            Color GetColor()
            {
                return Color.Lerp(Color.black, Color.white, (float)_weight / MAX_WEIGHT);
            }

            Color GetInverseColor()
            {
                return Color.Lerp(Color.white, Color.black, (float)_weight / MAX_WEIGHT);
            }

            // -- (( SETTERS )) -------- ))
            public void SetWeight(int weight)
            {
                _weight = weight;
            }

            public void SetRandomWeight()
            {
                _weight = UnityEngine.Random.Range(MIN_WEIGHT, MAX_WEIGHT);
            }

            // -- (( HANDLER METHODS )) -------- ))
            public void AddWeight(int amount)
            {
                _weight += amount;
            }

            public void SubtractWeight(int amount)
            {
                _weight -= amount;
            }
        }
    }
}