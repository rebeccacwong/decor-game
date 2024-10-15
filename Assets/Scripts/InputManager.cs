using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    [SerializeField]
    private LayerMask gridLayerMask;

    // Last selected map position
    private Vector3 lastPosition;

    public event Action OnClicked, OnEscape, OnItemHold, OnDelete, OnRightClick;

    private float mouseHoldTime = 0f;
    private float minMouseHoldTimeForAction = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }
        if (Input.GetMouseButton(0))
        {
            if (mouseHoldTime != -1)
            {
                mouseHoldTime += Time.deltaTime;
                if (mouseHoldTime >= minMouseHoldTimeForAction)
                {
                    OnItemHold?.Invoke();
                    mouseHoldTime = -1;
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnRightClick?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
        {
            OnDelete?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseHoldTime = 0;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape?.Invoke();
        }
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, gridLayerMask))
        {
            lastPosition = hit.point;
        }
        // This can occur if we hit a point outside of the edit space
        return lastPosition;
    }

    /*
     * Returns a gameObject that is within the 'Editable' LayerMask.
     */
    public GameObject ReturnSelectedEditableObj()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Editable")))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
}
