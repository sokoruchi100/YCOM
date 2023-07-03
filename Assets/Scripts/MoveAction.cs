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
    
    private Vector3 targetPosition;

    protected override void Awake() {
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update() {
        if (!isActive) { return; }
        float stoppingDistance = 0.1f;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance) {            

            float moveSpeed = 4;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        } else {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        float rotateSpeed = 10;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        ActionStart(OnActionCompleted);
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition currentGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveGrid; x <= maxMoveGrid; x++) {
            for (int z = -maxMoveGrid; z <= maxMoveGrid; z++) {
                GridPosition offsetGridPosition = new GridPosition(x,z);
                GridPosition newGridPosition = currentGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                float testDistance = Mathf.Sqrt(x * x + z * z);
                if (testDistance > maxMoveGrid) { continue; }

                if (newGridPosition == currentGridPosition) { continue; }
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(newGridPosition)) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override string GetActionName() {
        return "Move";
    }
}
