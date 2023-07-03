using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    private void Start() {
        UnitActionSystem.Instance.OnBusyChanged += Instance_OnBusyChanged;
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
    private void Show() {
        gameObject.SetActive(true);
    }

    private void Instance_OnBusyChanged(object sender, bool isBusy) {
        if (isBusy) {
            Show();
        } else {
            Hide();
        }
    }
}
