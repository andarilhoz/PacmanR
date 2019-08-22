using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Canvas.Score;

namespace _PacmanGame.Scripts
{
    public class LivesManager : MonoBehaviour
    {
        public List<GameObject> PacmanLivesImages;
        
        private int lives = 2;
        private const int MAX_LIVES = 5;

        private void Start()
        {
            Pacman.Die += async () =>
            {
                LevelManager.Instance.PauseGame();
                await Task.Delay(TimeSpan.FromSeconds(1.5f));
                Die();
            };
            ScoreManager.ExtraLife += GainLive;
            UpdateLives(lives);
        }
        
        
        private void GainLive()
        {
            if ( lives >= MAX_LIVES )
            {
                return;
            }

            UpdateLives(++lives);
        }
        
        private void UpdateLives(int currentLives)
        {
            PacmanLivesImages.ForEach(l => l.SetActive(false));
            for (var i = 0; i < currentLives; i++)
            {
                PacmanLivesImages[i].SetActive(true);
            }
        }
        
        private void Die()
        {
            if ( lives > 0 )
            {
                lives--;
                UpdateLives(lives);
                LevelManager.Instance.ResetActors();
                LevelManager.Instance.ResumeGame();
                return;
            }
            LevelManager.Instance.ResetGame();
        }
    }
}