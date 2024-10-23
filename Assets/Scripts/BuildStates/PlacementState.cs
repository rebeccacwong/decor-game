using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private Vector2Int updatedObjectSize;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectDatabaseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    bool isFirstPlacement;
    int itemRotation = 0;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          bool isFirstPlacement)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.isFirstPlacement = isFirstPlacement;

        selectedObjectIndex = database.objectsData.FindIndex(x => x.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size,
                isFirstPlacement);
        }
        else
        {
            throw new System.Exception($"No object with id {iD} found in database.");
        }

        this.updatedObjectSize = database.objectsData[selectedObjectIndex].Size;
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

        // Instantiate structure
        int gameObjId = objectPlacer.PlaceObject(data.Prefab, snappedWorldPos, data.Size, itemRotation);

        // TODO: make floor data as object index 0 and uncomment this line.
        // GridData selectedData = selectedObjectIndex == 0 ? floorData : furnitureData;
        GridData selectedData = furnitureData;
        furnitureData.AddObjectAt(snappedWorldPos, updatedObjectSize, data.ID, data.SwatchID, gameObjId, grid);

        previewSystem.UpdatePositionOfPreview(snappedWorldPos, false);
    }

    public void OnEscapeAction()
    {
        EndState();
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
