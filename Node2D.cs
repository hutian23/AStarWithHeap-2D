using UnityEngine;

namespace hutian.AI.PathFinding
{
    public class Node2D/* : IHeapItem<Node2D>*/:IComparable<Node2D>
    {
        public int gCost; //从起点到当前节点的代价
        public int hCost; //从当前格子到节点的代价(预估代价)
        public bool obstacle; //当前节点是否是障碍物
        public Vector3 worldPosition; //当前节点所在的世界坐标

        public int GridX, GridY; //节点在二维网格中的坐标
        public Node2D parent;   //父节点，根据父节点回溯到起点从而找到路径

        //private int heapIndex;

        public Node2D(bool _obstacle, Vector3 _worldPos, int _gridX, int _gridY)
        {
            obstacle = _obstacle;
            worldPosition = _worldPos;
            GridX = _gridX;
            GridY = _gridY;
        }

        public int FCost //节点的总代价
        {
            get
            {
                return gCost + hCost;
            }

        }

        //public int HeapIndex
        //{
        //    get => heapIndex;
        //    set => heapIndex = value;
        //}

        public int CompareTo(Node2D nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            
            //总代价相同，比较预估代价(选择离终点更近的节点)
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }

            return -compare;
        }

        public void SetObstacle(bool isObstacle)
        {
            obstacle = isObstacle;
        }
    }

}
