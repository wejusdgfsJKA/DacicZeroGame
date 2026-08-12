using UnityEngine;
using System.Collections.Generic;
public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;
    
    public int gridX;
    public int gridY;

    public Node parent;


    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }
}
