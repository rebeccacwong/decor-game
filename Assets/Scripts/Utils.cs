using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class Utils
{
    public static Vector3 GetCenterOfColliderInWorldSpace(Collider collider, GameObject obj)
    {
        Vector3 size = collider.bounds.size;
        return obj.transform.position + new Vector3(size.x / 2, size.y / 2, size.z / 2);
    }
}
