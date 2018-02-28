using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{

    //public bool isBlocked;
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

    // Replaced the blocked bool with a state that holds all possible and mutually exclusive values a node might have.
    // A separate array would actually be sligthly faster as open and closed sets need to be cleared with a loop.
    public enum State
    {
        isReady,
        isBlocked,
        isOpen,
        isClosed
    }

    public State state;

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
        state = blocked ? State.isBlocked : State.isReady;
        position = pos;

        GridX = x;
        GridY = y;
    }

    /// <summary>
    /// Function to return neighbours of the node. Results are cached in node.
    /// </summary>
    /// <returns>List of neighbouring nodes.</returns>
    public List<Node> GetNeighbours()
    {
        if (_neighbours == null)
        {
            _neighbours = _pgman.GetNeighbours(this);
        }

        return _neighbours;
    }
}
