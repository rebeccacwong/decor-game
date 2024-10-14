using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int swatchId, int gameObjId)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, ID, swatchId, gameObjId);
        foreach (var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            Debug.Log($"Adding object with gameObj Id {gameObjId} to {pos}.");
            placedObjects[pos] = data;
        }
    }

    /* 
     * Calculates a list of grid positions that will be occupied 
     * by an object of size objectSize.
     */
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positions = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                // position begins at bottom left corner of grid
                positions.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return positions;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(Vector3Int pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    public int GetObjectIdFromGridPosition(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition].ID;
        }
        return -1;
    }

    public int GetGameObjectIdFromGridPosition(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition].gameObjId;
        }
        return -1;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int swatchId { get; private set; }
    public int gameObjId { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int swatchId, int gameObjId)
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = iD;
        this.swatchId = swatchId;
        this.gameObjId = gameObjId;
    }
}