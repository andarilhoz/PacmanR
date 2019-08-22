﻿using TMPro;
using UnityEngine;
using _PacmanGame.Scripts.Actors;

namespace _PacmanGame.Scripts.Score
{
    public class ScoreManager : MonoBehaviour
    {
        public TextMeshProUGUI HighScoreText;
        public TextMeshProUGUI CurrentScoreText;

        public TextMeshProUGUI comboText;

        public Color ghostPointColor;
        public Color fruitPointColor;

        private int highScore;
        public int CurrentScore;

        private float comboTextShowingTime = 1f;
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
                comboText.transform.position = new Vector2(100, 100);
                return;
            }

            comboTextTimer -= Time.deltaTime;
        }

        public void SetComboText(string text, Vector2 postion, bool isGhost)
        {
            comboTextTimer = comboTextShowingTime;
            comboText.text = text;
            comboText.color = isGhost ? ghostPointColor : fruitPointColor;
            comboText.transform.position = postion;
            textOn = true;
        }
    }
}