using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Canvas;
using _PacmanGame.Scripts.Map;
using Grid = _PacmanGame.Scripts.Map.Grid;

namespace _PacmanGame.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public MapGenerator MapGenerator;

        public Grid LevelGrid { get; set; }
        public int[,] RowMap { get; set; }
        public Vector2[,] RealWorldPosMap { get; set; }
        private GameState currentGameState = GameState.Pause;

        private int eatenDots = 0;
        private int maxDots;

        public Material WallMaterial;
        private Color wallMaterialOriginalColor;
        private const float WALL_FLASH_FREQUENCY = .3f;
        private float flashTimer = 0;

        private const int FLASH_MAX_TIMES = 5;
        private int flashTimes = 0;

        private bool wonGame = false;
        
    
        private Actors.Actors[] actors;


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
            InstructionsFadeout.StartGame += () => currentGameState = GameState.Playing;
            Pacman.EatDot += PacmanDotEat;

            wallMaterialOriginalColor = WallMaterial.color;

            actors = FindObjectsOfType<Actors.Actors>();
        }

        public void PauseGame()
        {
            currentGameState = GameState.Pause;
        }
        
        public void ResumeGame()
        {
            currentGameState = GameState.Playing;
        }

        public GameState GetCurrentState()
        {
            return currentGameState;
        }

        public void ResetActors()
        {
            foreach (var actor in actors)
            {
                actor.ResetActor();
            }
        }

        private void Update()
        {
            if ( !wonGame )
            {
                return;
            }

            WonGameAnimation();
        }

        private void WonGameAnimation()
        {
            if ( flashTimer <= 0 )
            {
                FlashWallMaterial();
                flashTimer = WALL_FLASH_FREQUENCY;
                return;
            }

            flashTimer -= Time.deltaTime;
        }

        private void PacmanDotEat()
        {
            eatenDots++;

            if ( eatenDots < maxDots )
            {
                return;
            }

            currentGameState = GameState.Pause;
            wonGame = true;
        }

        public void FlashWallMaterial()
        {
            if ( flashTimes > FLASH_MAX_TIMES )
            {
                ResetGame();
                WallMaterial.color = wallMaterialOriginalColor;
                return;
            }

            WallMaterial.color = WallMaterial.color == Color.white ? wallMaterialOriginalColor : Color.white;
            flashTimes++;
        }

        public void ResetGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Initialize()
        {
            (RowMap, RealWorldPosMap) = MapGenerator.GenerateMap();
            LevelGrid = new Grid(RowMap, RealWorldPosMap);
            maxDots = MapGenerator.InstantiateMap(RowMap, RealWorldPosMap);
        }

        public enum GameState
        {
            Playing = 0,
            Pause = 1,
        }
    }
}