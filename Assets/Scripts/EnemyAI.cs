using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float timer;

    public enum State {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private State state;

    private void Awake() {
        state = State.WaitingForEnemyTurn;
    }

    private void Start() {
        TurnSystem.Instance.OnTurnChanged += Instance_OnTurnChanged;
    }

    private void Instance_OnTurnChanged(object sender, System.EventArgs e) {
        if (!TurnSystem.Instance.IsPlayerTurn()) {
            timer = 1;
            state = State.TakingTurn;
        }
        
    }

    private void Update() {
        if (TurnSystem.Instance.IsPlayerTurn()) { return; }
        
        switch (state) {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn)) {
                        state = State.Busy;
                    } else {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private bool TryTakeEnemyAIAction(Action callbackAction) {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) {
            if (TryTakeEnemyAIAction(enemyUnit, callbackAction)) {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action callbackAction) {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray()) {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) { continue; }

            if (bestEnemyAIAction == null) {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            } else {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPoints(bestBaseAction)) {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, callbackAction);
            return true;
        } else {
            return false;
        }
    }

    private void SetStateTakingTurn() {
        timer = 0.5f;
        state = State.TakingTurn;
    }
}
