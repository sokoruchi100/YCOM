using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public const float FLOOR_HEIGHT = 3f;
    public event EventHandler OnAnyUnitMovedGridPosition;
    public static LevelGrid Instance { get; private set; }
    
    [SerializeField] private Transform debugPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private int floorAmount;
    
    private List<GridSystemHex<GridObject>> gridSystemList;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystemList = new List<GridSystemHex<GridObject>>();

        for (int floor = 0; floor < floorAmount; floor++) {
            GridSystemHex<GridObject> gridSystem = new GridSystemHex<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT,
            (GridSystemHex<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
            //gridSystem.CreateDebugPrefabs(debugPrefab);

            gridSystemList.Add(gridSystem);
        }
    }

    private void Start() {
        Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
    }

    private GridSystemHex<GridObject> GetGridSystem(int floor) {
        return gridSystemList[floor];
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetUnitInGrid();
    }

    public int GetFloor(Vector3 worldPosition) {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);
    public GridPosition GetGridPosition(Vector3 worldPosition) => GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) {
        if (gridPosition.floor < 0 || gridPosition.floor >= floorAmount) { return false; }
        else { return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition); }
    }
    public int GetWidth() => GetGridSystem(0).GetWidth();
    public int GetHeight() => GetGridSystem(0).GetHeight();
    public int GetFloorAmount() => floorAmount;
    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition) { return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetInteractable(); }
    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) { GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetInteractable(interactable); }
}
