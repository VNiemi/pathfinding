﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public bool isBlocked;
    public Vector3 position;

    public int GridX, GridY;

    public int GCost, HCost;

    public int FCost
    {
        get { return GCost + HCost; }
    }

    int heapIndex;

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    List<Node> _neighbours;
    static PathGridManager _pgman;

    public static void StorePathGrid(PathGridManager pgman)
    {
        _pgman = pgman;
    }

    public int CompareTo(Node node)
    {
        int iComp = FCost.CompareTo(node.FCost);
        if (iComp == 0)
        {
            iComp = FCost.CompareTo(node.HCost);
        }
        return ~iComp;
    }

    public Node Parent;

    public Node(bool blocked, Vector3 pos, int x, int y)
    {
        isBlocked = blocked;
        position = pos;

        GridX = x;
        GridY = y;
    }

    public List<Node> GetNeighbours()
    {
        if (_neighbours == null)
        {
            _neighbours = _pgman.GetNeighbours(this);
        }

        return _neighbours;
    }
}
