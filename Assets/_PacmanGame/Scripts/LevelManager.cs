using TMPro;
using UnityEngine;
using _PacmanGame.Scripts.Pathfind;
using Grid = _PacmanGame.Scripts.Pathfind.Grid;

namespace _PacmanGame.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public MapGeneratorJsonDriven MapGenerator;

        public TextMeshProUGUI ghostComboText;
        
        public static Grid LevelGrid;
        private static int[,] rowMap;
        
        private float comboTextShowingTime = 1f;
        private float comboTextTimer;
        private bool textOn = false;
        
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
            var map = MapGenerator.GenerateMap();
            rowMap = map.mapGrid;
            LevelGrid = new Grid(map.mapGrid, map.realWorldPosGrid);
            MapGenerator.InstantiateMap(rowMap,map.realWorldPosGrid);
        }

        public void SetComboText(string text, Vector2 postion)
        {
            comboTextTimer = comboTextShowingTime;
            ghostComboText.text = text;
            ghostComboText.transform.position = postion;
            textOn = true;
        }

        private void Update()
        {
            if ( comboTextTimer <= 0 && textOn)
            {
                ghostComboText.transform.position = new Vector2(100, 100);
                return;
            }

            comboTextTimer -= Time.deltaTime;
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