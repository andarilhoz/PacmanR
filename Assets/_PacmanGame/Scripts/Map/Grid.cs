using System.Collections.Generic;
using UnityEngine;

namespace _PacmanGame.Scripts.Map
{
    public class Grid
    {
        private readonly List<Node> nodes = new List<Node>();

        // here are initialized the nodes and saved on a list
        public Grid(int[,] map, Vector2[,] realWorldPosGrid)
        {
            var grid = new Node[map.GetLength(0), map.GetLength(1)];

            for (var x = 0; x < map.GetLength(0); x++)
            {
                for (var y = 0; y < map.GetLength(1); y++)
                {
                    var wall = !((ItemTypes) map[x, y]).IsValidPath();
                    var thinWall = ((ItemTypes) map[x, y]).Equals(ItemTypes.ThinWall);
                    var isTeleport = ((ItemTypes) map[x, y]).Equals(ItemTypes.Teleport);

                    var node = new Node(wall, realWorldPosGrid[x, y], x, y)
                    {
                        IsTeleport = isTeleport, ThinWall = thinWall
                    };

                    nodes.Add(node);
                    grid[x, y] = node;
                }
            }
            
            // here the teleports do reference each other
            var teleports = nodes.FindAll(n => n.IsTeleport);
            teleports[0].TwinTeleport = teleports[1];

            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    var nodeNeighbors = new Node.Intersections()
                    {
                        Up = GetNeighbors(grid, x, y, Vector2.up),
                        Down = GetNeighbors(grid, x, y, Vector2.down),
                        Left = GetNeighbors(grid, x, y, Vector2.left),
                        Right = GetNeighbors(grid, x, y, Vector2.right)
                    };
                    grid[x, y].NodeNeighbors = nodeNeighbors;
                }
            }
        }
        
        private static Node GetNeighbors(Node[,] map, int x, int y, Vector2 direction)
        {
            if ( direction == Vector2.up )
            {
                x++;
            }

            if ( direction == Vector2.down )
            {
                x--;
            }

            if ( direction == Vector2.left )
            {
                y--;
            }

            if ( direction == Vector2.right )
            {
                y++;
            }

            if ( y < 0 || y >= map.GetLength(1) )
            {
                return null;
            }

            if ( x < 0 || x >= map.GetLength(0) )
            {
                return null;
            }

            if ( map[x, y].IsWall )
            {
                return null;
            }

            return map[x, y];
        }

        public Node NodeFromWorldPostiion(Vector2 worldPos)
        {
            nodes.Sort((v1, v2) =>
                Vector2.Distance(v1.Position, worldPos).CompareTo(Vector2.Distance(v2.Position, worldPos)));
            return nodes[0];
        }
    }
}