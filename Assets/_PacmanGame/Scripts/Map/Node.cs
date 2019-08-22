using UnityEngine;

namespace _PacmanGame.Scripts.Map
{
    [System.Serializable]
    public class Node
    {
        private int gridX;
        private int gridY;

        public readonly bool IsWall;
        public bool IsTeleport;
        private Node twinTeleport;

        public bool IsLeft;

        public Vector2 Position;

        public Intersections NodeNeighbors;
        public bool ThinWall = false;


        public Node TwinTeleport
        {
            get => twinTeleport;
            set
            {
                value.twinTeleport = this;
                value.IsLeft = Position.x > value.Position.x;
                IsLeft = Position.x < value.Position.x;
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
        
        // ghosts can only do a movement if are on a intersection 
        public bool IsIntersection
        {
            get
            {
                var horizontal = NodeNeighbors.Left != null || NodeNeighbors.Right != null;
                var vertical = NodeNeighbors.Up != null || NodeNeighbors.Down != null;
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