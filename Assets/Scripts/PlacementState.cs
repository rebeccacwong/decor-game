using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
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
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnClickAction(Vector3Int gridPosition)
    {
        if (!CanPlaceStructure(gridPosition, selectedObjectIndex))
        {
            Debug.Log($"Cannot place object of index {selectedObjectIndex} at {gridPosition}.");
            // TODO: Add error sound
            return;
        }

        // Instantiate structure
        ObjectData data = database.objectsData[selectedObjectIndex];

        int gameObjId = objectPlacer.PlaceObject(data.Prefab, grid.CellToWorld(gridPosition), gridPosition, itemRotation);

        // TODO: make floor data as object index 0 and uncomment this line.
        // GridData selectedData = selectedObjectIndex == 0 ? floorData : furnitureData;
        GridData selectedData = furnitureData;
        furnitureData.AddObjectAt(gridPosition, data.Size, data.ID, data.SwatchID, gameObjId);

        previewSystem.UpdatePositionOfPreview(grid.CellToWorld(gridPosition), false);
    }

    public void OnEscapeAction()
    {
        EndState();
    }

    private bool CanPlaceStructure(Vector3Int gridPosition, int objectIndex)
    {
        // TODO: make floor data as object index 0 and uncomment this line.
        // GridData selectedData = selectedObjectIndex == 0 ? floorData : furnitureData;
        GridData selectedData = furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[objectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool canPlace = CanPlaceStructure(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePositionOfPreview(grid.CellToWorld(gridPosition), canPlace);
    }

    public void Rotate90DegreesCW()
    {
        itemRotation = (itemRotation + 90) % 360;
        previewSystem.UpdatePreviewRotation(itemRotation);
    }
}
