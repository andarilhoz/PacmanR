using UnityEngine;

namespace _PacmanGame.Scripts.Pathfind
{
    public class Node
    {
        public int gridX;
        public int gridY;

        public bool IsWall;

        public Vector2 Position;

        public Node Parent;

        public bool dirt = false;

        public int gCost;
        public int hCost;
        public int FCost => gCost + hCost;

        public Node(bool isWall, Vector2 aPos, int aGridX, int aGridY)
        {
            Position = aPos;
            IsWall = isWall;
            gridX = aGridX;
            gridY = aGridY;

        }
    }
}