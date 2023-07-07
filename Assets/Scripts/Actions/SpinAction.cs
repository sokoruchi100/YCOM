using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float spinProgress;
    [SerializeField] private int healAmount = 40;

    private void Update() {
        if (!isActive) { return; }
        float spinAddAmount = 360 * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        spinProgress += spinAddAmount;
        if (spinProgress >= 360) {
            unit.GetHealthSystem().Heal(healAmount);
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        spinProgress = 0;
        ActionStart(OnActionCompleted);
    }

    public override string GetActionName() {
        return "Heal";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition currentGridPosition = unit.GetGridPosition();
        if (unit.GetNormalizedHealth() == 1) { return new List<GridPosition>(); } else { return new List<GridPosition> { currentGridPosition }; }
        
    }

    public override int GetActionPointsCost() {
        return 2;
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }
}
