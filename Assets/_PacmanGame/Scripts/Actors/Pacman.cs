using System;
using TMPro;
using UnityEngine;
using _PacmanGame.Scripts.Actors.Ghosts;
using _PacmanGame.Scripts.Input;
using _PacmanGame.Scripts.Pathfind;
using _PacmanGame.Scripts.Score;

namespace _PacmanGame.Scripts.Actors
{
    public class Pacman : Actors
    {
        public bool isAlive;
        public KeyboardInput InputControll;

        public static event Action EatPowerDot;
        public static event Action<int> AddScore;

        public float ghostEatComboTimer = 2f;
        private float comboTimer = 0;
        private int comboCounter = 0;
        private readonly int[] comboValues = {200, 400, 800, 1600};

        protected override void Awake()
        {
            base.Awake();
            InputControll.OnInput += ChangeDirection;
            BaseGhost.TouchGhost += TouchedGhost;
        }

        protected override void Start()
        {
            base.Start();
            ChangeDirection(currentDirection);
        }

        private void Update()
        {
            ComboTimerChecker();
        }

        private void ComboTimerChecker()
        {
            if ( comboCounter <= 0 )
            {
                return;
            }

            if ( comboTimer <= 0 )
            {
                comboCounter = 0;
                return;
            }

            comboTimer -= Time.deltaTime;
        }

        private void TouchedGhost(GhostStates ghostStates)
        {
            if ( ghostStates.Equals(GhostStates.Afraid) )
            {
                EatGhost();
                return;
            }

            // TODO do death
            Debug.Log("Ouch i'm dead");
        }

        private void EatGhost()
        {
            comboTimer = ghostEatComboTimer;
            comboCounter++;
            ScoreManager.Instance.SetComboText(comboValues[comboCounter - 1].ToString(), transform.position);
            AddScore?.Invoke(comboValues[comboCounter - 1]);
        }

        public override void ChangeDirection(Vector2 direction)
        {
            if ( GetNodeInDirection(direction) != null && GetNodeInDirection(direction).ThinWall )
            {
                return;
            }

            base.ChangeDirection(direction);

            var rotate = currentDirection == Vector2.left ? 180 :
                currentDirection == Vector2.down ? 270 :
                currentDirection == Vector2.up ? 90 : 0;
            transform.localRotation = Quaternion.Euler(0, 0, rotate);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ( !other.transform.CompareTag("Dot") && !other.transform.CompareTag("PowerDot") )
            {
                return;
            }

            if ( other.transform.CompareTag("PowerDot") )
            {
                EatPowerDot?.Invoke();
                Destroy(other.gameObject);
                AddScore?.Invoke(50);
                return;
            }

            Destroy(other.gameObject);
            AddScore?.Invoke(1);
        }
    }
}