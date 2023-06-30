using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro gridText;
    private GridObject gridObject;

    public void SetGridObject(GridObject gridObject) {
        this.gridObject = gridObject;
    }

    private void Update() {
        UpdateVisual();
    }

    private void UpdateVisual() {
        gridText.text = gridObject.ToString();
    }
}
