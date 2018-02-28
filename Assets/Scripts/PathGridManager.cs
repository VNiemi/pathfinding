using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGridManager : MonoBehaviour
{

    public LayerMask ObstacleMask;
    public Vector2 GridSize;
    public float HalfNodeWidth;

    public int numX, numY;

    float startX, startY;

    CapsuleCollider capsule;

    public List<Node> path = new List<Node>();

    Node[,] Grid;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(GridSize.x, 0, GridSize.y));

        Vector3 size = new Vector3(HalfNodeWidth * 1.8f, HalfNodeWidth * 1.8f, HalfNodeWidth * 2);

        // ObjectNode = GetNodeFromPosition(vPos);

        if (Grid == null) return;
        Node capsuleNode = GetNode(capsule.gameObject.transform.position);
        //List<Node> neighbours = GetNeighbours(capsuleNode);

        foreach (var node in Grid)
        {

            Gizmos.color = (node.state == Node.State.isBlocked) ? Color.red : Color.white;

            // Blue if capsule in the node.
            if (node == capsuleNode) Gizmos.color = Color.blue;

            // Cyan if capsule in neighbouring node.
            // if (neighbours.Contains(node)) Gizmos.color = Color.cyan;

            Gizmos.DrawWireCube(node.position, size);

        }

        // Draw the path
        Gizmos.color = Color.cyan;
        foreach (var node in path)
        {
            Gizmos.DrawWireCube(node.position, size);
        }


    }

    public Node GetNode(Vector3 position)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp((startX - position.x) / (HalfNodeWidth * 2), 0, numX - HalfNodeWidth * 2));
        int y = Mathf.RoundToInt(Mathf.Clamp((startY - position.z) / (HalfNodeWidth * 2), 0, numY - HalfNodeWidth * 2));

        return Grid[x, y];
    }

    private void Awake()
    {

        numX = (int)(GridSize.x / (HalfNodeWidth * 2));
        numY = (int)(GridSize.y / (HalfNodeWidth * 2));

        Grid = new Node[numX, numY];


        startX = numX * HalfNodeWidth - HalfNodeWidth;
        startY = numY * HalfNodeWidth - HalfNodeWidth;

        bool check;
        Vector3 position = new Vector3(0, HalfNodeWidth, 0);

        for (int x = 0; x < numX; x++)
        {
            position.x = startX - x * HalfNodeWidth * 2;

            for (int y = 0; y < numY; y++)
            {
                position.z = startY - y * HalfNodeWidth * 2;
                check = Physics.CheckSphere(position, HalfNodeWidth, ObstacleMask);

                Grid[x, y] = new Node(check, position, x, y);
            }
        }

        capsule = FindObjectOfType<CapsuleCollider>();

        Node.StorePathGrid(this);

    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> result = new List<Node>();

        for (int x = node.GridX - 1; x <= node.GridX + 1; x++)
        {
            // Skip nodes outside the grid.
            if (x < 0 || x >= numX) continue;

            for (int y = node.GridY - 1; y <= node.GridY + 1; y++)
            {
                // Skip nodes outside the grid.
                if (y < 0 || y >= numY) continue;

                // Skip the node itself.
                if (x == node.GridX && y == node.GridY) continue;

                // Doing obstacle check  here because why not?
                // In theory saves some resources as this runs once per node.
                // Would cause an issue if we checked obstacles dynamically,
                // but we do not. Probably should but...
                if (Grid[x, y].state != Node.State.isBlocked) result.Add(Grid[x, y]);
            }
        }

        return result;
    }

    public void ClearOpenClosed()
    {

        Node node;

        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                node = Grid[x, y];

                // In retrospect this is not a good way to do this. The separate array or an actual list would be faster.
                // But fixing this now would probably qualify as unnecessary optimization.
                if (node.state == Node.State.isOpen || node.state == Node.State.isClosed)
                {
                    node.state = Node.State.isReady;
                }
            }
        }

    }

}
