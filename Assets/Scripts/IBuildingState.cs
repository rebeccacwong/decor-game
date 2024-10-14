using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnClickAction(Vector3Int gridPosition);
    void OnEscapeAction();
    void UpdateState(Vector3Int gridPosition);
}