using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Canvas.Fadeout;
using _PacmanGame.Scripts.Canvas.Score;
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
        public GameState CurrentGameState = GameState.Pause;

        private int eatenDots = 0;
        private int maxDots;
        
        public Material wallMaterial;
        private Color wallMaterialOriginalColor;
        private float wallFlashFrequency = .3f;
        private float flashTimer = 0;

        private int flashMaxTimes = 5;
        private int flashTimes = 0;

        private bool wonGame = false;
        private bool looseGame = false;

        private int lives = 2;
        private int maxLives = 5;

        private Actors.Actors[] actors;

        public List<GameObject> pacmanLivesImages;

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
            Pacman.EatDot += PacmanDotEat;
            Pacman.Die += async () =>
            {
                CurrentGameState = GameState.Pause;
                await Task.Delay(TimeSpan.FromSeconds(1.5f));
                Die();
            };
            ScoreManager.ExtraLife += GainLive;
            wallMaterialOriginalColor = wallMaterial.color;

            actors = FindObjectsOfType<Actors.Actors>();
            UpdateLives(lives);
        }

        private void GainLive()
        {
            if ( lives >= maxLives )
            {
                return;
            }
            
            UpdateLives(++lives);
        }

        private void UpdateLives(int currentLives)
        {
            pacmanLivesImages.ForEach(l => l.SetActive(false));
            for (var i = 0; i < currentLives; i++)
            {
                pacmanLivesImages[i].SetActive(true);    
            }
        }

        private async void Update()
        {
            if ( wonGame )
            {
                WonGameAnimation();
                return;
            }
        }

        private void Die()
        {
            if ( lives > 0 )
            {
                lives--;
                UpdateLives(lives);
                foreach (var actor in actors)
                {
                    actor.ResetActor();
                }
                CurrentGameState = GameState.Playing;
                return;
            }
            ResetGame();
        }

        private void WonGameAnimation()
        {
            if ( flashTimer <= 0 )
            {
                FlashWallMaterial();
                flashTimer = wallFlashFrequency;
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

            CurrentGameState = GameState.Pause;
            wonGame = true;
        }

        public void FlashWallMaterial()
        {
            if ( flashTimes > flashMaxTimes )
            {
                ResetGame();
                wallMaterial.color = wallMaterialOriginalColor;
                return;
            }

            wallMaterial.color = wallMaterial.color == Color.white ? wallMaterialOriginalColor : Color.white;
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