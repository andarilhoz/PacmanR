using UnityEngine;
using _PacmanGame.Scripts.Pathfind;
using Grid = _PacmanGame.Scripts.Pathfind.Grid;

namespace _PacmanGame.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public MapGeneratorJsonDriven MapGenerator;
        public static Grid LevelGrid;
        private static int[,] rowMap;

        private void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            var map = MapGenerator.GenerateMap();
            rowMap = map.mapGrid;
            LevelGrid = new Grid(map.mapGrid, map.realWorldPosGrid);
            MapGenerator.InstantiateMap(rowMap,map.realWorldPosGrid);
        }
        
        private void OnDrawGizmos()
        {
            if ( LevelGrid == null )
            {
                return;
            }

            foreach (var node in LevelGrid.nodes)
            {
                if ( node.IsWall )
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawCube(node.Position, Vector3.one * .255f);
            }
        }
    }
}