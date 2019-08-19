using System;
using UnityEngine;
using _PacmanGame.Scripts.Input;
using _PacmanGame.Scripts.Score;

namespace _PacmanGame.Scripts.Actors
{
    public class Pacman : Actors
    {
        public bool isAlive;
        public KeyboardInput InputControll;
        
        public static event Action<int> AddScore;

        protected override void Awake()
        {
            base.Awake();
            InputControll.OnInput += ChangeDirection;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ( !other.transform.CompareTag("Dot") )
            {
                return;
            }
            Destroy(other.gameObject);
            AddScore?.Invoke(1);
        }
    }
}