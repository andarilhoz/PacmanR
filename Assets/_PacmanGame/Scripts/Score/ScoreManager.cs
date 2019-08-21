using TMPro;
using UnityEngine;
using _PacmanGame.Scripts.Actors;

namespace _PacmanGame.Scripts.Score
{
    public class ScoreManager : MonoBehaviour
    {
        public TextMeshProUGUI HighScoreText;
        public TextMeshProUGUI CurrentScoreText;

        public TextMeshProUGUI ghostComboText;

        private int highScore;
        private int currentScore;

        private float comboTextShowingTime = 1f;
        private float comboTextTimer;
        private bool textOn = false;

        #region Singleton

        private static ScoreManager instance;

        public static ScoreManager Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = GameObject.FindObjectOfType<ScoreManager>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            if ( instance == null )
            {
                instance = this;
                Initialize();
            }
        }

        #endregion

        private void Initialize()
        {
            /* TODO save highscore in playerpref */
            Pacman.AddScore += UpdateScore;
            UpdateScore(0);
            UpdateHighScore(0);
        }

        public void UpdateScore(int value)
        {
            currentScore += value;
            CurrentScoreText.text = currentScore.ToString();
            if ( currentScore > highScore )
            {
                UpdateHighScore(currentScore);
            }
        }

        public void UpdateHighScore(int number)
        {
            highScore = number;
            HighScoreText.text = highScore.ToString();
        }

        private void Update()
        {
            if ( comboTextTimer <= 0 && textOn )
            {
                ghostComboText.transform.position = new Vector2(100, 100);
                return;
            }

            comboTextTimer -= Time.deltaTime;
        }

        public void SetComboText(string text, Vector2 postion)
        {
            comboTextTimer = comboTextShowingTime;
            ghostComboText.text = text;
            ghostComboText.transform.position = postion;
            textOn = true;
        }
    }
}