using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

    public Transform Seeker, Target;

    private Node _start, _end;

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

        FindPath(Seeker, Target);
		
	}

    private void FindPath(Transform seeker, Transform target)
    {
        Node start, end, current;

        start = PathGridManager.GetNode(seeker.position);
        end = PathGridManager.GetNode(target.position);

        // No change, skip pathfinding.
       // if (start == _start && end == _end) return;

        _start = start;
        _end = end;

        ClearSets();
        

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

        // Turns out the code was calling array length function every iteration.
        int xL = _openClosed.GetLength(0);
        int yL = _openClosed.GetLength(1);

        for (int x = 0; x<xL; x++)
        {
            for (int y = 0; y < yL; y++)
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
