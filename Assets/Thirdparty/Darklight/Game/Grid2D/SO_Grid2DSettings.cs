using System;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using Darklight.UnityExt;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Darklight/Grid2DSettings")]
[CanEditMultipleObjects]
public class SO_Grid2DSettings : ScriptableObject
{
    const int MIN = 1;
    const int MAX = 10;
    [Range(MIN, MAX)] public int gridSizeX = 3;
    [Range(MIN, MAX)] public int gridSizeY = 3;
    [Range(0.1f, 1f)] public float coordinateSize = 1;
    [Range(-MAX, MAX)] public int originKeyX = 1;
    [Range(-MAX, MAX)] public int originKeyY = 1;

    public static void DisplayGrid2D(Grid2D<IGrid2DData> grid2D)
    {
        if (grid2D == null)
        {
            Debug.LogError("Grid2D is null");
            return;
        }
        List<Vector2Int> positionKeys = grid2D.GetPositionKeys();
        if (positionKeys != null && positionKeys.Count > 0)
        {
            foreach (Vector2Int vector2Int in positionKeys)
            {
                IGrid2DData data = grid2D.GetData(vector2Int);
                if (data == null)
                {
                    Debug.LogError("Data is null");
                    continue;
                }
                //float weightValue = overlapData.weight;
                float size = grid2D.settings.coordinateSize;
                Vector3 direction = Vector3.forward;


                CustomGizmos.DrawLabel(data.label, data.worldPosition, CustomGUIStyles.BoldStyle);
                CustomGizmos.DrawWireSquare(data.worldPosition, size, direction, data.activeColor);
                CustomGizmos.DrawButtonHandle(data.worldPosition, size * 0.75f, direction, data.activeColor, () =>
                {

                }, Handles.RectangleHandleCap);
            }
        }
    }

}