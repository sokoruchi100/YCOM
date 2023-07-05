using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour {
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

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
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }
    protected void ActionComplete() {
        isActive = false;
        OnActionCompleted();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit() {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction() {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        List<GridPosition> gridPositionList = GetValidGridPositionList();
        foreach (GridPosition gridPosition in gridPositionList) {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0) {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        } else {
            return null;
        }
        
    }

    protected abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}
