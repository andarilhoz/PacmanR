using UnityEngine;

namespace _PacmanGame.Scripts.Pathfind
{
    public class Node
    {
        public int gridX;
        public int gridY;

        public bool IsWall;
        public bool IsTeleport;
        private Node twinTeleport;
        public Node TwinTeleport
        {
            get { return twinTeleport; }
            set
            {
                value.twinTeleport = this;
                value.isLeft = Position.x > value.Position.x;
                isLeft = Position.x < value.Position.x;
                twinTeleport = value;
            }
        }

        public bool IsIntersection
        {
            get
            {
                var horizontal = nodeIntersections.Left != null || nodeIntersections.Right != null;
                var vertical = nodeIntersections.Up != null || nodeIntersections.Down != null;
                return horizontal && vertical;
            }
        }

        public bool isLeft;
        
        public Vector2 Position;

        public Node Parent;

        public int gCost;
        public int hCost;
        public int FCost => gCost + hCost;
        public Intersections nodeIntersections;
        public bool ThinWall = false;

    
        public Node(bool isWall, Vector2 aPos, int aGridX, int aGridY)
        {
            Position = aPos;
            IsWall = isWall;
            gridX = aGridX;
            gridY = aGridY;
        }

        public class Intersections
        {
            public Node Right;
            public Node Left;
            public Node Up;
            public Node Down;
        }
    }
}