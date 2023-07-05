using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction {

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State {
        Aiming,
        Shooting,
        Cooloff
    }

    [SerializeField] private int maxShootRange = 7;
    [SerializeField] private int damageAmount = 40;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    private void Update() {
        if (!isActive) { return; }

        stateTimer -= Time.deltaTime;

        switch (state) {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10;
                transform.forward = Vector3.Slerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShootBullet) {
                    canShootBullet = false;
                    Shoot();
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0) {
            NextState();
        }
    }

    private void Shoot() {
        targetUnit.Damage(damageAmount);
        OnAnyShoot?.Invoke(this, new OnShootEventArgs {
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        OnShoot?.Invoke(this, new OnShootEventArgs { 
            targetUnit = targetUnit,
            shootingUnit = unit
        });
    }

    private void NextState() {
        switch (state) {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = 0.1f;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = 0.5f;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName() {
        return "Shoot";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition gridPosition = unit.GetGridPosition();
        return GetValidGridPositionList(gridPosition);
    }
    public List<GridPosition> GetValidGridPositionList(GridPosition gridPosition) {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootRange; x <= maxShootRange; x++) {
            for (int z = -maxShootRange; z <= maxShootRange; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition newGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                float testDistance = Mathf.Sqrt(x*x + z*z);
                if (testDistance > maxShootRange) { continue; }

                if (newGridPosition == gridPosition) { continue; }
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(newGridPosition)) { continue; }

                Unit otherUnit = LevelGrid.Instance.GetUnitAtGridPosition(newGridPosition);
                if (otherUnit.IsEnemy() == unit.IsEnemy()) { continue; }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                Vector3 shootDir = (otherUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, otherUnit.GetWorldPosition()),
                    obstaclesLayerMask)) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        state = State.Aiming;
        stateTimer = 1f;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        canShootBullet = true;
        ActionStart(OnActionCompleted);
    }

    public Unit GetTargetUnit() { 
        return targetUnit; 
    }

    public int GetMaxShootRange() {
        return maxShootRange;
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetNormalizedHealth()) * 100)
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) {
        return GetValidGridPositionList(gridPosition).Count;
    }
}
