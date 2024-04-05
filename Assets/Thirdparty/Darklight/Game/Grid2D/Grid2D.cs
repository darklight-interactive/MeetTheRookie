using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darklight.Game.Grid2D
{
    [System.Serializable]
    public class Grid2D<Type>
    {
        public class Coordinate
        {
            public Vector2Int positionKey { get; set; }
            public Type typeValue { get; set; }
            public Coordinate(Vector2Int key, Type value)
            {
                this.positionKey = key;
                this.typeValue = value;
            }
        }

        // -- [[ Grid2D Properties ]] ------------------------------ >>
        [SerializeField] private Dictionary<Vector2Int, Coordinate> grid = new Dictionary<Vector2Int, Coordinate>();
        private Vector2Int gridXAxis = Vector2Int.right;
        private Vector2Int gridYAxis = Vector2Int.up;
        private Vector2Int gridSize = new Vector2Int(3, 3);

        [Range(0.1f, 1f)]
        public float coordinateSize = 1;

        public Grid2D(Vector2Int gridSize, int cellSize)
        {
            this.gridSize = gridSize;
            this.coordinateSize = cellSize;
            InitializeGrid();
        }

        public Transform gridParent = null;
        public Vector2Int gridParentPositionKey = new Vector2Int(0, 0);
        public Grid2D(Vector2Int gridSize, int cellSize, Transform gridParent)
        {
            this.gridSize = gridSize;
            this.coordinateSize = cellSize;
            this.gridParent = gridParent;
            InitializeGrid();
        }

        public void InitializeGrid()
        {
            grid = new Dictionary<Vector2Int, Coordinate>();
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2Int position = gridXAxis * x + gridYAxis * y;
                    Coordinate coordinate = new Coordinate(position, default);
                    grid.Add(position, coordinate);
                }
            }
        }

        public Type GetCoordinateValue(Vector2Int position)
        {
            if (grid.ContainsKey(position))
            {
                return grid[position].typeValue;
            }
            return default;
        }

        public List<Vector2Int> GetPositionKeys()
        {
            return new List<Vector2Int>(grid.Keys);
        }

        public Vector3 GetCoordinatePositionInWorldSpace(Vector2Int positionKey)
        {
            if (this.gridParent == null) { return Vector3.zero; }

            Vector2Int offsetPosition = positionKey - gridParentPositionKey; // Subtract the location of the parentPosition
            Vector3 vec3_position = new Vector3(offsetPosition.x, offsetPosition.y, 0); // Convert to Vector3
            vec3_position *= coordinateSize; // Scale by the coordinate size

            Vector3 worldSpacePosition = gridParent.TransformVector(gridParent.position + vec3_position); // Transform to world space
            return worldSpacePosition;
        }
    }
}
