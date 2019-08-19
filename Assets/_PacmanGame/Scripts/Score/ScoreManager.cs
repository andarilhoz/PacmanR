using System;
using TMPro;
using UnityEngine;
using _PacmanGame.Scripts.Actors;

namespace _PacmanGame.Scripts.Score
{
    public class ScoreManager : MonoBehaviour
    {
        public TextMeshProUGUI HighScoreText;
        public TextMeshProUGUI CurrentScoreText;
            
        private int highScore;
        private int currentScore;

        private void Awake()
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
    }
}
