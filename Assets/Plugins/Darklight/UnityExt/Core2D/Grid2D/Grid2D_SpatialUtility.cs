using UnityEngine;
namespace Darklight.UnityExt.Core2D
{

    public partial class Grid2D
    {
        public static class SpatialUtility
        {
            public static void CalculateCellTransform(
            out Vector3 position, out Vector2Int coordinate,
                out Vector3 normal, out Vector2 dimensions,
            Cell2D cell, Config config)
            {
                position = CalculatePositionFromKey(cell.Key, config);
                coordinate = CalculateCoordinateFromKey(cell.Key, config);
                normal = config.GridNormal;
                dimensions = config.CellConfig.CellDimensions;
            }

            public static Vector3 CalculatePositionFromKey(Vector2Int key, Config config)
            {
                Cell2D.SettingsConfig cellConfig = config.CellConfig;

                // Get the origin key of the grid
                Vector2Int originKey = CalculateOriginKey(config);

                // Calculate the spacing offset && clamp it to avoid overlapping cells
                Vector2 spacingOffsetPos = cellConfig.CellSpacing + Vector2.one; // << Add 1 to allow for values of 0
                spacingOffsetPos.x = Mathf.Clamp(spacingOffsetPos.x, 0.5f, float.MaxValue);
                spacingOffsetPos.y = Mathf.Clamp(spacingOffsetPos.y, 0.5f, float.MaxValue);

                // Calculate bonding offsets
                Vector2 bondingOffset = Vector2.zero;
                if (key.y % 2 == 0)
                    bondingOffset.x = cellConfig.CellBonding.x;
                if (key.x % 2 == 0)
                    bondingOffset.y = cellConfig.CellBonding.y;

                // Calculate the offset of the cell from the grid origin
                Vector2 originOffsetPos = originKey * cellConfig.CellDimensions;
                Vector2 keyOffsetPos = key * cellConfig.CellDimensions;

                // Calculate the final position of the cell
                Vector2 cellPosition = (keyOffsetPos - originOffsetPos); // << Calculate the position offset
                cellPosition *= spacingOffsetPos; // << Multiply the spacing offset
                cellPosition += bondingOffset; // << Add the bonding offset

                // Create a rotation matrix based on the grid's normal
                Quaternion rotation = Quaternion.LookRotation(config.GridNormal, Vector3.forward);

                // Apply the rotation to the grid offset and return the final world position
                return config.GetGridWorldPosition() + (rotation * new Vector3(cellPosition.x, cellPosition.y, 0));
            }

            static Vector2Int CalculateCoordinateFromKey(Vector2Int key, Grid2D.Config config)
            {
                Vector2Int originKey = CalculateOriginKey(config);
                return key - originKey;
            }

            static Vector2Int CalculateOriginKey(Grid2D.Config config)
            {
                Vector2Int gridDimensions = config.GridDimensions - Vector2Int.one;
                Vector2Int originKey = Vector2Int.zero;

                switch (config.GridAlignment)
                {
                    case Grid2D.Alignment.BottomLeft:
                        originKey = new Vector2Int(0, 0);
                        break;
                    case Grid2D.Alignment.BottomCenter:
                        originKey = new Vector2Int(Mathf.FloorToInt(gridDimensions.x / 2), 0);
                        break;
                    case Grid2D.Alignment.BottomRight:
                        originKey = new Vector2Int(Mathf.FloorToInt(gridDimensions.x), 0);
                        break;
                    case Grid2D.Alignment.MiddleLeft:
                        originKey = new Vector2Int(0, Mathf.FloorToInt(gridDimensions.y / 2));
                        break;
                    case Grid2D.Alignment.Center:
                        originKey = new Vector2Int(
                        Mathf.FloorToInt(gridDimensions.x / 2),
                            Mathf.FloorToInt(gridDimensions.y / 2)
                            );
                        break;
                    case Grid2D.Alignment.MiddleRight:
                        originKey = new Vector2Int(
                            Mathf.FloorToInt(gridDimensions.x),
                            Mathf.FloorToInt(gridDimensions.y / 2)
                            );
                        break;
                    case Grid2D.Alignment.TopLeft:
                        originKey = new Vector2Int(0, Mathf.FloorToInt(gridDimensions.y));
                        break;
                    case Grid2D.Alignment.TopCenter:
                        originKey = new Vector2Int(
                            Mathf.FloorToInt(gridDimensions.x / 2),
                            Mathf.FloorToInt(gridDimensions.y)
                            );
                        break;
                    case Grid2D.Alignment.TopRight:
                        originKey = new Vector2Int(
                            Mathf.FloorToInt(gridDimensions.x),
                            Mathf.FloorToInt(gridDimensions.y)
                            );
                        break;
                }

                return originKey;
            }
        }
    }
}
