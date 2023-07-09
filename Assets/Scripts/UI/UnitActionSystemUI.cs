using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainer;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    private List<ActionButtonUI> actionButtonList;

    private void Start() {
        actionButtonList = new List<ActionButtonUI>();
        UnitActionSystem.Instance.OnSelectedUnitEventChanged += Instance_OnSelectedUnitEventChanged;
        UnitActionSystem.Instance.OnSelectedActionEventChanged += Instance_OnSelectedActionEventChanged;
        CreateUnitActionButton();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void Instance_OnSelectedActionEventChanged(object sender, System.EventArgs e) {
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void Instance_OnSelectedUnitEventChanged(object sender, System.EventArgs e) {
        CreateUnitActionButton();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void CreateUnitActionButton() {
        foreach (Transform actionButtonChild in actionButtonContainer) {
            Destroy(actionButtonChild.gameObject);
        }
        actionButtonList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray()) {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainer);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
            actionButtonList.Add(actionButtonUI);
        }
    }

    private void UpdateSelectedVisual() {
        foreach (ActionButtonUI actionButtonUI in actionButtonList) {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints() {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        actionPointsText.text = "Action Points Cost: " + selectedAction.GetActionPointsCost();
    }
}
