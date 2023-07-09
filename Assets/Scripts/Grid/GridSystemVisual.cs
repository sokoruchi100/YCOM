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
    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;

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
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(), 
            LevelGrid.Instance.GetHeight(),
            LevelGrid.Instance.GetFloorAmount()
            ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++) {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Transform gridSystemVisualSingle = Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                    gridSystemVisualSingleArray[x, z, floor] = gridSystemVisualSingle.GetComponent<GridSystemVisualSingle>();
                }
            }
        }

        UnitActionSystem.Instance.OnSelectedActionEventChanged += Instance_OnSelectedActionEventChanged;
        HealthSystem.OnAnyDead += HealthSystem_OnAnyDead;
        UnitActionSystem.Instance.OnBusyChanged += Instance_OnBusyChanged;

        UpdateGridVisual();
    }

    private void Instance_OnBusyChanged(object sender, bool e) {
        UpdateGridVisual();
    }

    private void HealthSystem_OnAnyDead(object sender, EventArgs e) {
        UpdateGridVisual();
    }

    private void Instance_OnSelectedActionEventChanged(object sender, System.EventArgs e) {
        UpdateGridVisual();
    }

    public void HideAllGridPositions() {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++) {
                    gridSystemVisualSingleArray[x, z, floor].Hide();
                }
            }
        }
    }

    public void ShowGridPositions(List<GridPosition> gridPositionList, GridVisualType gridVisual) {
        foreach (GridPosition gridPosition in gridPositionList) {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor].Show(GetMaterialFromGridVisual(gridVisual));
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
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case KnifeAction knifeAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), knifeAction.GetMaxKnifeRange(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
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
        return null;
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisual) {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++) {
            for (int z = -range; z <= range; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition newGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        ShowGridPositions(validGridPositionList, gridVisual);
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisual) {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++) {
            for (int z = -range; z <= range; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition newGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                float testDistance = Vector3.Distance(LevelGrid.Instance.GetWorldPosition(gridPosition), LevelGrid.Instance.GetWorldPosition(newGridPosition));
                if (testDistance > range) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        ShowGridPositions(validGridPositionList, gridVisual);
    }
}
