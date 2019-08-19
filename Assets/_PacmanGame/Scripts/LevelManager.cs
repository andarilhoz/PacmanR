using UnityEngine;
using _PacmanGame.Scripts.Pathfind;
using Grid = _PacmanGame.Scripts.Pathfind.Grid;

namespace _PacmanGame.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public MapGeneratorJsonDriven MapGenerator;

        private void Start()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            var map = MapGenerator.GenerateMap();
            GetComponent<Grid>().CreateGrid(map.mapGrid, map.realWorldPosGrid);
        }
    }
}