using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    private Collider previewObjCollider;
    private Quaternion originalRotation;
    private int rotation = 0;
    private Vector3 currPosition;

    // The offset to place the cellIndicator in the center of the cell.
    private Vector3 cellIndicatorOffset = new Vector3(0.5f, 0f, 0.5f);

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int objSize, bool showTransparent)
    {
        Debug.Log("Start showing preview");
        previewObject = Instantiate(prefab);
        currPosition = previewObject.transform.position;
        if (showTransparent)
        {
            PrepareTransparentPreview(previewObject);
        }

        previewObjCollider = previewObject.GetComponent<Collider>();
        if (previewObjCollider == null)
        {
            throw new Exception($"Object {previewObject} is missing required Collider component!");
        }
        originalRotation = previewObject.transform.rotation;

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
        cellIndicator.SetActive(false);
    }

    public void UpdatePositionOfPreview(Vector3 position, bool isPositionValid)
    {
        MovePreviewObject(position);
        MoveCursor(position);
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

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position + cellIndicatorOffset;
    }

    private void MovePreviewObject(Vector3 position)
    {
        // Reset to default
        previewObject.transform.position = currPosition;
        previewObject.transform.rotation = originalRotation;

        // update new position and rotation
        previewObject.transform.position = new Vector3(position.x, previewYOffset, position.z);
        Vector3 center = Utils.GetCenterOfColliderInWorldSpace(previewObjCollider, previewObject);
        previewObject.transform.RotateAround(center, Vector3.up, rotation);

        currPosition = position;
    }

    public void UpdatePreviewRotation(int newAngle)
    {
        rotation = newAngle;
    }
}
