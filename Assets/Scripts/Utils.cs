using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class Utils
{
    //public static Vector3 GetCenterOfColliderInWorldSpace(Collider collider, GameObject obj, int rotation)
    //{
    //    Vector3 size = collider.bounds.size;
    //    Vector3 offset = new Vector3(size.x / 2, size.y / 2, size.z / 2);

    //    if (rotation == 90)
    //    {
    //        offset = new Vector3(size.z / 2, size.y / 2, size.x / -2);
    //    }
    //    else if (rotation == 270)
    //    {
    //        offset = new Vector3(size.z / -2, size.y / 2, size.x / 2);
    //    }

    //    Debug.Log($"Rotation is: {rotation}, offset vector is {offset}, position is {obj.transform.position}");
    //    return obj.transform.position + offset;
    //}

    //public static void RotateObjectAroundCenter(Collider collider, GameObject obj, int rotation)
    //{
    //    Vector3 center = GetCenterOfColliderInWorldSpace(collider, obj, rotation);
    //    obj.transform.RotateAround(center, Vector3.up, rotation);
    //}

    public static Vector3 GetWorldPosForObjWhenSnapped(Vector3Int gridPosition, Grid grid, Vector2Int objSize)
    {
        Vector3 snapped = gridPosition;

        if (objSize.x % 2 != 0)
        {
            // We'll need to place it on a grid boundary
            snapped.x += grid.cellSize.x / 2;
        }
        if (objSize.y % 2 != 0)
        {
            snapped.z += grid.cellSize.z / 2;
        }

        return snapped;
    }

    public static void ScaleRoom(GameObject room, Vector2Int objSize)
    {
        float roomLengthOffset = 0.15f;
        int diffInDimensions = Mathf.Abs(objSize.x - objSize.y);

        ItemData itemData = room.GetComponent<ItemData>();
        Debug.Assert(itemData != null);
        Vector2Int currObjSize = itemData.getObjectSize();

        foreach(Transform child in room.transform)
        {
            if (child.name.Contains("XAxisWall"))
            {
                child.localScale = new Vector3(objSize.x + roomLengthOffset, child.localScale.y, child.localScale.z);
                int offset = (child.name == "XAxisWall1") ? -1 : 1;
                child.transform.position = new Vector3(child.transform.position.x, child.transform.position.y, objSize.y / 2 * offset);
            }
            else if (child.name.Contains("ZAxisWall"))
            {
                child.localScale = new Vector3(child.localScale.x, child.localScale.y, objSize.y + roomLengthOffset);
                if (objSize.x < objSize.y)
                {
                    int offset = (child.name == "ZAxisWall1") ? 1 : -1;
                    child.transform.position += new Vector3(objSize.y / 2 * offset, child.transform.position.y, child.transform.position.z);
                }
            }
            else if (child.name == "Floor")
            {
                child.localScale = new Vector3(objSize.x, child.localScale.y, objSize.y);
            }
            else
            {
                Debug.LogWarning($"Unexpected child of {room} found, called {child.name}");
            }
        }

        itemData.setObjectSize(objSize);

    }

}
