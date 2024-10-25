using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static UnityEditor.FilePathAttribute;

[DisallowMultipleComponent]
public class CameraSystem : MonoBehaviour
{
    #region Fixed Parameters
    private static float moveSpeed = 5f;
    private static float rotationSpeed = 55f;
    private static float fieldOfViewMax = 50f;
    private static float fieldOfViewMin = 20f;
    private static float zoomSpeed = 15f;
    #endregion

    private float targetFieldOfView = 25f;

    private bool dragPanMoveActive;
    private bool dragPanRotateActive;
    private Vector2 lastMousePosition;

    private CinemachineVirtualCamera activeVirtualCam;

    [SerializeField]
    private CinemachineVirtualCamera virtualCam3D;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamTopView;


    // Start is called before the first frame update
    void Start()
    {
        activeVirtualCam = virtualCam3D;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            inputDir.z += 1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            inputDir.z += -1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            inputDir.x += 1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            inputDir.x += -1f;
        }

        if (dragPanMoveActive)
        {
            inputDir = GetInputDirUpdatedForCameraPan(inputDir);
        }

        if (dragPanRotateActive)
        {
            HandleCameraRotation();
        }

        if (inputDir != Vector3.zero)
        {
            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            HandleCameraZoom();
        }
    }

    /*
     * Public camera methods used by input manager.
     */

    public void BeginPanCamera()
    {
        dragPanMoveActive = true;
        lastMousePosition = Input.mousePosition;
    }

    public void BeginRotateCamera()
    {
        dragPanRotateActive = true;
        lastMousePosition = Input.mousePosition;
    }

    public void EndCameraMouseDragIfNecessary()
    {
        dragPanMoveActive = false;
        dragPanRotateActive = false;
    }

    public void SwapCameraPerspective()
    {
        Debug.Log("Swapping camera");

        if (activeVirtualCam == virtualCam3D)
        {
            activeVirtualCam = virtualCamTopView;
            virtualCam3D.gameObject.SetActive(false);
            virtualCamTopView.gameObject.SetActive(true);
            virtualCamTopView.m_Lens = virtualCam3D.m_Lens;
        }
        else
        {
            activeVirtualCam = virtualCam3D;
            virtualCam3D.gameObject.SetActive(true);
            virtualCamTopView.gameObject.SetActive(false);
            virtualCam3D.m_Lens = virtualCamTopView.m_Lens;
        }
    }


    /*
     * Private methods
     */

    private Vector3 GetInputDirUpdatedForCameraPan(Vector3 inputDir)
    {
        Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;
        inputDir.x = mouseMovementDelta.x;
        inputDir.z = mouseMovementDelta.y;
        lastMousePosition = (Vector2)Input.mousePosition;

        inputDir.Normalize();
        inputDir *= 1.5f;

        return inputDir;
    }

    private void HandleCameraRotation()
    {
        Vector2 currMousePos = (Vector2)Input.mousePosition;
        float mouseDiff = Mathf.Abs(currMousePos.x - lastMousePosition.x);
        int mouseDiffThreshold = 2;

        if (mouseDiff <= mouseDiffThreshold)
        {
            return;
        }
        float rotation = rotationSpeed * Time.deltaTime;

        if (currMousePos.x > lastMousePosition.x)
        {
            // swipe left to right
            rotation *= -1;
        }
        transform.Rotate(0, rotation, 0);
        lastMousePosition = currMousePos;
    }


    private void HandleCameraZoom()
    {
        float fieldOfViewChangePerScroll = 5;
        if (Input.mouseScrollDelta.y > 0)
        {
            // zoom in
            targetFieldOfView += fieldOfViewChangePerScroll;
        }
        else
        {
            targetFieldOfView -= fieldOfViewChangePerScroll;
        }

        targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);
        activeVirtualCam.m_Lens.FieldOfView = Mathf.Lerp(activeVirtualCam.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
    }
}
