using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float timer;

    private void Start() {
        TurnSystem.Instance.OnTurnChanged += Instance_OnTurnChanged;
    }

    private void Instance_OnTurnChanged(object sender, System.EventArgs e) {
        timer = 2;
    }

    private void Update() {
        if (TurnSystem.Instance.IsPlayerTurn()) { return; }
        timer -= Time.deltaTime;
        if (timer <= 0) {
            TurnSystem.Instance.NextTurn();
        }
    }
}
