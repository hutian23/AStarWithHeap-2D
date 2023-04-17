using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace hutian.AI.PathFinding
{
    public class Grid2D : MonoBehaviour
    {
        public Vector3 gridWorldSize;
        public float nodeRadius;
        public Node2D[,] Grid;
        public Tilemap obstaclemap;
        public List<Node2D> path;
        Vector3 worldBottomLeft;

        float nodeDiameter;
        public int gridSizeX, gridSizeY;

        void Awake()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            CreateGrid();
        }



        void CreateGrid()
        {
            Grid = new Node2D[gridSizeX, gridSizeY];
            worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    Grid[x, y] = new Node2D(false, worldPoint, x, y);

                    if (obstaclemap.HasTile(obstaclemap.WorldToCell(Grid[x, y].worldPosition)))
                        Grid[x, y].SetObstacle(true);
                    else
                        Grid[x, y].SetObstacle(false);


                }
            }
        }


        //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
        public List<Node2D> GetNeighbors(Node2D node)
        {
            List<Node2D> neighbors = new List<Node2D>();

            ////checks and adds top neighbor
            //if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            //    neighbors.Add(Grid[node.GridX, node.GridY + 1]);

            ////checks and adds bottom neighbor
            //if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            //    neighbors.Add(Grid[node.GridX, node.GridY - 1]);

            ////checks and adds right neighbor
            //if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            //    neighbors.Add(Grid[node.GridX + 1, node.GridY]);

            ////checks and adds left neighbor
            //if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            //    neighbors.Add(Grid[node.GridX - 1, node.GridY]);

            //八方向搜索
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    //越界
                    if (node.GridX + i < 0 || node.GridX + i > gridSizeX ||
                        node.GridY + j < 0 || node.GridY + j > gridSizeY)
                    {
                        continue;
                    }

                    Node2D neighbourNode = Grid[node.GridX + i, node.GridY + j];
                    //处于斜角的位置
                    if (Mathf.Abs(i) == 1 && Mathf.Abs(j) == 1)
                    {
                        if (Grid[node.GridX, node.GridY + j].obstacle || Grid[node.GridX + i, node.GridY].obstacle) //斜角的节点的两个邻居节点有任意一个是障碍物，这个点就不能走
                        {
                            continue;
                        }
                    }
                    neighbors.Add(neighbourNode);
                }
            }

            /* Uncomment this code to enable diagonal movement

            //checks and adds top right neighbor
            if (node.GridX + 1 >= 0 && node.GridX + 1< gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX + 1, node.GridY + 1]);

            //checks and adds bottom right neighbor
            if (node.GridX + 1>= 0 && node.GridX + 1 < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX + 1, node.GridY - 1]);

            //checks and adds top left neighbor
            if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY + 1>= 0 && node.GridY + 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX - 1, node.GridY + 1]);

            //checks and adds bottom left neighbor
            if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY  - 1>= 0 && node.GridY  - 1 < gridSizeY)
                neighbors.Add(Grid[node.GridX - 1, node.GridY - 1]);
            */



            return neighbors;
        }

        public Node2D NodeFromWorldPoint(Vector3 worldPosition)
        {

            int x = Mathf.RoundToInt(worldPosition.x + (gridSizeX / 2f));
            int y = Mathf.RoundToInt(worldPosition.y + (gridSizeY / 2f));
            return Grid[x, y];
        }



        //Draws visual representation of grid
        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

            if (Grid != null)
            {
                foreach (Node2D n in Grid)
                {
                    if (n.obstacle)
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = Color.white;

                    if (path != null && path.Contains(n))
                        Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius));

                }
            }
        }
    }

}
