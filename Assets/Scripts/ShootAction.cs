using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction {
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    [SerializeField] private int maxShootRange = 7;
    private enum State {
        Aiming,
        Shooting,
        Cooloff
    }

    [SerializeField] private int damageAmount = 40;
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
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition currentGridPosition = unit.GetGridPosition();

        for (int x = -maxShootRange; x <= maxShootRange; x++) {
            for (int z = -maxShootRange; z <= maxShootRange; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition newGridPosition = currentGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                float testDistance = Mathf.Sqrt(x*x + z*z);
                if (testDistance > maxShootRange) { continue; }

                if (newGridPosition == currentGridPosition) { continue; }
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(newGridPosition)) { continue; }

                Unit otherUnit = LevelGrid.Instance.GetUnitAtGridPosition(newGridPosition);
                if (otherUnit.IsEnemy() == unit.IsEnemy()) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        ActionStart(OnActionCompleted);
        state = State.Aiming;
        stateTimer = 1f;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        canShootBullet = true;
    }
}
