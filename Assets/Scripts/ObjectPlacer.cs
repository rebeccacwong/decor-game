using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private Dictionary<int, GameObject> placedObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObj = Instantiate(prefab);
        newObj.transform.position = position;
        placedObjects.Add(newObj.GetInstanceID(), newObj);
        return newObj.GetInstanceID();
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
