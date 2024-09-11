using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PathingObj : MonoBehaviour
{
    private PriorityQueue<node> queue;

    public Grid masterGrid;

    private node[,,] grid;

    public Transform target;
    private bool moving;
    public float speed = 50;
    public float rotationSpeed = 130;

    private List<Vector3> startPathfinding(Transform target)
    {
        if (grid == null)
        {
            grid = masterGrid.CreateGrid(masterGrid.gridSize, target);
        }
        masterGrid.resetGrid(grid);
        node endNode = findClosestNode(target.position);
        endNode.state = 5;
        node startNode = findClosestNode(transform.position);
        startNode.state = 4;
        queue = new PriorityQueue<node>((x, y) => x.findF().CompareTo(y.findF()));
        queue.Add(startNode);
        return runPathfinding(endNode);

    }

    private List<Vector3> path = null;

    public List<Vector3> runPathfinding(node endNode)
    {
        int iterations = 0;
        List<Vector3> path = queue.Pop().discover(grid, queue, endNode);
        while (path == null && iterations <= 10000)
        {
            path = queue.Pop().discover(grid, queue, endNode);
            iterations++;
        }

        if (iterations > 10000)
        {
            print("Failed");
        }
        path.Add(target.position);
        return path;
    }

    public node findClosestNode(Vector3 pos)
    {
        //new Vector3((i*nodeSize) + offset.x, yLevel, (j*nodeSize) + offset.y)
        Vector3 closestNodePos = new Vector3();
        closestNodePos.x = pos.x - ((pos.x - masterGrid.offSet[0]) % masterGrid.nodeSize);
        closestNodePos.y = pos.y - ((pos.y - masterGrid.offSet[1]) % masterGrid.nodeSize);
        closestNodePos.z = pos.z - ((pos.z - masterGrid.offSet[2]) % masterGrid.nodeSize);

        int indexX = System.Convert.ToInt16((closestNodePos.x / masterGrid.nodeSize));
        int indexY = System.Convert.ToInt16((closestNodePos.y / masterGrid.nodeSize));
        int indexZ = System.Convert.ToInt16((closestNodePos.z / masterGrid.nodeSize));
        return grid[indexX, indexY, indexZ];
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame

    private int pathPos = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            moving = true;
            path = startPathfinding(target);
        }

        if (moving)
        {
            if (Vector3.Distance(transform.position, path[pathPos]) <= 0.01f)
            {
                print("next");
                pathPos += 1;
                if (pathPos >= path.Count)
                {
                    pathPos = 0;
                    moving = false;
                    //grid = null;
                    path = null;
                    return;
                }
            }
            else
            {
                Vector3 direction = path[pathPos] - transform.position;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    transform.rotation =
                        Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                transform.transform.position =
                    Vector3.MoveTowards(transform.position, path[pathPos], speed * Time.deltaTime);
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.red);
            }
        }
    }

    void OnDrawGizmos()
    {
        return;
        if (grid == null)
        {
            return;
        }

        // Calculate grid size in world units
        Vector3 gridCenter = masterGrid.offSet;
        Vector3 gridExtents = new Vector3(grid.GetLength(2) * masterGrid.nodeSize,
            grid.GetLength(1) * masterGrid.nodeSize,
            grid.GetLength(0) * masterGrid.nodeSize) / 2f;

        // Offset grid center to start from (0, 0, 0) bottom corner
        gridCenter += new Vector3(gridExtents.x, gridExtents.y, gridExtents.z);

        // Draw grid boundary
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gridCenter, gridExtents * 2);

        // Draw nodes
        for (int z = 0; z < grid.GetLength(0); z++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(2); x++)
                {
                    if (grid[z, y, x].state == 6)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(grid[z, y, x].worldPos, masterGrid.nodeSize);
                    }
                    else if (grid[z, y, x].state == 3)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawWireSphere(grid[z, y, x].worldPos, masterGrid.nodeSize);
                    }
                }
            }
        }
    }
    
}