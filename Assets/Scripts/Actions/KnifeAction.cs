using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAction : BaseAction {
    public static event EventHandler OnAnyKnifeHit;
    public event EventHandler OnKnifeActionStarted;
    public event EventHandler OnKnifeActionCompleted;

    [SerializeField] private int maxKnifeRange = 2;
    [SerializeField] private int knifeDamage = 40;

    private enum State {
        stabbingKnifeBeforeHit,
        stabbingKnifeAfterHit
    }

    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Update() {
        if (!isActive) { return; }
        stateTimer -= Time.deltaTime;

        switch (state) {
            case State.stabbingKnifeBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10;
                transform.forward = Vector3.Slerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.stabbingKnifeAfterHit:
                break;
        }
        if (stateTimer <= 0) {
            NextState();
        }
    }

    private void NextState() {
        switch (state) {
            case State.stabbingKnifeBeforeHit:
                state = State.stabbingKnifeAfterHit;
                float afterHitStateTime = 1.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(knifeDamage);
                OnAnyKnifeHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.stabbingKnifeAfterHit:
                OnKnifeActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName() {
        return "Knife";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxKnifeRange; x <= maxKnifeRange; x++) {
            for (int z = -maxKnifeRange; z <= maxKnifeRange; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z,0);
                GridPosition newGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }
                
                float testDistance = Vector3.Distance(LevelGrid.Instance.GetWorldPosition(unitGridPosition), LevelGrid.Instance.GetWorldPosition(newGridPosition));
                if (testDistance > maxKnifeRange) { continue; }

                if (newGridPosition == unitGridPosition) { continue; }
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(newGridPosition)) { continue; }

                Unit otherUnit = LevelGrid.Instance.GetUnitAtGridPosition(newGridPosition);
                if (otherUnit.IsEnemy() == unit.IsEnemy()) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.stabbingKnifeBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;
        OnKnifeActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(OnActionCompleted);
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction { 
            gridPosition = gridPosition,
            actionValue = 200
        };
    }

    public int GetMaxKnifeRange() {
        return maxKnifeRange;
    }
    public override int GetActionPointsCost() {
        return 2;
    }

}
