using System;
using System.Collections.Generic;
using UnityEngine;
using _PacmanGame.Scripts.ExtensionHelpers;

namespace _PacmanGame.Scripts.Pathfind
{
    public class Grid
    {
        public List<Node> FinalPath;
        
        private Node[,] grid;
        private Vector2[,] realWorldPos;
        
        private int gridSizeX;
        private int gridSizeY;
        public List<Node> nodes = new List<Node>();
        
        private const float TILE_OFFSET = 0.255f;
        
        public Grid(int[,] map, Vector2[,] realWorldPosGrid)
        {
            realWorldPos = realWorldPosGrid;
            
            gridSizeX = map.GetLength(0);
            gridSizeY = map.GetLength(1);
            
            grid = new Node[map.GetLength(0), map.GetLength(1)];
            
            for (var x = 0; x < map.GetLength(0); x++)
            {
                for (var y = 0; y < map.GetLength(1); y++)
                {
                    var Wall = !((ItemTypes) map[x, y]).IsValidPath();
                    var ThinWall = ((ItemTypes) map[x, y]).Equals(ItemTypes.ThinWall);
                    var IsTeleport = ((ItemTypes) map[x, y]).Equals(ItemTypes.Teleport);
                    var node = new Node(Wall, realWorldPosGrid[x,y], x, y);
                    node.IsTeleport = IsTeleport;
                    node.ThinWall = ThinWall;
                    
                    nodes.Add(node);
                    grid[x, y] = node;
                }
            }
            
            var teleports = nodes.FindAll(n => n.IsTeleport);
            teleports[0].TwinTeleport = teleports[1];
            
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    var nodeIntersections = new Node.Intersections()
                    {
                        Up = GetIntersection(grid, x, y, Vector2.up),
                        Down = GetIntersection(grid, x, y, Vector2.down),
                        Left = GetIntersection(grid, x, y, Vector2.left),
                        Right = GetIntersection(grid, x, y, Vector2.right)
                    };
                    grid[x, y].nodeIntersections = nodeIntersections;
                }
            }

            
            

        }

        public Node GetIntersection(Node[,] map, int x, int y, Vector2 direction)
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

            if ( y < 0 || y >= map.GetLength(1))
            {
                return null;
            }

            if ( x < 0 || x >= map.GetLength(0))
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
                return grid[xCheck,yCheck];
            }

            return null;
        }

       
    }
}