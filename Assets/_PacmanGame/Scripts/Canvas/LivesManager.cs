using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Utils;

namespace _PacmanGame.Scripts.Canvas
{
    public class LivesManager : MonoBehaviour
    {
        public List<GameObject> PacmanLivesImages;

        private int lives = 2;
        private const int MAX_LIVES = 5;
        private const float DIE_TIMER = 1.5f;

        private void Start()
        {
#if UNITY_WEBGL
            Pacman.Die += () =>
            {
                LevelManager.Instance.PauseGame();
                StartCoroutine(CoroutineUtils.WaitSecondsCoroutine(DIE_TIMER, Die));
            };

#else
            Pacman.Die += async () =>
            {
                LevelManager.Instance.PauseGame();
                await Task.Delay(TimeSpan.FromSeconds(DIE_TIMER));
                Die();
            };
#endif
            ScoreManager.ExtraLife += GainLive;
            UpdateLives(lives);
        }

        private void GainLive()
        {
            if ( lives < MAX_LIVES )
            {
                UpdateLives(++lives);
            }
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