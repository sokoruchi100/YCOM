using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    
    [SerializeField] private int maxMoveGrid = 4;
    
    private List<Vector3> positionList;
    private int currentPositionIndex;

    private void Update() {
        if (!isActive) { return; }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = 0.1f;
        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance) {            

            float moveSpeed = 4;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        } else {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count) {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList) {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(OnActionCompleted);
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition currentGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveGrid; x <= maxMoveGrid; x++) {
            for (int z = -maxMoveGrid; z <= maxMoveGrid; z++) {
                GridPosition offsetGridPosition = new GridPosition(x,z);
                GridPosition newGridPosition = currentGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                if (newGridPosition == currentGridPosition) { continue; }
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(newGridPosition)) { continue; }

                if (!Pathfinding.Instance.IsWalkableGridPosition(newGridPosition)) { continue; }
                if (!Pathfinding.Instance.HasPath(currentGridPosition, newGridPosition)) { continue; }

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(currentGridPosition, newGridPosition) > maxMoveGrid * pathfindingDistanceMultiplier) { continue; }
                validGridPositionList.Add(newGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override string GetActionName() {
        return "Move";
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
    }
}
