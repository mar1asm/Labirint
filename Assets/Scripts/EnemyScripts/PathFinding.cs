using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding{

    bool[,] map;
    PathNode[,] nodeMap;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    int mapSize;
    const int Move_Straight_Cost = 10;
    const int Move_Diagonal_Cost = 14;
    public PathFinding(bool[,] m)
    {
        map = m;
        mapSize = (int)Mathf.Sqrt(map.Length);
        nodeMap = new PathNode[mapSize, mapSize];

    }



    public List<PathNode> FindPath(int startX, int startZ, int endX, int endZ)
    {
        nodeMap[startX, startZ] = new PathNode { xValue = startX, zValue = startZ };
        PathNode startNode = nodeMap[startX, startZ];
        nodeMap[endX, endZ] = new PathNode { xValue = endX, zValue = endZ };
        PathNode endNode = nodeMap[endX, endZ];

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateCost();

        for (int i=0; i<mapSize; i++)
        {
            for (int j=0; j<mapSize; j++)
            {
                nodeMap[i,j] = new PathNode { xValue = i, zValue = j, gCost = int.MaxValue, previousNode = null };
                nodeMap[i,j].CalculateCost();
            }
        }

        while (openList.Count > 0)
        {
            PathNode currentNode =GetLowestFCost(openList);
            if ( currentNode.xValue ==endNode.xValue && currentNode.zValue==endNode.zValue)
            {
                return CalculatePath(nodeMap[endX, endZ]);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach( PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                int newGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                if (newGCost < neighbourNode.gCost)
                {
                    neighbourNode.previousNode = currentNode;
                    neighbourNode.gCost = newGCost;
                    neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.CalculateCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
               
            }

        }
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.xValue - 1 >= 0&& map[currentNode.xValue-1, currentNode.zValue])
            neighbourList.Add(nodeMap[currentNode.xValue-1, currentNode.zValue]);
        if (currentNode.xValue + 1 < mapSize && map[currentNode.xValue + 1, currentNode.zValue])
            neighbourList.Add(nodeMap[currentNode.xValue + 1, currentNode.zValue]);
        if (currentNode.zValue >= 0 && map[currentNode.xValue, currentNode.zValue-1])
            neighbourList.Add(nodeMap[currentNode.xValue, currentNode.zValue-1]);
        if (currentNode.zValue < mapSize && map[currentNode.xValue, currentNode.zValue+1])
            neighbourList.Add(nodeMap[currentNode.xValue, currentNode.zValue+1]);

        return neighbourList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List < PathNode > path= new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.previousNode != null)
        {
            path.Add(currentNode.previousNode);
            currentNode = currentNode.previousNode;
        }

        path.Reverse();
        return path;
    }

    private int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.xValue - b.xValue);
        int yDistance = Mathf.Abs(a.zValue - b.zValue);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return Move_Diagonal_Cost * Mathf.Min(xDistance, yDistance) + Move_Straight_Cost * remaining;
    }

    private PathNode GetLowestFCost(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i=1; i< pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost< lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
