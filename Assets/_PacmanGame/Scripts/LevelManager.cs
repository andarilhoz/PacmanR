using UnityEngine;
using UnityEngine.SceneManagement;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Canvas.Fadeout;
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
            wallMaterialOriginalColor = wallMaterial.color;
        }

        private void Update()
        {
            if ( !wonGame )
            {
                return;
            }

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