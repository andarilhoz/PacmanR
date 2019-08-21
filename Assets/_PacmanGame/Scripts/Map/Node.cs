using UnityEngine;

namespace _PacmanGame.Scripts.Pathfind
{
    [System.Serializable]
    public class Node
    {
        private int gridX;
        private int gridY;

        public readonly bool IsWall;
        public bool IsTeleport;
        private Node twinTeleport;

        public bool isLeft;

        public Vector2 Position;

        public Intersections nodeIntersections;
        public bool ThinWall = false;


        public Node TwinTeleport
        {
            get => twinTeleport;
            set
            {
                value.twinTeleport = this;
                value.isLeft = Position.x > value.Position.x;
                isLeft = Position.x < value.Position.x;
                twinTeleport = value;
            }
        }

        public Node(bool isWall, Vector2 aPos, int aGridX, int aGridY)
        {
            Position = aPos;
            IsWall = isWall;
            gridX = aGridX;
            gridY = aGridY;
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

        public class Intersections
        {
            public Node Right;
            public Node Left;
            public Node Up;
            public Node Down;
        }
    }
}