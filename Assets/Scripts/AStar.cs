using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{

    // The transform the path is between.
    public Transform Seeker, Target;

    // The start and end nodes for the current path.
    private Node _start, _end;

    //NOTE : Obsolete comment and code.
    //Used this to do open and closed sets, do not remember why.
    //I am guessing that since it is only used here I did not want to put it elsewhere.
    //private OpenClosed[,] _openClosed;

    //enum OpenClosed
    //{
    //    idle = 0,
    //    open = 1,
    //    closed = 2
    //}

    private Heap<Node> _heap;


    PathGridManager PathGridManager;


    private void Awake()
    {
        // Gets a reference to pathgridmanager.
        PathGridManager = GetComponent<PathGridManager>();
    }

    private void Start()
    {
        // Initializing these after the pathgrid manager knows the dimensions.
        // _openClosed = new OpenClosed[PathGridManager.numX, PathGridManager.numY];
        _heap = new Heap<Node>((PathGridManager.numX + 1) * (PathGridManager.numY + 1));
    }


    // Update is called once per frame
    void Update()
    {

        // The path needs to be checked, probably not every frame really.
        FindPath(Seeker, Target);

    }

    /// <summary>
    /// Finds, builds and stores path between two positions on the grid.
    /// </summary>
    /// <param name="seeker">Seeker transform/object.</param>
    /// <param name="target">Target transform/object.</param>
    private void FindPath(Transform seeker, Transform target)
    {

        // The nodes the algorithm uses.
        Node start, end, current;

        // Get grid nodes for current end points.
        start = PathGridManager.GetNode(seeker.position);
        end = PathGridManager.GetNode(target.position);

        // No change in end points, current path still valid, skip pathfinding.
        // The part you need to comment if you want to run the profiler.
        if (start == _start && end == _end) return;

        // Update the stored end points.
        _start = start;
        _end = end;

        // Clear heap and the open and closed sets.
        ClearSets();

        // Add first node to open set.
        AddToOpen(start);

        // Loop until all possible paths toward target have been tried.
        while (_heap.Count > 0)
        {
            current = _heap.RemoveFirst();
            CloseNode(current);

            if (current == _end)
            {
                MakePath();
                return;
            }

            foreach (Node next in current.GetNeighbours())
            {
                // Already handled, note that GetNeighbours already did the block check and cached result.
                if (IsClosed(next)) continue;

                int cost = current.GCost + GetDistance(current, _end);

                if (cost < next.GCost || !IsOpen(next))
                {
                    next.GCost = cost;
                    next.HCost = GetDistance(next, _end);
                    next.Parent = current;

                    if (!IsOpen(next))
                    {
                        AddToOpen(next);
                    }
                }
            }
        }
    }

    #region This contains the closed and open sets replacement functions.

    private bool IsOpen(Node node)
    {
        return node.state == Node.State.isOpen;
    }

    private bool IsClosed(Node node)
    {
        return node.state == Node.State.isClosed;
    }

    private void AddToOpen(Node node)
    {
        _heap.Add(node);
        node.state = Node.State.isOpen;
    }

    private void CloseNode(Node node)
    {
        node.state = Node.State.isClosed;
    }

#endregion

    private void ClearSets()
    {
        // Clears the heap.
        _heap.Clear();

        // Clear the open and closed sets.
        // While moving these to Node seemed nice, it means this has to happen via Grid.
        PathGridManager.ClearOpenClosed();
    }

    /// <summary>
    /// Builds and stores the path for pathgridmanager to show.
    /// </summary>
    private void MakePath()
    {
        PathGridManager.path.Clear();
        Node current = _end;

        while (current != _start)
        {
            PathGridManager.path.Add(current);
            current = current.Parent;
        }
        PathGridManager.path.Reverse();
    }

    /// <summary>
    /// Function to get the Manhattan distance between two nodes.
    /// </summary>
    /// <param name="a">First Node</param>
    /// <param name="b">Second Node</param>
    /// <returns>Manhattan distance between nodes.</returns>
    public int GetDistance(Node a, Node b)
    {
        int DistX = Math.Abs(a.GridX - b.GridX);
        int DistY = Math.Abs(a.GridY - b.GridY);

        if (DistX > DistY)
        {
            return 14 * DistX + 10 * (DistY - DistX);
        }
        else
        {
            return 14 * DistY + 10 * (DistX - DistY);
        }
    }
}
