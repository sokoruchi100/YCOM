using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    public event EventHandler<OnChangedFloorsStartedEventArgs> OnChangedFloorsStarted;
    public class OnChangedFloorsStartedEventArgs : EventArgs {
        public GridPosition unitGridPosition;
        public GridPosition targetGridPosition;
    }
    
    [SerializeField] private int maxMoveGrid = 4;
    
    private List<Vector3> positionList;
    private int currentPositionIndex;
    private bool isChangingFloors;
    private float differentFloorsTeleportTimer;
    private float differentFloorsTeleportTimerMax = 0.6f;

    private void Update() {
        if (!isActive) { return; }

        Vector3 targetPosition = positionList[currentPositionIndex];
        if (isChangingFloors) {
            //Stop and Teleport Logic
            Vector3 targetSameFloorPosition = targetPosition;
            targetSameFloorPosition.y = transform.position.y;
            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;

            float rotateSpeed = 10;
            transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * rotateSpeed);

            differentFloorsTeleportTimer -= Time.deltaTime;
            if (differentFloorsTeleportTimer < 0f) {
                isChangingFloors = false;
                transform.position = targetPosition;
            }
        } else {
            //Regular Move Logic
            
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float rotateSpeed = 10;
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            float moveSpeed = 4;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        float stoppingDistance = 0.1f;
        if (Vector3.Distance(targetPosition, transform.position) < stoppingDistance) {            
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count) {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            } else {
                targetPosition = positionList[currentPositionIndex];
                GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
                if (targetGridPosition.floor != unitGridPosition.floor) {
                    //Different Floors
                    isChangingFloors = true;
                    differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;

                    OnChangedFloorsStarted?.Invoke(this, new OnChangedFloorsStartedEventArgs { 
                        unitGridPosition = unitGridPosition,
                        targetGridPosition = targetGridPosition,
                    });
                }
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
                for (int floor = -maxMoveGrid; floor <= maxMoveGrid; floor++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
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
