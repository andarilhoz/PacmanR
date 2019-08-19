using System.Collections.Generic;
using UnityEngine;
using _PacmanGame.Scripts.ExtensionHelpers;

namespace _PacmanGame.Scripts.Pathfind
{
    public class Grid : MonoBehaviour
    {
        private Node[,] grid;
        private Vector2[,] realWorldPos;
        public List<Node> FinalPath;
        
        private int gridSizeX;
        private int gridSizeY;
        private List<Node> nodes = new List<Node>();

        private Vector2 GridWorldSize;
        
        private const float TILE_OFFSET = 0.255f;
        
        public void CreateGrid(int[,] map, Vector2[,] realWorldPosGrid)
        {
            realWorldPos = realWorldPosGrid;
            
            gridSizeX = map.GetLength(0);
            gridSizeY = map.GetLength(1);
            GridWorldSize = new Vector2(gridSizeX * TILE_OFFSET, gridSizeY * TILE_OFFSET);
            grid = new Node[map.GetLength(0), map.GetLength(1)];
            for (var y = 0; y < map.GetLength(0); y++)
            {
                for (var x = 0; x < map.GetLength(1); x++)
                {
                    var yOffset = (map.GetLength(0) * TILE_OFFSET) / 2;
                    var xOffset = (map.GetLength(1) * TILE_OFFSET) /2;
                    var nodeWorldPos = new Vector2( x * TILE_OFFSET - xOffset, y * TILE_OFFSET - yOffset);

                    bool Wall = !((ItemTypes) map[y, x]).IsValidPath();
                    var node = new Node(Wall, nodeWorldPos, x, y );
                    nodes.Add(node);
                    grid[y, x] = node;
                }
            }
        }

        public Node NodeFromWorldPostiion(Vector2 worldPos)
        {
            nodes.Sort((v1,v2) =>Vector2.Distance(v1.Position,worldPos).CompareTo(Vector2.Distance(v2.Position,worldPos)));
            var a = nodes.FindAll(n => n.gridY == 1);
            return nodes[0];
        }

        public List<Node> GetNeighboringNodes(Node aNode)
        {
            List<Node> NeighboringNodes = new List<Node>();
            int xCheck;
            int yCheck;
            
            //Right
            xCheck = aNode.gridX + 1;
            yCheck = aNode.gridY;
            
            NeighboringNodes.AddIfNotNull(CheckNeighboringSide(xCheck, yCheck));
            
            //Left
            xCheck = aNode.gridX - 1;
            yCheck = aNode.gridY;
            
            NeighboringNodes.AddIfNotNull(CheckNeighboringSide(xCheck, yCheck));
            //Top
            xCheck = aNode.gridX;
            yCheck = aNode.gridY + 1;
            
            NeighboringNodes.AddIfNotNull(CheckNeighboringSide(xCheck, yCheck));
            //Bottom
            xCheck = aNode.gridX;
            yCheck = aNode.gridY - 1;
            
            NeighboringNodes.AddIfNotNull(CheckNeighboringSide(xCheck, yCheck));

            return NeighboringNodes;
        }
        
        private Node CheckNeighboringSide(int xCheck, int yCheck)
        {
            if ( xCheck < 0 || xCheck >= gridSizeY )
            {
                return null;
            }

            if ( yCheck >= 0 && yCheck < gridSizeX )
            {
                return grid[yCheck, xCheck];
            }

            return null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, new Vector3(gridSizeX, 1, gridSizeY));
            if ( grid == null )
            {
                return;
            }

            foreach (var node in grid)
            {
                if ( node.IsWall )
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }

                if (FinalPath != null)//If the final path is not empty
                {
                    if (FinalPath.Contains(node))//If the current node is in the final path
                    {
                        Gizmos.color = Color.red;//Set the color of that node
                    }

                }

                Gizmos.DrawCube(node.Position, Vector3.one * TILE_OFFSET);
            }
        }
    }
}