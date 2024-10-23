
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class EditingState : IBuildingState
{
    Vector3 initialWorldPos;
    Grid grid;
    ObjectDatabaseSO database;
    int selectedObjectIndex;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    private Vector2Int updatedObjectSize;
    private Vector2Int initialObjectSize;
    private int itemRotation = 0;
    private int initialItemRotation = 0;

    public EditingState(
        Vector3 initialWorldPos,
        Grid grid,
        ObjectDatabaseSO database,
        PreviewSystem previewSystem,
        GridData floorData,
        GridData furnitureData,
        ObjectPlacer objPlacer,
        ItemData itemData)
    {
        this.initialWorldPos = initialWorldPos;
        this.grid = grid;
        this.database = database;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objPlacer;
        this.updatedObjectSize = itemData.getObjectSize();
        this.initialObjectSize = updatedObjectSize;
        this.itemRotation = itemData.getItemRotation();
        this.initialItemRotation = itemRotation;

        GridData selectedData = furnitureData;

        int gameObjId = selectedData.GetGameObjectIdFromWorldPos(initialWorldPos, updatedObjectSize, grid);
        if (gameObjId == -1)
        {
            throw new Exception($"Could not find gameObjID at world position {initialWorldPos}!");
        }

        int itemId = selectedData.GetObjectIdFromItemWorldPos(initialWorldPos, updatedObjectSize, grid);
        if (itemId == -1)
        {
            throw new Exception($"Could not find item ID at world position {initialWorldPos}");
        }
        Debug.Log($"Found item of index {selectedObjectIndex} to edit.");
        this.selectedObjectIndex = itemId;

        Debug.Log($"Removing structure at world position {initialWorldPos}");
        selectedData.RemoveObjectAt(initialWorldPos, updatedObjectSize, grid);
        objPlacer.RemoveObject(gameObjId);

        previewSystem.StartShowingPlacementPreview(
            database.objectsData[itemId].Prefab,
            updatedObjectSize,
            false);

        previewSystem.UpdatePositionOfPreview(initialWorldPos, true);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnClickAction(Vector3Int gridPosition)
    {
        ObjectData data = database.objectsData[selectedObjectIndex];
        Vector3 snappedWorldPos = Utils.GetWorldPosForObjWhenSnapped(gridPosition, grid, updatedObjectSize);

        if (!CanPlaceStructure(snappedWorldPos))
        {
            Debug.Log($"Cannot place object of index {selectedObjectIndex} at {snappedWorldPos}.");
            // TODO: Add error sound
            return;
        }

        // physically place the final obj and update gridData
        int newGameObjId = objectPlacer.PlaceObject(data.Prefab, snappedWorldPos, data.Size, itemRotation);
        furnitureData.AddObjectAt(snappedWorldPos, updatedObjectSize, data.ID, data.SwatchID, newGameObjId, grid);

        previewSystem.UpdatePositionOfPreview(snappedWorldPos, false);
    }

    public void OnEscapeAction()
    {
        // Reset position back to original
        ObjectData data = database.objectsData[selectedObjectIndex];
        int newGameObjId = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            initialWorldPos,
            data.Size,
            initialItemRotation);
        furnitureData.AddObjectAt(initialWorldPos, initialObjectSize, data.ID, data.SwatchID, newGameObjId, grid);
    }

    private bool CanPlaceStructure(Vector3 worldPosition)
    {
        // TODO: make floor data as object index 0 and uncomment this line.
        // GridData selectedData = selectedObjectIndex == 0 ? floorData : furnitureData;
        GridData selectedData = furnitureData;
        return selectedData.CanPlaceObjectAt(worldPosition, updatedObjectSize, grid);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        Vector3 snappedWorldPos = Utils.GetWorldPosForObjWhenSnapped(gridPosition, grid, updatedObjectSize);
        Debug.Log($"Grid position {gridPosition} snapped to {snappedWorldPos}");

        bool canPlace = CanPlaceStructure(snappedWorldPos);
        previewSystem.UpdatePositionOfPreview(snappedWorldPos, canPlace);
    }

    public void OnMouseUpAction(Vector3Int gridPosition)
    {
        throw new NotImplementedException();
    }

    public void Rotate90DegreesCW()
    {
        var output = previewSystem.UpdatePreviewRotation90DegCW();
        updatedObjectSize = output.Item1;
        itemRotation = output.Item2;
    }
}
