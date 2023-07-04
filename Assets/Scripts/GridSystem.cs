using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjects;
    private Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject;

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.createGridObject = createGridObject;
        gridObjects = new TGridObject[width,height];

        CreateGrid();
    }

    private void CreateGrid() {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjects[x,z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
            );
    }

    public void CreateDebugPrefabs(Transform debugPrefab) {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition position = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(position), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(position) as GridObject);
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition) {
        return gridObjects[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition) {
        return gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}
