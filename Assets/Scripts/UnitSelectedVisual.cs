using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;
    private MeshRenderer meshRenderer;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitEventChanged += Instance_OnSelectedEventChanged;
        UpdateVisuals();
    }

    private void Instance_OnSelectedEventChanged(object sender, System.EventArgs e) {
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        if (unit == UnitActionSystem.Instance.GetSelectedUnit()) {
            meshRenderer.enabled = true;
        } else {
            meshRenderer.enabled = false;
        }
    }

    private void OnDestroy() {
        UnitActionSystem.Instance.OnSelectedUnitEventChanged -= Instance_OnSelectedEventChanged;
    }
}
