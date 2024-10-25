using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDatabaseSO furnitureDatabase;

    [SerializeField]
    private ObjectDatabaseSO buildDatabase;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private ObjectPlacer objPlacer;

    [SerializeField]
    private PreviewSystem preview;

    private GridData floorData, furnitureData;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private IBuildingState buildingState;

    private bool enableFurniturePlacement;
    private bool enableBuildPlacement;


    // Start is called before the first frame update
    void Start()
    {
        EndBuildState();
        floorData = new();
        furnitureData = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            // Only update preview if it's a new position
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    #region Build / Furniture mode controls
    public void EnableFurniturePlacement()
    {
        DisableBuildPlacement();

        enableFurniturePlacement = true;

        inputManager.OnItemHold += StartEditExistingStructureIfPossible;
        inputManager.OnRightClick += RotateStructure;
    }

    private void DisableFurniturePlacement()
    {
        enableFurniturePlacement = false;
        buildingState = null;

        inputManager.OnItemHold -= StartEditExistingStructureIfPossible;
        inputManager.OnRightClick -= RotateStructure;
    }

    public void EnableBuildPlacement()
    {
        DisableFurniturePlacement();
        enableBuildPlacement = true;
    }

    private void DisableBuildPlacement()
    {
        enableBuildPlacement = false;
        buildingState = null;
    }
    #endregion

    private void HandleClickAndWaitForDrag()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        if (buildingState == null)
        {
            return;
        }

        Debug.Log("Handle click and wait for drag");

        inputManager.OnMouseUp += HandleMouseUpThenEndBuildState;
        inputManager.OnClicked -= HandleClickAndWaitForDrag;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        buildingState.OnClickAction(gridPosition);
    }

    private void HandleMouseUpThenEndBuildState()
    {
        if (buildingState == null)
        {
            return;
        }

        Debug.Log("Handle mouse location then end build state");

        inputManager.OnMouseUp -= HandleMouseUpThenEndBuildState;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnMouseUpAction(gridPosition);

        EndBuildState();
    }

    private void HandleClickThenEndBuildState()
    {
        if (inputManager.IsPointerOverUI() || buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnClickAction(gridPosition);

        inputManager.OnClicked -= HandleClickThenEndBuildState;

        EndBuildState();
    }

    /*
     * Generic EndBuildState that works for all members of IBuildingState.
     */
    private void EndBuildState()
    {
        if (buildingState == null)
        {
            return;
        }
        gridVisualization.SetActive(false);
        buildingState.EndState();
        preview.StopShowingPreview();
        lastDetectedPosition = Vector3Int.zero;

        inputManager.OnEscape -= EndBuildState;
        inputManager.OnDelete += EndBuildState;

        buildingState = null;
    }

    public void StartFurniturePlacement(int ID, bool isFirstPlacement)
    {
        EndBuildState();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID,
            grid,
            preview,
            furnitureDatabase,
            floorData,
            furnitureData,
            objPlacer,
            isFirstPlacement);

        inputManager.OnClicked += HandleClickThenEndBuildState;
        inputManager.OnDelete += EndBuildState;
        inputManager.OnEscape += EndBuildState;
    }

    public void StartRoomPlacement(int roomId)
    {
        EndBuildState();
        gridVisualization.SetActive(true);

        buildingState = new RoomBuildState(roomId, grid, preview, buildDatabase, floorData, objPlacer);

        inputManager.OnClicked += HandleClickAndWaitForDrag;
        inputManager.OnDelete += EndBuildState;
        inputManager.OnEscape += EndBuildState;
    }

    #region EditingState methods
    /*
     * If the mouse has selected an editable object, 
     * handles the editing action. If nothing is selected,
     * this is a no-op.
     */
    private void StartEditExistingStructureIfPossible()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        GameObject structure = inputManager.ReturnSelectedEditableObj();
        if (structure == null)
        {
            Debug.Log("No editable structure found. Ignoring.");
            return;
        }

        inputManager.OnItemHold -= StartEditExistingStructureIfPossible;

        Debug.Log($"Start editing {structure}");
        gridVisualization.SetActive(true);

        ItemData itemData = structure.GetComponent<ItemData>();
        if (itemData == null)
        {
            throw new System.Exception($"Structure {structure} is missing the required ItemData script!");
        }

        Vector3 worldPos = structure.transform.position;
        buildingState = new EditingState(worldPos, grid, furnitureDatabase, preview, floorData, furnitureData, objPlacer, itemData);

        inputManager.OnClicked += ModifyExistingStructure;
        inputManager.OnEscape += CancelEditingExistingStructure;
        inputManager.OnDelete += EndBuildState;
    }

    /*
     * This gets called when a click is made when we are in EditingState.
     */
    private void ModifyExistingStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        Debug.Log("Modifying existing structure");

        if (buildingState == null)
        {
            return;
        }

        buildingState.OnClickAction(gridPosition);

        inputManager.OnClicked -= ModifyExistingStructure;
        inputManager.OnItemHold += StartEditExistingStructureIfPossible;
        inputManager.OnDelete -= EndBuildState;
        inputManager.OnEscape -= CancelEditingExistingStructure;

        EndBuildState();
    }

    /*
     * Cancels the edit, allowing the buildState to return to 
     * its original state prior to edit.
     */
    private void CancelEditingExistingStructure()
    {
        if (buildingState == null)
        {
            return;
        }
        buildingState.OnEscapeAction();

        inputManager.OnClicked -= ModifyExistingStructure;
        inputManager.OnItemHold += StartEditExistingStructureIfPossible;
        inputManager.OnDelete -= EndBuildState;
        inputManager.OnEscape -= CancelEditingExistingStructure;

        EndBuildState();
    }

    private void RotateStructure()
    {
        if (buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.Rotate90DegreesCCW();
        buildingState.UpdateState(gridPosition);
    }
    #endregion
}
