using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainer;

    private void Start() {
        CreateUnitActionButton();
        UnitActionSystem.Instance.OnSelectedEventChanged += Instance_OnSelectedEventChanged;
    }

    private void Instance_OnSelectedEventChanged(object sender, System.EventArgs e) {
        CreateUnitActionButton();
    }

    private void CreateUnitActionButton() {
        foreach (Transform actionButtonChild in actionButtonContainer) {
            Destroy(actionButtonChild.gameObject);
        }

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray()) {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainer);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
        }
    }
}
