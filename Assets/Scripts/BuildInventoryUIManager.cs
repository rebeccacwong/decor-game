using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuildInventoryUIManager : MonoBehaviour
{
    private enum BuildMode
    {
        Build, Furnish
    }

    [SerializeField]
    [Tooltip("The object database of furnishings.")]
    private ObjectDatabaseSO furnitureDatabase;

    [SerializeField]
    [Tooltip("The object database of build assets.")]
    private ObjectDatabaseSO buildDatabase;

    [SerializeField]
    [Tooltip("The UI gameobject that is the parent of the inventory content.")]
    private GameObject contentParent;

    [SerializeField]
    [Tooltip("Prefab for the buttons that you can press for each inventory option.")]
    private GameObject inventoryButtonPrefab;

    [SerializeField]
    private PlacementSystem placementSystem;

    [SerializeField]
    [Tooltip("The build mode buttons. First should be build, then furnish mode.")]
    private List<BuildModeButton> buildModeButtons;

    private Dictionary<int, int> objIdToNumSwatches = new();

    private BuildMode activeMode = BuildMode.Build;


    // Start is called before the first frame update
    private void Start()
    {
        // Start at build mode by default.
        SwitchToBuildMode();
    }

    public void SwitchToFurnishMode()
    {
        ClearContent();

        buildModeButtons[(int)activeMode]?.MarkButtonAsNotSelected();
        activeMode = BuildMode.Furnish;
        buildModeButtons[(int)activeMode]?.MarkButtonAsSelected();

        foreach (ObjectData obj in furnitureDatabase.objectsData)
        {
            CreateFurnitureOptionInInventory(obj);
        }
        placementSystem.EnableFurniturePlacement();
    }

    public void SwitchToBuildMode()
    {
        ClearContent();

        buildModeButtons[(int)activeMode]?.MarkButtonAsNotSelected();
        activeMode = BuildMode.Build;
        buildModeButtons[(int)activeMode]?.MarkButtonAsSelected();

        foreach (ObjectData obj in buildDatabase.objectsData)
        {
            CreateBuildOptionInInventory(obj);
        }
        placementSystem.EnableBuildPlacement();
    }

    private void ClearContent()
    {
        objIdToNumSwatches.Clear();
        foreach (Transform child in contentParent.transform)
        {
            GameObject elem = child.gameObject;
            Destroy(elem);
        }
    }

    private void CreateFurnitureOptionInInventory(ObjectData obj)
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
        button.onClick.AddListener(delegate { placementSystem.StartFurniturePlacement(id, true); });

        UpdateButtonImage(buttonGameObj, obj);
    }

    private void CreateBuildOptionInInventory(ObjectData obj)
    {
        GameObject buttonGameObj = Instantiate(inventoryButtonPrefab, contentParent.transform);

        Button button = buttonGameObj.GetComponent<Button>();

        int id = obj.ID;
        if (obj.Category == ItemCategory.Rooms)
        {
            button.onClick.AddListener(delegate { placementSystem.StartRoomPlacement(id); });
        }
        // TODO: Add support for other categories

        UpdateButtonImage(buttonGameObj, obj);
    }

    private void UpdateButtonImage(GameObject buttonGameObj, ObjectData obj)
    {
        Image img = buttonGameObj.transform.Find("Image")?.GetComponent<Image>();
        if (obj.Image)
        {
            img.sprite = obj.Image;
            img.preserveAspect = true;
        }
    }
}
