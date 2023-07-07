using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction {
    [SerializeField] private int maxInteractRange = 1;

    private void Update() {
        if (!isActive) { return; }
    }

    public override string GetActionName() {
        return "Interact";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractRange; x <= maxInteractRange; x++) {
            for (int z = -maxInteractRange; z <= maxInteractRange; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition newGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(newGridPosition)) { continue; }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(newGridPosition);
                if (interactable == null) { continue; }

                validGridPositionList.Add(newGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionCompleted) {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractComplete);
        ActionStart(OnActionCompleted);
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }
    private void OnInteractComplete() {
        ActionComplete();
    }
}
