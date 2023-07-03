using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour {
    protected Unit unit;
    protected bool isActive;
    protected Action OnActionCompleted;

    protected virtual void Awake() {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action OnActionCompleted);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition) {
        List<GridPosition> gridPositionList = GetValidGridPositionList();
        return gridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidGridPositionList();

    public virtual int GetActionPointsCost() {
        return 1;
    }

    protected void ActionStart(Action OnActionCompleted) {
        isActive = true;
        this.OnActionCompleted = OnActionCompleted;
    }
    protected void ActionComplete() {
        isActive = false;
        OnActionCompleted();
    }
}
