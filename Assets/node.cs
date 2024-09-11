using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class node
{
    private int[] pos;
    public Vector3 worldPos;

    private float g = 0;

    private float h = 0;
    
    public int state = 0;

    private node parent = null;




    public node(int[] position, Vector3 worldPos,int state_ = 0)
    {
        pos = position;
        this.worldPos = worldPos;
        state = state_;
    }

    public void reset(int[] position, Vector3 worldPos,int state_ = 0)
    {
     
        g = 0;
        h = 0;
        parent = null;
        pos = position;
        this.worldPos = worldPos;
        state = state_;
    }
    public float findF()
    {
        return g + h;
    }
    
    private float absDistance(int[] position2)
    {
        float x = position2[0] - pos[0];
        float y = position2[1] - pos[1];
        float z = position2[2] - pos[2];

        return Convert.ToSingle(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2)));
    }

    private List<Vector3> traceBack()
    {
        node current = this;
        List<Vector3> path = new List<Vector3>();
        path.Add(current.worldPos);
        while (current != null)
        {
            path.Add(current.worldPos);
            current.state = 6;
            current = current.parent;
        }
        path.Reverse();
        
        return path;
        
    }

    public List<Vector3> discover(node[,,] grid, PriorityQueue<node> queue,node endNode)
    {
        int x = pos[0];
        int y = pos[1];
        int z = pos[2];
        List<Tuple<node, float>> adjacent = getAdjacentCells(grid, pos[0], pos[1], pos[2]);

        if (state != 4 && state != 5)
        {
            state = 2;
        }

        foreach (var node in adjacent)
        {
            if (node.Item1.state == 5)
            {
                List<Vector3> path = traceBack();
                return path;
            }

            if (node.Item1 != this && node.Item1.state == 0)
            {
                node.Item1.g = g + node.Item2;
                node.Item1.h = node.Item1.absDistance(endNode.pos);
                node.Item1.state = 1;
                node.Item1.parent = this;
                queue.Add(node.Item1);
            }
            else if (node.Item1 != this && node.Item1.state == 1 && node.Item1.parent.g >= g)
            {
                node.Item1.g = g + node.Item2;
                node.Item1.parent = this;
            }
        }

        return null;

    }
    private List<Tuple<node,float>> getAdjacentCells(node[,,] grid,int x, int y, int z)
    {
        List<Tuple<node, float>> cells = new List<Tuple<node, float>>();
        List<int[]> directions = new List<int[]>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (!(dx == 0 && dy == 0 && dz == 0))
                    {
                        directions.Add(new int[3] {dx, dy, dz});
                    }
                    
                }
            }
        }

        foreach (var dir in directions)
        {
            int newX = x + dir[0];
            int newY = y + dir[1];
            int newZ = z + dir[2];
            if (0 <= newX && newX < grid.GetLength(0) &&
                0 <= newY && newY < grid.GetLength(1) &&
                0 <= newZ && newZ < grid.GetLength(2))
            {
                float dis = Convert.ToSingle(Math.Sqrt(Math.Pow(dir[0], 2) + Math.Pow(dir[1], 2) + Math.Pow(dir[2], 2)));
                cells.Add(new Tuple<node, float>(grid[newX,newY,newZ],dis));
            }
        }

        return cells;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
