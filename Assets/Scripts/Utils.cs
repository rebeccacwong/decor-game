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

}
