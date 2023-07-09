using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemHex<TGridObject>
{
    private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;
    private int width;
    private int height;
    private float cellSize;
    private int floor;
    private float floorHeight;
    private TGridObject[,] gridObjects;
    private Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject;

    public GridSystemHex(int width, int height, float cellSize, int floor, float floorHeight, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floor = floor;
        this.floorHeight = floorHeight;
        this.createGridObject = createGridObject;
        gridObjects = new TGridObject[width,height];

        CreateGrid();
    }

    private void CreateGrid() {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition gridPosition = new GridPosition(x, z, floor);
                gridObjects[x,z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) {
        return new Vector3(gridPosition.x, 0, 0) * cellSize + 
            new Vector3(0,0,gridPosition.z) * cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER +
            (gridPosition.z % 2 == 1 ? new Vector3(1,0,0) * cellSize * .5f : Vector3.zero) +
            new Vector3(0, gridPosition.floor, 0) * floorHeight;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        GridPosition roughXZ = new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize / HEX_VERTICAL_OFFSET_MULTIPLIER),
            floor
        );

        bool oddRow = roughXZ.z % 2 == 1;

        List<GridPosition> neighbourGridPositionList = new List<GridPosition> { 
            roughXZ + new GridPosition(-1, 0, floor),
            roughXZ + new GridPosition(+1, 0, floor),

            roughXZ + new GridPosition(0, +1, floor),
            roughXZ + new GridPosition(0, -1, floor),

            roughXZ + new GridPosition(oddRow ? +1 : -1, +1, floor),
            roughXZ + new GridPosition(oddRow ? +1 : -1, -1, floor),
        };

        GridPosition closestGridPosition = roughXZ;
        foreach(GridPosition neighbourGridPosition in neighbourGridPositionList) {
            if (Vector3.Distance(GetWorldPosition(neighbourGridPosition), worldPosition) <
                Vector3.Distance(GetWorldPosition(closestGridPosition), worldPosition)) {
                closestGridPosition = neighbourGridPosition;
            }
        }
        return closestGridPosition;
    }

    public void CreateDebugPrefabs(Transform debugPrefab) {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition position = new GridPosition(x, z, floor);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(position), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(position));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition) {
        return gridObjects[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition) {
        return gridPosition.x >= 0 && 
            gridPosition.z >= 0 && 
            gridPosition.x < width && 
            gridPosition.z < height &&
            gridPosition.floor == floor;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}
