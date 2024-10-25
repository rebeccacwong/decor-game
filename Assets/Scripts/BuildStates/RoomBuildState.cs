using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomBuildState : IBuildingState
{
    private Vector3Int gridStartPosition;
    private int selectedObjectIndex;
    private bool isUserDrawing = false;

    Grid grid;
    int roomId;
    PreviewSystem previewSystem;
    ObjectDatabaseSO buildDatabase;
    GridData floorData;
    ObjectPlacer objectPlacer;
    int itemRotation = 0;

    public RoomBuildState(int roomId,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO database,
                          GridData floorData,
                          ObjectPlacer objectPlacer)
    {
        this.roomId = roomId;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.buildDatabase = database;
        this.floorData = floorData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(x => x.ID == roomId);
        if (selectedObjectIndex == -1)
        {
            throw new System.Exception($"No object with id {roomId} found in database.");
        }
        previewSystem.ShowEditCursor();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
        previewSystem.HideEditCursor();
    }

    public void OnClickAction(Vector3Int gridPosition)
    {
        gridStartPosition = gridPosition;
        isUserDrawing = true;

        previewSystem.StartShowingPlacementPreview(
            buildDatabase.objectsData[selectedObjectIndex].Prefab,
            buildDatabase.objectsData[selectedObjectIndex].Size,
            true);

        previewSystem.UpdatePositionOfPreview(gridPosition + new Vector3(0.5f, 0, 0.5f), true);
    }

    public void OnEscapeAction()
    {
        EndState();
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (isUserDrawing)
        {
            Vector3Int bottomLeft = getBottomLeftCorner(gridStartPosition, gridPosition);
            Vector3Int topRight = getTopRightCorner(gridStartPosition, gridPosition);

            bool canPlace = floorData.CanPlaceObjectInGridRange(gridStartPosition, gridPosition);

            Vector2Int objSize = GetSizeFromGridRange(bottomLeft, topRight);
            Vector3 worldPos = GetWorldPosCenterFromGridRange(bottomLeft, topRight);

            previewSystem.UpdatePositionAndSizeOfPreview(worldPos, canPlace, objSize);
        }
        else
        {
            Vector3 snappedWorldPos = Utils.GetWorldPosForObjWhenSnapped(gridPosition, grid, new Vector2Int(1, 1));
            previewSystem.MoveCursor(snappedWorldPos);
        }
    }

    public void Rotate90DegreesCCW()
    {
        throw new NotImplementedException();
    }

    public void OnMouseUpAction(Vector3Int gridPosition)
    {
        isUserDrawing = false;
        if (floorData.CanPlaceObjectInGridRange(gridStartPosition, gridPosition))
        {
            Vector3Int bottomLeft = getBottomLeftCorner(gridStartPosition, gridPosition);
            Vector3Int topRight = getTopRightCorner(gridStartPosition, gridPosition);

            Vector2Int dimensions = GetSizeFromGridRange(bottomLeft, topRight);
            Vector3 worldPos = GetWorldPosCenterFromGridRange(bottomLeft, topRight);

            int gameObjId = objectPlacer.PlaceRoom(buildDatabase.objectsData[selectedObjectIndex].Prefab, worldPos, dimensions);
            floorData.AddObjectAt(worldPos, dimensions, roomId, 0, gameObjId, grid);
        }
    }

    #region Private helper methods
    private Vector3Int getBottomLeftCorner(Vector3Int gridPos1, Vector3Int gridPos2)
    {

        return Vector3Int.Min(gridPos1, gridPos2);
    }

    private Vector3Int getTopRightCorner(Vector3Int gridPos1, Vector3Int gridPos2)
    {
        // We know grid positions are in bottom left corner of cell. To get top
        // right corner of the grid pos in world coordinates, have to add (1, 0, 1).
        return Vector3Int.Max(gridPos1, gridPos2) + new Vector3Int(1, 0, 1);
    }

    private Vector2Int GetSizeFromGridRange(Vector3Int gridPos1, Vector3Int gridPos2)
    {
        Vector2Int dist = new Vector2Int(Math.Abs(gridPos1.x - gridPos2.x), Math.Abs(gridPos1.z - gridPos2.z));
        return Vector2Int.Max(dist, new Vector2Int(1, 1));
    }

    private Vector3 GetWorldPosCenterFromGridRange(Vector3 bottomLeftCorner, Vector3 topRightCorner)
    {
        return bottomLeftCorner + ((topRightCorner - bottomLeftCorner) / 2f);
    }
    #endregion
}
