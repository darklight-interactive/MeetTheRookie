using System;
using Darklight.UnityExt.Utility;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        public class SpawnerComponent : BaseComponent
        {
            /* 
            1. Spawn object at this cell position
                - optionally have object inherit any of these Cell2D properties: 
                    - normal
                    - width
                    - height
                    - parent
            */
            public void SpawnObject(GameObject prefab, bool inheritNormal = false, bool inheritWidth = false, bool inheritHeight = false)
            {
                GameObject instance = GameObject.Instantiate(prefab, BaseCell.Data.Position, Quaternion.identity);

                if (inheritNormal)
                    instance.transform.up = BaseCell.Data.Normal;

                if (inheritWidth || inheritHeight)
                {
                    Vector3 scale = instance.transform.localScale;
                    if (inheritWidth)
                        scale.x = BaseCell.Data.Dimensions.x;
                    if (inheritHeight)
                        scale.y = BaseCell.Data.Dimensions.y;
                    instance.transform.localScale = scale;
                }
            }


            /*
            2. Expand upon the base spawn function to allow the object to be spawned with a specified origin point
                - e.g. spawn the object so that the bottom left corner of the object is at the cell position
                - use a enum to specify the origin point
            */



            public SpawnerComponent(Cell2D cell) : base(cell) { }
        }
    }
}