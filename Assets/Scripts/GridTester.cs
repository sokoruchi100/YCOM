using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTester : MonoBehaviour
{
    [SerializeField] private Unit unit;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            GridSystemVisual.Instance.HideAllGridPositions();
            GridSystemVisual.Instance.ShowGridPositions(unit.GetMoveAction().GetValidGridPositionList());
        }
    }
}
