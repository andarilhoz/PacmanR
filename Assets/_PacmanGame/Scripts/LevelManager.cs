using TMPro;
using UnityEngine;
using _PacmanGame.Scripts.Pathfind;
using Grid = _PacmanGame.Scripts.Pathfind.Grid;

namespace _PacmanGame.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public MapGenerator MapGenerator;

        public Grid LevelGrid { get; set; }
        public int[,] rowMap { get; set; }
        public Vector2[,] realWorldPosMap { get; set; }

        #region Singleton

        private static LevelManager instance;

        public static LevelManager Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = GameObject.FindObjectOfType<LevelManager>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            if ( instance == null )
            {
                instance = this;
            }
        }

        #endregion

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            (rowMap, realWorldPosMap) = MapGenerator.GenerateMap();
            LevelGrid = new Grid(rowMap, realWorldPosMap);
            MapGenerator.InstantiateMap(rowMap, realWorldPosMap);
        }
    }
}