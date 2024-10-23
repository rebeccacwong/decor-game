using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.PlayerSettings;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3 worldPos, Vector2Int objectSize, int ID, int swatchId, int gameObjId, Grid grid)
    {
        List<Vector3Int> positionsToOccupy = CalculateGridPositions(worldPos, objectSize, grid);
        PlacementData data = new PlacementData(ID, swatchId, gameObjId);
        foreach (var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            placedObjects[pos] = data;
        }
    }

    /*
     * Gets the gridPositions that the object occupies when it is centered at worldPos.
     */
    private List<Vector3Int> CalculateGridPositions(Vector3 worldPos, Vector2Int objectSize, Grid grid)
    {
        List<Vector3Int> positionsToOccupy = new();

        Vector3 bottomPosition = new Vector3(worldPos.x - (objectSize.x / 2f), 0, worldPos.z - (objectSize.y / 2f));
        Vector3 topPosition = new Vector3(worldPos.x + (objectSize.x / 2f), 0, worldPos.z + (objectSize.y / 2f));

        Vector3Int bottomGridCell = grid.WorldToCell(bottomPosition);
        Vector3Int topGridCell = grid.WorldToCell(topPosition);
        bottomGridCell.y = 0;
        topGridCell.y = 0;

        if (topGridCell == bottomGridCell)
        {
            positionsToOccupy.Add(new Vector3Int(topGridCell.x, 0, topGridCell.z));
        }
        else
        {
            for (int x = bottomGridCell.x; x < topGridCell.x; x++)
            {
                for (int z = bottomGridCell.z; z < topGridCell.z; z++)
                {
                    positionsToOccupy.Add(new Vector3Int(x, 0, z));
                }
            }
        }
        return positionsToOccupy;
    }

    private Vector3Int GetBottomLeftGridPos(Vector3 worldPos, Vector2Int objectSize, Grid grid)
    {
        Vector3 bottomPosition = new Vector3(worldPos.x - (objectSize.x / 2f), 0, worldPos.z - (objectSize.y / 2f));
        Vector3Int bottomGridCell = grid.WorldToCell(bottomPosition);
        bottomGridCell.y = 0;
        return bottomGridCell;
    }

    public bool CanPlaceObjectAt(Vector3 worldPos, Vector2Int objectSize, Grid grid)
    {
        List<Vector3Int> positionsToOccupy = CalculateGridPositions(worldPos, objectSize, grid);
        foreach (Vector3Int pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    public bool CanPlaceObjectInGridRange(Vector3Int gridPos1, Vector3Int gridPos2)
    {
        Vector3Int bottomLeftCorner = Vector3Int.Min(gridPos1, gridPos2);
        Vector3Int topRightCorner = Vector3Int.Max(gridPos1, gridPos2);

        for (int x = bottomLeftCorner.x; x < topRightCorner.x; x++)
        {
            for (int z = bottomLeftCorner.z; z < topRightCorner.z; z++)
            {
                if (placedObjects.ContainsKey(new Vector3Int(x, 0, z)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetObjectIdFromItemWorldPos(Vector3 worldPos, Vector2Int objectSize, Grid grid)
    {
        Vector3Int gridPosition = GetBottomLeftGridPos(worldPos, objectSize, grid);
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition].ID;
        }
        return -1;
    }

    public int GetGameObjectIdFromWorldPos(Vector3 worldPos, Vector2Int objectSize, Grid grid)
    {
        Vector3Int gridPosition = GetBottomLeftGridPos(worldPos, objectSize, grid);
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition].gameObjId;
        }
        return -1;
    }

    public void RemoveObjectAt(Vector3 worldPos, Vector2Int objectSize, Grid grid)
    {
        List<Vector3Int> positionsToOccupy = CalculateGridPositions(worldPos, objectSize, grid);
        foreach (var gridPos in positionsToOccupy)
        {
            placedObjects.Remove(gridPos);
        }
    }
}

public class PlacementData
{
    public int ID { get; private set; }
    public int swatchId { get; private set; }
    public int gameObjId { get; private set; }

    public PlacementData(int iD, int swatchId, int gameObjId)
    {
        this.ID = iD;
        this.swatchId = swatchId;
        this.gameObjId = gameObjId;
    }
}