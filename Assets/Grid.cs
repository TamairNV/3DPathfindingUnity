using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector3 gridSize = new Vector3(50, 50, 50);
    public Vector3 offSet = new Vector3(0, 0, 0);
    public LayerMask wallLayer;
    public float nodeSize = 1;
    private HashSet<Vector3Int> wallIndices;

    private HashSet<Vector3Int> targetIndecies;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public node[,,] CreateGrid(Vector3 gridSize,Transform target)
    {
        if (wallIndices == null)
        {
            wallIndices = GetBuildingIndices(target);
        }

        targetIndecies = getIndice(target);
        
        int xSize = System.Convert.ToInt16(gridSize.x);
        int ySize = System.Convert.ToInt16(gridSize.y);
        int zSize = System.Convert.ToInt16(gridSize.z);

        node[,,] grid = new node[zSize, ySize, xSize];

        for (int z = 0; z < zSize; z++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    int state = 0;
                    if (wallIndices.Contains(new Vector3Int(z, y,x)) &&
                        !targetIndecies.Contains(new Vector3Int(z, y,x)))
                    {
                        state = 3; // wall
                    }
                    node n = new node(new int[] { z, y, x },new Vector3(z*nodeSize + offSet[2], y*nodeSize+ offSet[1], x*nodeSize+ offSet[0]),state_:state);
                    grid[z, y, x] = n;
                }
            }
        }
        
        return grid;
    }

    public void resetGrid(node[,,] grid)
    {

        for (int z = 0; z < grid.GetLength(0); z++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(2); x++)
                {
                    int state = 0;
                    if (wallIndices.Contains(new Vector3Int(z, y,x)) &&
                        !targetIndecies.Contains(new Vector3Int(z, y,x)))
                    {
                        state = 3; // wall
                    }
                    grid[z, y, x].reset(new int[] { z, y, x },new Vector3(z*nodeSize + offSet[2], y*nodeSize+ offSet[1], x*nodeSize+ offSet[0]),state_:state);
                  
                }
            }
        }
        
    }
    public HashSet<Vector3Int> GetBuildingIndices(Transform target)
    {
        GameObject[] buildings =  FindObjectsOfType<GameObject>().Where(obj => obj.layer == 6).ToArray();
        HashSet<Vector3Int> indices = new HashSet<Vector3Int>();
        
        foreach (GameObject collider in buildings)
        {
            if (collider.GetComponent<Collider>() != target.GetComponent<Collider>())
            {
                indices.UnionWith(getIndice(collider.transform));
            }
        }

        return indices;
    }

    private HashSet<Vector3Int> getIndice(Transform collider)
    {
        //finds the indices of the points where the collider overlaps with the grid
        HashSet<Vector3Int> indices = new HashSet<Vector3Int>();
        Collider colliders = collider.GetComponent<Collider>();
        Vector3 center = colliders.bounds.center;
        Vector3 extents = colliders.bounds.extents;
        int startX = Mathf.RoundToInt((center.x - extents.x - offSet.x) / nodeSize);
        int endX = Mathf.RoundToInt((center.x + extents.x - offSet.x) / nodeSize);
        int startY = Mathf.RoundToInt((center.y - extents.y - offSet.y) / nodeSize);
        int endY = Mathf.RoundToInt((center.y + extents.y - offSet.y) / nodeSize);
        int startZ = Mathf.RoundToInt((center.z - extents.z - offSet.z) / nodeSize);
        int endZ = Mathf.RoundToInt((center.z + extents.z - offSet.z) / nodeSize);

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                for (int z = startZ; z <= endZ; z++)
                {
                    indices.Add(new Vector3Int(x, y, z));
                }
            }
        }

        return indices;
    }



}
