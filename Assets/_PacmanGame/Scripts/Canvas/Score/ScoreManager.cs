using System;
using TMPro;
using UnityEngine;
using _PacmanGame.Scripts.Actors;

namespace _PacmanGame.Scripts.Canvas.Score
{
    public class ScoreManager : MonoBehaviour
    {
        public TextMeshProUGUI HighScoreText;
        public TextMeshProUGUI CurrentScoreText;

        public TextMeshProUGUI ComboText;

        public Color GhostPointColor;
        public Color FruitPointColor;

        private int highScore;
        public int CurrentScore;


        private const int EXTRA_LIFE_POINTS = 1000;
        public static event Action ExtraLife;
        private int lastExtraLife = 0;

        private const float COMBO_TEXT_SHOWING_TIME = 1f;
        private float comboTextTimer;
        private bool textOn = false;

        private const string PLAYER_PREFS_HIGHSCORE_KEY = "PACMANHIGHSCORE";

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

        private void OnDestroy()
        {
            ExtraLife = () => { };
        }

        private void Initialize()
        {
            Pacman.AddScore += UpdateScore;
            UpdateScore(0);
            SetSavedHighScore();
        }

        private void SetSavedHighScore()
        {
            var savedHighScore = PlayerPrefs.GetInt(PLAYER_PREFS_HIGHSCORE_KEY, 0);
            UpdateHighScore(savedHighScore);
        }

        public void UpdateScore(int value)
        {
            CurrentScore += value;
            CurrentScoreText.text = CurrentScore.ToString();
            if ( CurrentScore > highScore )
            {
                UpdateHighScore(CurrentScore);
            }

            if ( CurrentScore - lastExtraLife < EXTRA_LIFE_POINTS )
            {
                return;
            }

            lastExtraLife = CurrentScore;
            ExtraLife?.Invoke();
        }

        public void UpdateHighScore(int number)
        {
            highScore = number;
            HighScoreText.text = highScore.ToString();
            PlayerPrefs.SetInt(PLAYER_PREFS_HIGHSCORE_KEY, number);
        }

        private void Update()
        {
            if ( comboTextTimer <= 0 && textOn )
            {
                ComboText.transform.position = new Vector2(100, 100);
                return;
            }

            if ( comboTextTimer <= 0 )
            {
                return;
            }

            comboTextTimer -= Time.deltaTime;
        }

        public void SetComboText(string text, Vector2 postion, bool isGhost)
        {
            comboTextTimer = COMBO_TEXT_SHOWING_TIME;
            ComboText.text = text;
            ComboText.color = isGhost ? GhostPointColor : FruitPointColor;
            ComboText.transform.position = postion;
            textOn = true;
        }
    }
}