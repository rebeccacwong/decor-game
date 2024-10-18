using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;

    [SerializeField]
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    [SerializeField]
    GameObject cube;

    private Renderer cellIndicatorRenderer;
    private int rotation = 0;
    private GameObject previewObject;
    private ItemData previewItemData;

    // The offset to place the cellIndicator in the center of the cell.
    private Vector3 cellIndicatorOffset = new Vector3(0.5f, 0f, 0.5f);

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);

        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        Debug.Assert(cellIndicatorRenderer != null);
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int objSize, bool showTransparent)
    {
        Debug.Log("Start showing preview");
        previewObject = Instantiate(prefab);

        if (showTransparent)
        {
            PrepareTransparentPreview(previewObject);
        }

        previewItemData = previewObject.GetComponent<ItemData>();
        Debug.Assert(previewItemData != null, $"GameObject {previewObject} needs an ItemData component.");
        previewItemData.setObjectSize(objSize);

        PrepareCursor();
        cellIndicator.SetActive(true);
    }

    private void PrepareTransparentPreview(GameObject previewObj)
    {
        Renderer[] renderers = previewObj.GetComponentsInChildren<Renderer>();
        foreach(var renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    private void PrepareCursor()
    {
        Vector2Int objSize = previewItemData.getObjectSize();
        if (objSize.x > 0 && objSize.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(objSize.x, 1, objSize.y);
            cellIndicator.GetComponentInChildren<Renderer>().material.mainTextureScale = objSize;
        }
    }

    public void StopShowingPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        rotation = 0;
        previewItemData = null;
        cellIndicator.transform.rotation = Quaternion.identity;
        cellIndicator.SetActive(false);
    }

    public void UpdatePositionOfPreview(Vector3 worldPos, bool isPositionValid)
    {
        MovePreviewObject(worldPos);
        MoveCursor(worldPos);
        ApplyFeedback(isPositionValid);
    }

    private void ApplyFeedback(bool isPositionValid)
    {
        Color materialColor = isPositionValid ? Color.white : Color.red;
        materialColor.a = 0.8f;
        previewMaterialInstance.color = materialColor;

        Color cellIndicatorColor = isPositionValid ? Color.yellow : Color.red;
        cellIndicatorColor.a = 0.8f;
        cellIndicatorRenderer.material.color = cellIndicatorColor;
    }

    private void MoveCursor(Vector3 worldPos)
    {
        cellIndicator.transform.position = worldPos;
    }

    private void MovePreviewObject(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, previewYOffset, position.z);
    }

    public (Vector2Int, int) UpdatePreviewRotation90DegCW()
    {
        rotation = (rotation + 90) % 360;
        previewItemData.UpdateItemDataAccordingToRotation(rotation);

        previewObject.transform.Rotate(0, 0, 90f);
        cellIndicator.transform.Rotate(0, 90f, 0);

        return (previewItemData.getObjectSize(), rotation);
    }

    public GameObject GetPreviewObject()
    {
        return previewObject;
    }
}
