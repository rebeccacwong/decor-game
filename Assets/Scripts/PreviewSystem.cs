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

    private Renderer cellIndicatorRenderer;
    private int rotation = 0;
    private GameObject previewObject;
    private ItemData previewItemData;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);

        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        Debug.Assert(cellIndicatorRenderer != null);
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int objSize, bool showTransparent = true, int objRotation = 0)
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

        RotatePreviewObj(objRotation);
        rotation = objRotation;

        PrepareCursor(objSize);
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

    private void PrepareCursor(Vector2Int objSize)
    {
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

    public void UpdatePositionAndSizeOfPreview(Vector3 centerWorldPos, bool isPositionValid, Vector2Int objSize)
    {
        MovePreviewObject(centerWorldPos);
        Utils.ScaleRoom(previewObject, centerWorldPos, objSize);
        PrepareCursor(objSize);
        MoveCursor(centerWorldPos);
        ApplyFeedback(isPositionValid);
    }

    public void ShowEditCursor()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(new Vector2Int(1, 1));
    }

    public void HideEditCursor()
    {
        cellIndicator.SetActive(false);
    }

    public void MoveCursor(Vector3 worldPos)
    {
        cellIndicator.transform.position = worldPos;
    }

    public (Vector2Int, int) UpdatePreviewRotation90DegCCW()
    {
        rotation = (rotation + 90) % 360;
        previewItemData.UpdateItemDataAccordingToRotation(rotation);

        RotatePreviewObj(90);

        return (previewItemData.getObjectSize(), rotation);
    }

    private void RotatePreviewObj(int degrees)
    {
        previewObject.transform.Rotate(0, 0, degrees);
        cellIndicator.transform.Rotate(0, degrees, 0);
    }

    public GameObject GetPreviewObject()
    {
        return previewObject;
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

    private void MovePreviewObject(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, previewYOffset, position.z);
    }
}
