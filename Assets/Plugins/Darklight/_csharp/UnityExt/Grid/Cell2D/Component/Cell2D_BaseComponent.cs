

using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;
using UnityEngine;

namespace Darklight.UnityExt.Game.Grid
{


    public class Cell2D_BaseComponent : Cell2D.Component
    {
        public Cell2D_BaseComponent(Cell2D cell) : base(cell) { }
        public override void OnUpdate() { }
        public override void DrawGizmos()
        {

        }
        public override void DrawSelectedGizmos()
        {
            BaseCell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);

            Color faintWhite = new Color(1, 1, 1, 0.5f);
            CustomGizmos.DrawWireRect(position, dimensions, normal, faintWhite);
        }

        public override void DrawEditorGizmos() { }
        public override ComponentTypeKey GetTypeKey() => ComponentTypeKey.BASE;
    }
}