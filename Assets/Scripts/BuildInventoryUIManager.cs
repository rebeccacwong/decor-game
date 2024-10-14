using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildInventoryUIManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The object database of furnishings.")]
    private ObjectDatabaseSO database;

    [SerializeField]
    [Tooltip("The UI gameobject that is the parent of the inventory content.")]
    private GameObject contentParent;

    [SerializeField]
    [Tooltip("Prefab for the buttons that you can press for each inventory option.")]
    private GameObject inventoryButtonPrefab;

    [SerializeField]
    private PlacementSystem placementSystem;

    Dictionary<int, int> objIdToNumSwatches = new();
    

    // Start is called before the first frame update
    void Start()
    {
        foreach (ObjectData obj in database.objectsData)
        {
            CreateButtonInInventory(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: add a filter the objects in the database to the category and show the objects
    }

    void CreateButtonInInventory(ObjectData obj)
    {
        if (objIdToNumSwatches.ContainsKey(obj.ID))
        {
            // do not create a new button, as there is another swatch already in inventory.
            // TODO: populate this as a swatch option.
            objIdToNumSwatches[obj.ID]++;
            return;
        }
        else
        {
            objIdToNumSwatches[obj.ID] = 1;
        }

        GameObject buttonGameObj = Instantiate(inventoryButtonPrefab, contentParent.transform);

        Button button = buttonGameObj.GetComponent<Button>();

        int id = obj.ID;
        button.onClick.AddListener(delegate { placementSystem.StartPlacement(id, true); });

        Image img = buttonGameObj.transform.Find("Image")?.GetComponent<Image>();
        if (obj.Image)
        {
            img.sprite = obj.Image;
            img.preserveAspect = true;
        }
    }
}
