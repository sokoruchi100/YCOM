using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }
    
    private const int MOVE_STRAIGHT_COST = 10;
    
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private Transform pathfindingLinkContainer;

    private int width;
    private int height;
    private float cellSize;
    private int floorAmount;
    private List<GridSystemHex<PathNode>> gridSystemList;
    private List<PathfindingLink> pathfindingLinkList;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize, int floorAmount) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floorAmount = floorAmount;

        gridSystemList = new List<GridSystemHex<PathNode>>();
        for (int floor = 0; floor < floorAmount; floor++) {
            GridSystemHex<PathNode> gridSystem = new GridSystemHex<PathNode>(width, height, cellSize, floor, LevelGrid.FLOOR_HEIGHT, (GridSystemHex<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            gridSystemList.Add(gridSystem);
        }

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                for (int floor = 0; floor < floorAmount; floor++) {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    float raycastOffsetDistance = 1f;
                    GetNode(x,z,floor).SetIsWalkable(false);
                    if (Physics.Raycast(
                        worldPosition + Vector3.up * raycastOffsetDistance,
                        Vector3.down,
                        raycastOffsetDistance * 2,
                        floorLayerMask)) {
                        GetNode(x, z, floor).SetIsWalkable(true);
                    }
                    if (Physics.Raycast(
                        worldPosition + Vector3.down * raycastOffsetDistance,
                        Vector3.up,
                        raycastOffsetDistance * 2,
                        obstaclesLayerMask)) {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                }
            }
        }

        pathfindingLinkList = new List<PathfindingLink>();
        foreach (Transform pathfindingLinkTransform in pathfindingLinkContainer) {
            if (pathfindingLinkTransform.TryGetComponent<PathfindingLinkMonoBehaviour>(out PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour)) {
                pathfindingLinkList.Add(pathfindingLinkMonoBehaviour.GetPathfindingLink());
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength) { 
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                for (int floor = 0; floor < floorAmount; floor++) {
                    //Creating PathNodes
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

                    //Initializing the PathNodes
                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();
                }
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateHeuristicDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostPathNode(openList);
            if (currentNode == endNode) {
                //Reached Final Node
                pathLength = endNode.GetGCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighborList(currentNode)) {
                if (closedList.Contains(neighborNode)) {
                    continue;
                }

                if (!neighborNode.IsWalkable()) { 
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + MOVE_STRAIGHT_COST;
                if (tentativeGCost < neighborNode.GetGCost()) { 
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(CalculateHeuristicDistance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode)) {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        //No Path Found
        pathLength = 0;
        return null;
    }

    public int CalculateHeuristicDistance(GridPosition gridPositionA, GridPosition gridPositionB) {
        return Mathf.RoundToInt(MOVE_STRAIGHT_COST * 
            Vector3.Distance(GetGridSystem(gridPositionA.floor).GetWorldPosition(gridPositionA), GetGridSystem(gridPositionB.floor).GetWorldPosition(gridPositionB)));
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()) {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private GridSystemHex<PathNode> GetGridSystem(int floor) {
        return gridSystemList[floor];
    }

    private PathNode GetNode(int x, int z, int floor) {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
    }

    private List<PathNode> GetNeighborList(PathNode currentNode) { 
        List<PathNode> neighborList = new List<PathNode>();
        GridPosition gridPosition = currentNode.GetGridPosition();
        bool oddRow = gridPosition.z % 2 == 1;

        if (gridPosition.x - 1 >= 0) {
            //Left
            neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z, gridPosition.floor));
        }

        if (gridPosition.x + 1 < width) {
            //Right
            neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z, gridPosition.floor));
        }

        if (gridPosition.z - 1 >= 0) {
            //Down
            neighborList.Add(GetNode(gridPosition.x, gridPosition.z - 1, gridPosition.floor));
        }

        if (gridPosition.z + 1 < height) {
            //Up
            neighborList.Add(GetNode(gridPosition.x, gridPosition.z + 1, gridPosition.floor));
        }

        //Odd Row
        if (oddRow && gridPosition.x + 1 < width) {
            if (gridPosition.z - 1 >= 0) {
                //Down
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
            }

            if (gridPosition.z + 1 < height) {
                //Up
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
            }
            //Even Row
        } else if (!oddRow && gridPosition.x - 1 >= 0) {
            if (gridPosition.z - 1 >= 0) {
                //Down
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));
            }

            if (gridPosition.z + 1 < height) {
                //Up
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
            }
        }

        List<PathNode> totalNeighbourList = new List<PathNode>();
        totalNeighbourList.AddRange(neighborList);

        List<GridPosition> pathfindingLinkGridPositionList = GetPathfindingLinkConnectedGridPositionList(gridPosition);
        foreach (GridPosition linkGridPosition in pathfindingLinkGridPositionList) {
            totalNeighbourList.Add(GetNode(linkGridPosition.x, linkGridPosition.z, linkGridPosition.floor));
        }

        return totalNeighbourList;
    }

    private List<GridPosition> GetPathfindingLinkConnectedGridPositionList(GridPosition gridPosition) {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathfindingLink pathfindingLink in pathfindingLinkList) {
            if (pathfindingLink.gridPositionA == gridPosition) {
                gridPositionList.Add(pathfindingLink.gridPositionB);
            }
            if (pathfindingLink.gridPositionB == gridPosition) {
                gridPositionList.Add(pathfindingLink.gridPositionA);
            }
        }

        return gridPositionList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode) { 
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null) {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }
        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList) {
            gridPositionList.Add(pathNode.GetGridPosition());
        }
        return gridPositionList;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }
    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable) {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition) {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
