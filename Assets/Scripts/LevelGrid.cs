using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class LevelGrid : MonoBehaviour
{
    public event EventHandler OnAnyUnitMovedGridPosition;
    public static LevelGrid Instance { get; private set; }
    
    [SerializeField] private Transform debugPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    
    private GridSystem<GridObject> gridSystem;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, 
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.CreateDebugPrefabs(debugPrefab);
    }

    private void Start() {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        gridSystem.GetGridObject(gridPosition).AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
        return gridSystem.GetGridObject(gridPosition).GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        gridSystem.GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition) {
        return gridSystem.GetGridObject(gridPosition).GetUnitInGrid();
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
}
