using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using UnityEngine;

[RequireComponent(typeof(Grid2D_OverlapComponent))]
public class MTRDestinationReciever : InteractionReciever
{
    Grid2D_OverlapComponent _overlapGrid => GetComponent<Grid2D_OverlapComponent>();

    public Library<Vector2Int, bool> cellLibrary = new Library<Vector2Int, bool>()
    {
        RequiredKeys = new Vector2Int[] { Vector2Int.zero },
        ReadOnlyKey = true,
        ReadOnlyValue = false
    };

    public override InteractionType InteractionType => InteractionType.DESTINATION;

    public void Awake()
    {
        cellLibrary.SetRequiredKeys(_overlapGrid.BaseGrid.CellKeys);
    }

    /// <summary>
    /// Get the closest, valid destination cell to the origin
    /// </summary>
    /// <param name="origin">The origin of the external object</param>
    /// <param name="destination">The closest, valid destination cell</param>
    public void GetClosestValidDestination(Vector2 origin, out Vector2 destination)
    {
        // If the cell library is not set, set it to the overlap grid's cell keys
        if (cellLibrary.Count == 0)
        {
            cellLibrary.SetRequiredKeys(_overlapGrid.BaseGrid.CellKeys);
        }

        // Get the closest cell to the origin
        Cell2D closestCell = _overlapGrid.BaseGrid.GetClosestCellTo(origin);
        destination = closestCell.Position;

        // Get the empty cells
        List<Cell2D> emptyCells = _overlapGrid.GetCellsWithColliderCount(0);

        // Find the closest empty cell
        float minDistance = float.MaxValue;
        foreach (Cell2D cell in emptyCells)
        {
            // Skip if cell is already occupied / disabled
            if (cellLibrary[cell.Key] == true)
                continue;

            // Compare the distance to the last minimum distance
            float distance = Vector2.Distance(origin, cell.Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                destination = cell.Position;
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        try
        {
            List<Cell2D> emptyCells = _overlapGrid.GetCellsWithColliderCount(0);
            MTRPlayerInteractor playerInteractor = FindFirstObjectByType<MTRPlayerInteractor>();
            GetClosestValidDestination(
                playerInteractor.transform.position,
                out Vector2 destination
            );
            foreach (Cell2D cell in _overlapGrid.BaseGrid.GetCells())
            {
                if (cellLibrary[cell.Key] == true)
                {
                    Gizmos.color = Color.black;
                }
                else if (!emptyCells.Contains(cell))
                {
                    Gizmos.color = Color.red;
                }
                else if (cell.Position.x == destination.x)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawLine(cell.Position, cell.Position + Vector3.up * 1f);
                Gizmos.DrawSphere(cell.Position, 0.025f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MTRDestinationReciever))]
    public class MTRDestinationRecieverCustomEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedObject _serializedObject;
        MTRDestinationReciever _script;

        private void OnEnable()
        {
            _serializedObject = new UnityEditor.SerializedObject(target);
            _script = (MTRDestinationReciever)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            UnityEditor.EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (GUILayout.Button("Initialize"))
            {
                _script.Awake();
            }

            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
