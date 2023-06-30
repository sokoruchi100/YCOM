using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject { 
    private GridSystem gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList = new List<Unit>();

    public GridObject(GridSystem gridSystem, GridPosition gridPosition) {
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
}
