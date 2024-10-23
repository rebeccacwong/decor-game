using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuildState : IBuildingState
{
    private Vector3Int gridStartPosition;

    Grid grid;
    int roomId;
    PreviewSystem previewSystem;
    //ObjectDatabaseSO buildDatabase;
    GridData floorData;
    ObjectPlacer objectPlacer;
    int itemRotation = 0;

    public void EndState()
    {

    }

    public void OnClickAction(Vector3Int gridPosition)
    {
        gridStartPosition = gridPosition;

    }

    public void OnEscapeAction()
    {

    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool canPlace = floorData.CanPlaceObjectInGridRange(gridStartPosition, gridPosition);
        previewSystem.UpdatePositionOfPreview(GetWorldPosCenterFromGridRange(gridStartPosition, gridPosition), canPlace);
    }

    public void Rotate90DegreesCW()
    {
        throw new NotImplementedException();
    }

    public void OnMouseUpAction(Vector3Int gridPosition)
    {
        if (floorData.CanPlaceObjectInGridRange(gridStartPosition, gridPosition))
        {

        }
    }

    private Vector3 GetWorldPosCenterFromGridRange(Vector3Int gridPos1, Vector3Int gridPos2)
    {
        Vector3Int bottomLeftCorner = Vector3Int.Min(gridPos1, gridPos2);

        // We know grid positions are in bottom left corner of cell. To get top
        // right corner of the grid pos in world coordinates, have to add (1, 0, 1).
        Vector3Int topRightCorner = Vector3Int.Max(gridPos1, gridPos2) + new Vector3Int(1, 0, 1);

        return bottomLeftCorner + ((topRightCorner - bottomLeftCorner) / 2);
    }
}
