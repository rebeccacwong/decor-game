using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private Dictionary<int, GameObject> placedObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position, Vector3Int gridPosition, int rotation)
    {
        GameObject newObj = PlaceObjectInScene(prefab, position, rotation);

        placedObjects.Add(newObj.GetInstanceID(), newObj);

        ItemData itemData = newObj.GetComponent<ItemData>();
        if (itemData == null)
        {
            throw new System.Exception($"Object of prefab {prefab} is missing the required ItemData script!");
        }
        itemData.gridPosition = gridPosition;
        return newObj.GetInstanceID();
    }

    /*
     * Helper method that places the prefab in the scene.
     * Returns the newly instantiated gameObject.
     */
    private GameObject PlaceObjectInScene(GameObject prefab, Vector3 position, int rotation)
    {
        GameObject newObj = Instantiate(prefab);
        newObj.transform.position = position;

        Collider objCollider = newObj.GetComponent<Collider>();
        if (objCollider == null)
        {
            throw new System.Exception($"GameObject {newObj} is missing a required Collider component!");
        }

        Vector3 center = Utils.GetCenterOfColliderInWorldSpace(objCollider, newObj);
        newObj.transform.RotateAround(center, Vector3.up, rotation);
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
