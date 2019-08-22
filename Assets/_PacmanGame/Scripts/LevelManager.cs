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
        
        public GameState CurrentGameState = GameState.Pause;

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
            InstructionsFadeout.StartGame += () => CurrentGameState = GameState.Playing;
        }

        public void Initialize()
        {
            (rowMap, realWorldPosMap) = MapGenerator.GenerateMap();
            LevelGrid = new Grid(rowMap, realWorldPosMap);
            MapGenerator.InstantiateMap(rowMap, realWorldPosMap);
        }

        public enum GameState
        {
            Playing = 0,
            Pause = 1,
        }
    }
}