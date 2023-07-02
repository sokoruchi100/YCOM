using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.CanvasScaler;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedEventChanged;
    [SerializeField] private LayerMask unitsLayerMask;
    [SerializeField] private Unit selectedUnit;
    private bool isBusy;
    private BaseAction selectedAction;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        SetSelectedUnit(selectedUnit);
    }

    private void Update() {
        if (isBusy) { return; }
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if (TryHandleUnitSelection()) { return; }
        HandleSelectedAction();
    }

    private void HandleSelectedAction() {
        if (Input.GetMouseButtonDown(0)) {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition)) {
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            }
        }
    }

    private bool TryHandleUnitSelection() {
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitsLayerMask)) {
                if (hitInfo.transform.TryGetComponent<Unit>(out Unit unit)) {
                    if (unit == selectedUnit) { return false; }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit) {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        OnSelectedEventChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction) {
        selectedAction = baseAction;
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }

    private void SetBusy() {
        isBusy = true;
    }
    private void ClearBusy() {
        isBusy = false;
    }
    public BaseAction GetSelectedAction() {
        return selectedAction;
    }
}
