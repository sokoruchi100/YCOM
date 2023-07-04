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

    public event EventHandler OnSelectedUnitEventChanged;
    public event EventHandler OnSelectedActionEventChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

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
        if (!TurnSystem.Instance.IsPlayerTurn()) { return; }
        if (TryHandleUnitSelection()) { return; }
        HandleSelectedAction();
    }

    private void HandleSelectedAction() {
        if (Input.GetMouseButtonDown(0)) {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) { return; }
            if (!selectedUnit.TrySpendActionPoints(selectedAction)) { return; }

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool TryHandleUnitSelection() {
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitsLayerMask)) {
                if (hitInfo.transform.TryGetComponent<Unit>(out Unit unit)) {
                    if (unit == selectedUnit) { return false; }
                    if (unit.IsEnemy()) { return false; }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit) {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitEventChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction) {
        selectedAction = baseAction;
        OnSelectedActionEventChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }

    private void SetBusy() {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }
    private void ClearBusy() {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }
    public BaseAction GetSelectedAction() {
        return selectedAction;
    }
}
