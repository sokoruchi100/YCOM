using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float spinProgress;
    
    private void Update() {
        if (!isActive) { return; }
        float spinAddAmount = 360 * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        spinProgress += spinAddAmount;
        if (spinProgress >= 360) {
            isActive = false;
            OnActionCompleted();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        this.OnActionCompleted = OnActionCompleted;
        spinProgress = 0;
        isActive = true;
    }

    public override string GetActionName() {
        return "Spin";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition currentGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { currentGridPosition };
    }
}
