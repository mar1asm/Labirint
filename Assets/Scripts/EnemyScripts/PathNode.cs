using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {
    public int xValue;
    public int zValue;
    public int gCost;
    public int hCost;
    public int fCost;
    public PathNode previousNode;

    public void CalculateCost()
    {
        fCost = gCost + hCost;
    }
}
