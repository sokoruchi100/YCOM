using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }
    [SerializeField] private Transform gridSystemVisualPrefab;
    private GridSystemVisualSingle[,] gridSystemVisualSingles;

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
    }

    private void Update() {
        UpdateGridVisual();
    }

    public void HideAllGridPositions() {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                gridSystemVisualSingles[x, z].Hide();
            }
        }
    }

    public void ShowGridPositions(List<GridPosition> gridPositionList) {
        foreach (GridPosition gridPosition in gridPositionList) {
            gridSystemVisualSingles[gridPosition.x, gridPosition.z].Show();
        }
    }

    private void UpdateGridVisual() {
        HideAllGridPositions();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        ShowGridPositions(selectedAction.GetValidGridPositionList());
    }
}
