using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }
    [SerializeField] private Transform gridSystemVisualPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    private GridSystemVisualSingle[,] gridSystemVisualSingles;

    public enum GridVisualType { 
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }

    [Serializable]
    public struct GridVisualTypeMaterial {
        public GridVisualType gridVisualType;
        public Material material;
    }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        gridSystemVisualSingles = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingle = Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingles[x, z] = gridSystemVisualSingle.GetComponent<GridSystemVisualSingle>();
            }
        }
        HideAllGridPositions();
        UpdateGridVisual();
        UnitActionSystem.Instance.OnSelectedActionEventChanged += Instance_OnSelectedActionEventChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += Instance_OnAnyUnitMovedGridPosition;
        HealthSystem.OnAnyDead += HealthSystem_OnAnyDead;
    }

    private void HealthSystem_OnAnyDead(object sender, EventArgs e) {
        UpdateGridVisual();
    }

    private void Instance_OnAnyUnitMovedGridPosition(object sender, System.EventArgs e) {
        UpdateGridVisual();
    }

    private void Instance_OnSelectedActionEventChanged(object sender, System.EventArgs e) {
        UpdateGridVisual();
    }

    public void HideAllGridPositions() {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                gridSystemVisualSingles[x, z].Hide();
            }
        }
    }

    public void ShowGridPositions(List<GridPosition> gridPositionList, GridVisualType gridVisual) {
        foreach (GridPosition gridPosition in gridPositionList) {
            gridSystemVisualSingles[gridPosition.x, gridPosition.z].Show(GetMaterialFromGridVisual(gridVisual));
        }
    }

    private void UpdateGridVisual() {
        HideAllGridPositions();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch (selectedAction) {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootRange(), GridVisualType.RedSoft);
                break;
        }

        ShowGridPositions(selectedAction.GetValidGridPositionList(), gridVisualType);
    }

    private Material GetMaterialFromGridVisual(GridVisualType gridVisualType) {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList) {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.Log("Should not occur");
        return null;
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisual) {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++) {
            for (int z = -range; z <= range; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition newGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                float testDistance = Mathf.Sqrt(x * x + z * z);
                if (testDistance > range) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        ShowGridPositions(validGridPositionList, gridVisual);
    }
}
