using UnityEngine;
using System.Collections.Generic;

namespace hutian.AI.PathFinding
{
    public class Pathfinding2D : MonoBehaviour
    {

        public Transform seeker, target;
        Grid2D grid;
        Node2D seekerNode, targetNode;
        public GameObject GridOwner;
        public float interval;
        public float timer;

        void Start()
        {
            //Instantiate grid
            grid = GridOwner.GetComponent<Grid2D>();
        }

        private void Update()
        {
            if (timer < 0f)
            {
                timer = interval;
                FindPath(seeker.transform.position, target.position);
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        public void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            //get player and target position in grid coords
            seekerNode = grid.NodeFromWorldPoint(startPos);
            targetNode = grid.NodeFromWorldPoint(targetPos);

            Heap<Node2D> openSet = new Heap<Node2D>(grid.gridSizeX*grid.gridSizeY);
            //List<Node2D> openSet = new List<Node2D>();
            HashSet<Node2D> closedSet = new HashSet<Node2D>();
            openSet.Add(seekerNode);

            //calculates path for pathfinding
            while (openSet.Count > 0)
            {
                Debug.Log(openSet.Count);
                //iterates through openSet and finds lowest FCost
                //Node2D node = openSet[0];
                Node2D node = openSet.RemoveFirst();
                //for (int i = 1; i < openSet.Count; i++)
                //{
                //    if (openSet[i].FCost <= node.FCost)
                //    {
                //        if (openSet[i].hCost < node.hCost)
                //            node = openSet[i];
                //    }
                //}

                //openSet.Remove(node);
                closedSet.Add(node);

                //If target found, retrace path
                if (node == targetNode)
                {
                    RetracePath(seekerNode, targetNode);
                    return;
                }

                //adds neighbor nodes to openSet
                foreach (Node2D neighbour in grid.GetNeighbors(node))
                {
                    if (neighbour.obstacle || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        //加上heap
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        //reverses calculated path so first node is closest to seeker
        void RetracePath(Node2D startNode, Node2D endNode)
        {
            List<Node2D> path = new List<Node2D>();
            Node2D currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            grid.path = path;

        }

        //gets distance between 2 nodes for calculating cost
        int GetDistance(Node2D nodeA, Node2D nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            //Vector3 start =new Vector3(Mathf.RoundToInt(seeker.transform.position.x),Mathf.RoundToInt(seeker.transform.position.y));
            //Vector3 end = new Vector3(Mathf.RoundToInt(target.transform.position.x), Mathf.RoundToInt(target.transform.position.y));
            seekerNode = grid.NodeFromWorldPoint(seeker.transform.position);
            targetNode = grid.NodeFromWorldPoint(target.transform.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(seekerNode.worldPosition, Vector3.one *grid.nodeRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(targetNode.worldPosition, Vector3.one * grid.nodeRadius);
        }
    }
}
