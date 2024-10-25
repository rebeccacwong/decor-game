using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public Vector3 centroid { get; internal set; }
    private Vector2Int objectSize;
    private int rotation = 0;

    public void UpdateItemDataAccordingToRotation(int newRotation)
    {
        if (newRotation == (rotation + 90) % 360)
        {
            objectSize = new Vector2Int(objectSize.y, objectSize.x);
        }
        rotation = newRotation;
    }

    public int getItemRotation()
    {
        return rotation;
    }

    public void setItemRotation(int newRotation)
    {
        rotation = newRotation;
    }

    public Vector2Int getObjectSize()
    {
        return objectSize;
    }

    public void setObjectSize(Vector2Int size)
    {
        objectSize = size;
    }
}
