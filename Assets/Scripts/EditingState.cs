using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditingState : IBuildingState
{
    Vector3Int initialGridPosition;
    Grid grid;
    ObjectDatabaseSO database;
    int selectedObjectIndex;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    int itemRotation = 0;

    public EditingState(
        Vector3Int initialGridPosition,
        Grid grid,
        ObjectDatabaseSO database,
        PreviewSystem previewSystem,
        GridData floorData,
        GridData furnitureData,
        ObjectPlacer objPlacer)
    {
        this.initialGridPosition = initialGridPosition;
        this.grid = grid;
        this.database = database;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objPlacer;

        GridData selectedData = furnitureData;

        int gameObjId = selectedData.GetGameObjectIdFromGridPosition(initialGridPosition);
        if (gameObjId == -1)
        {
            throw new Exception($"Could not find gameObjID at position {initialGridPosition}!");
        }
        int itemId = selectedData.GetObjectIdFromGridPosition(initialGridPosition);
        if (itemId == -1)
        {
            throw new Exception($"Could not find item ID at position {initialGridPosition}");
        }
        Debug.Log($"Found item of index {selectedObjectIndex} to edit.");
        this.selectedObjectIndex = itemId;

        Debug.Log($"Removing structure at grid position {initialGridPosition}");
        selectedData.RemoveObjectAt(initialGridPosition);
        objPlacer.RemoveObject(gameObjId);

        previewSystem.StartShowingPlacementPreview(
            database.objectsData[itemId].Prefab,
            database.objectsData[itemId].Size,
            false);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
        //return;
    }

    public void OnClickAction(Vector3Int gridPosition)
    {
        if (!CanPlaceStructure(gridPosition))
        {
            Debug.Log($"Cannot place object of index {selectedObjectIndex} at {gridPosition}.");
            // TODO: Add error sound
            return;
        }

        // physically place the final obj and update gridData
        ObjectData data = database.objectsData[selectedObjectIndex];
        int newGameObjId = objectPlacer.PlaceObject(data.Prefab,
                                                    grid.CellToWorld(gridPosition),
                                                    gridPosition,
                                                    itemRotation);
        furnitureData.AddObjectAt(gridPosition, data.Size, data.ID, data.SwatchID, newGameObjId);

        previewSystem.UpdatePositionOfPreview(grid.CellToWorld(gridPosition), false);
    }

    public void OnEscapeAction()
    {
        // Reset position back to original
        int newGameObjId = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(initialGridPosition),
            initialGridPosition,
            itemRotation);

        ObjectData data = database.objectsData[selectedObjectIndex];
        furnitureData.AddObjectAt(initialGridPosition, data.Size, data.ID, data.SwatchID, newGameObjId);
    }

    private bool CanPlaceStructure(Vector3Int gridPosition)
    {
        // TODO: make floor data as object index 0 and uncomment this line.
        // GridData selectedData = selectedObjectIndex == 0 ? floorData : furnitureData;
        GridData selectedData = furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool canPlace = CanPlaceStructure(gridPosition);
        previewSystem.UpdatePositionOfPreview(grid.CellToWorld(gridPosition), canPlace);
    }

    public void Rotate90DegreesCW()
    {
        itemRotation = (itemRotation + 90) % 360;
        previewSystem.UpdatePreviewRotation(itemRotation);
    }
}
