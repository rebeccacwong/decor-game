using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private Dictionary<int, GameObject> placedObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 worldPos, Vector2Int defaultObjectSize, int rotation)
    {
        GameObject newObj = PlaceObjectInScene(prefab, worldPos, rotation);

        placedObjects.Add(newObj.GetInstanceID(), newObj);

        ItemData itemData = newObj.GetComponent<ItemData>();
        if (itemData == null)
        {
            throw new System.Exception($"Object of prefab {prefab} is missing the required ItemData script!");
        }
        itemData.setItemRotation(rotation);
        itemData.setObjectSize(GetObjectSizeWhenRotated(defaultObjectSize, rotation));

        return newObj.GetInstanceID();
    }

    public int PlaceRoom(GameObject roomPrefab, Vector3 worldPos, Vector2Int dimensions)
    {
        GameObject newRoom = PlaceObjectInScene(roomPrefab, worldPos, 0);
        Utils.ScaleRoom(newRoom, worldPos, dimensions);

        placedObjects.Add(newRoom.GetInstanceID(), newRoom);

        ItemData itemData = newRoom.GetComponent<ItemData>();
        if (itemData == null)
        {
            throw new System.Exception($"Object of prefab {roomPrefab} is missing the required ItemData script!");
        }
        itemData.setObjectSize(dimensions);

        return newRoom.GetInstanceID();
    }

    private static Vector2Int GetObjectSizeWhenRotated(Vector2Int defaultObjectSize, int rotation)
    {
        if (rotation == 90 || rotation == 270)
        {
            return new Vector2Int(defaultObjectSize.y, defaultObjectSize.x);
        }
        return defaultObjectSize;
    }

    /*
     * Helper method that places the prefab in the scene.
     * Returns the newly instantiated gameObject.
     */
    private GameObject PlaceObjectInScene(GameObject prefab, Vector3 worldPos, int rotation)
    {
        GameObject newObj = Instantiate(prefab);
        newObj.transform.position = worldPos;
        newObj.transform.Rotate(0, 0, rotation);

        Collider objCollider = newObj.GetComponent<Collider>();
        if (objCollider == null)
        {
            throw new System.Exception($"GameObject {newObj} is missing a required Collider component!");
        }

        //Utils.RotateObjectAroundCenter(objCollider, newObj, rotation);
        return newObj;
    }

    public void RemoveObject(int gameObjId)
    {
        if (placedObjects.ContainsKey(gameObjId))
        {
            Destroy(placedObjects[gameObjId]);
            placedObjects.Remove(gameObjId);
        }
        else
        {
            Debug.LogError($"Could not find key {gameObjId} in placedObjects dictionary!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
