using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject { 
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList = new List<Unit>();
    private IInteractable interactable;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition) {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }

    public override string ToString() {
        string debugString = gridPosition.ToString();

        foreach (Unit unit in unitList) {
            debugString += "\n" + unit;
        }

        return debugString;
    }

    public List<Unit> GetUnitList() {
        return unitList;
    }
    public void AddUnit(Unit unit) {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        unitList.Remove(unit);
    }

    public bool HasAnyUnit() {
        return unitList.Count > 0;
    }
    public Unit GetUnitInGrid() {
        if (unitList.Count > 0) {
            return unitList[0];
        } else {
            return null;
        }
    }

    public IInteractable GetInteractable() { return interactable; }
    public void SetInteractable(IInteractable interactable) { this.interactable = interactable; }
}
