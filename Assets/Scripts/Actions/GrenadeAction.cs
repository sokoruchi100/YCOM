using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction {
    [SerializeField] private int maxThrowRange = 5;
    [SerializeField] private int grenadeDamage = 60;
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private void Update() {
        if (!isActive) {
            return;
        }
    }

    public override string GetActionName() {
        return "Grenade";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowRange; x <= maxThrowRange; x++) {
            for (int z = -maxThrowRange; z <= maxThrowRange; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition newGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                float testDistance = Mathf.Sqrt(x * x + z * z);
                if (testDistance > maxThrowRange) { continue; }
                
                if (newGridPosition == unitGridPosition) { continue; }

                Vector3 unitWorldPosition = unit.GetWorldPosition();
                Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(newGridPosition);
                Vector3 throwDir = (targetWorldPosition - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    throwDir,
                    Vector3.Distance(unitWorldPosition, targetWorldPosition),
                    obstaclesLayerMask)) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, grenadeDamage, OnGrenadeBehaviourComplete);

        ActionStart(OnActionCompleted);
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    private void OnGrenadeBehaviourComplete() {
        ActionComplete();
    }
    public override int GetActionPointsCost() {
        return 3;
    }
}
