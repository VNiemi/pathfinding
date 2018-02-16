using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

    // The transform the path is between.
    public Transform Seeker, Target;

    // The start and end nodes for the current path.
    private Node _start, _end;

    // Used this to do open and closed sets, do not remember why.
    // I am guessing that since it is only used here I did not want to put it elsewhere.
    private OpenClosed[,] _openClosed;

    enum OpenClosed
    {
        idle = 0,
        open = 1,
        closed = 2
    }

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
        _openClosed = new OpenClosed[PathGridManager.numX, PathGridManager.numY];
        _heap = new Heap<Node>((PathGridManager.numX+1) * (PathGridManager.numY+1));
    }


    // Update is called once per frame
    void Update () {

        // The path needs to be checked, probably not every frame really.
        FindPath(Seeker, Target);
		
	}

    private void FindPath(Transform seeker, Transform target)
    {

        // The nodes the algorithm uses.
        Node start, end, current;

        // Get grid nodes for current end points.
        start = PathGridManager.GetNode(seeker.position);
        end = PathGridManager.GetNode(target.position);

        // No change in end points, current path still valid, skip pathfinding.
       if (start == _start && end == _end) return;

       // Update the stored end points.
        _start = start;
        _end = end;

        // Clear heap and the open and closed sets.
        ClearSets();
        
        // Add first node to open set.
        AddToOpen(start);

        while(_heap.Count > 0)
        {
            current = _heap.RemoveFirst();
            CloseNode(current);

            if(current == _end)
            {
                MakePath();
                return;
            }
            
            foreach(Node next in current.GetNeighbours())
            {
                // Already handled.
                if (IsClosed(next)) continue;

                int cost = current.GCost + GetDistance(current, _end);

                if(cost<next.GCost || !IsOpen(next))
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

    private bool IsOpen(Node node)
    {
        return _openClosed[node.GridX, node.GridY] == OpenClosed.open;
    }

    private bool IsClosed(Node node)
    {
        return _openClosed[node.GridX, node.GridY] == OpenClosed.closed;
    }

    private void AddToOpen(Node node)
    {
        _heap.Add(node);
        _openClosed[node.GridX, node.GridY] = OpenClosed.open;
    }

    private void ClearSets()
    {
        // Clears the heap
        _heap.Clear();

        // Clears the open closed data, might be faster to just make a new one.
        // But it would create random garbage collect pauses eventually.
        for (int x = 0; x< PathGridManager.numX; x++)
        {
            for (int y = 0; y < PathGridManager.numY; y++)
            {
                _openClosed[x, y] = OpenClosed.idle;
            }
        }
        
    }

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

    private void CloseNode(Node node)
    {
        _openClosed[node.GridX, node.GridY] = OpenClosed.closed;
    }


 

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
