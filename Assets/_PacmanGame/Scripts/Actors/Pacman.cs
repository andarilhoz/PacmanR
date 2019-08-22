using System;
using System.Threading.Tasks;
using UnityEngine;
using _PacmanGame.Scripts.Actors.Ghosts;
using _PacmanGame.Scripts.Canvas.Score;
using _PacmanGame.Scripts.InputSystem;

namespace _PacmanGame.Scripts.Actors
{
    public class Pacman : Actors
    {
        public bool isAlive;

        public static event Action EatPowerDot;
        public static event Action Die;
        public static event Action EatDot;
        public static event Action EatGhost;
        public static event Action EatFruit;
        public static event Action<int> AddScore;

        public float ghostEatComboTimer = 2f;
        private float comboTimer = 0;
        private int comboCounter = 0;
        private readonly int[] comboValues = {200, 400, 800, 1600};

        private Vector2 lastDirectionBuffer;
        private float changeDirectionTimeout = 0.3f;
        private float lastDirectionTimeout = 0;

        protected override void Awake()
        {
            base.Awake();
            KeyboardInput.Instance.OnInput += ChangeDirection;
            TouchInput.Instance.OnInput += ChangeDirection;
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

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            TryAgainChangeDirection();
        }
        
        private void OnDestroy()
        {
            EatPowerDot = () => { };
            Die = () => { };
            EatDot = () => { };
            EatGhost = () => { };
            EatFruit = () => { };
            AddScore = (n) => { };
        }

        public override void ResetActor()
        {
            base.ResetActor();
            animator.SetBool("IsAlive", true);
            ChangeDirection(Vector2.left);
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

        private async void TouchedGhost(GhostStates ghostStates)
        {
            if ( ghostStates.Equals(GhostStates.Afraid) )
            {
                EatGhostCombo();
                return;
            }

            LevelManager.Instance.CurrentGameState = LevelManager.GameState.Pause;
            await Task.Delay(TimeSpan.FromSeconds(.5f));
            Die?.Invoke();
            animator.SetBool("IsAlive", false);
            Debug.Log("Ouch i'm dead");
        }

        private void EatGhostCombo()
        {
            comboTimer = ghostEatComboTimer;
            comboCounter++;
            EatGhost?.Invoke();
            ScoreManager.Instance.SetComboText(comboValues[comboCounter - 1].ToString(), transform.position, true);
            AddScore?.Invoke(comboValues[comboCounter - 1]);
        }

        public override void ChangeDirection(Vector2 direction)
        {
            if ( GetNodeInDirection(direction) != null && GetNodeInDirection(direction).ThinWall )
            {
                return;
            }


            lastDirectionBuffer = direction;
            if ( lastDirectionTimeout <= 0 )
            {
                lastDirectionTimeout = changeDirectionTimeout;
            }

            if ( !IsDirectionAvailable(direction) )
            {
                return;
            }

            base.ChangeDirection(direction);

            var rotate = currentDirection == Vector2.left ? 180 :
                currentDirection == Vector2.down ? 270 :
                currentDirection == Vector2.up ? 90 : 0;
            transform.localRotation = Quaternion.Euler(0, 0, rotate);
        }

        /// <summary>
        /// This method trys to repeat player move to give a more fluid feeling. 
        /// </summary>
        private void TryAgainChangeDirection()
        {
            if ( lastDirectionTimeout <= 0 )
            {
                lastDirectionTimeout = 0;
                return;
            }

            ChangeDirection(lastDirectionBuffer);
            lastDirectionTimeout -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            if ( other.transform.CompareTag("PowerDot") )
            {
                EatPowerDot?.Invoke();
                EatDot?.Invoke();    
                Destroy(other.gameObject);
                AddScore?.Invoke(50);
                return;
            }

            if ( other.transform.CompareTag("Fruit") )
            {
                EatFruit?.Invoke();
                AddScore?.Invoke(100);
                ScoreManager.Instance.SetComboText("100", transform.position, false);
                Destroy(other.gameObject);
                return;
            }

            if ( other.transform.CompareTag("Dot") )
            {
                Destroy(other.gameObject);
                AddScore?.Invoke(1);
                EatDot?.Invoke();    
            }

            return;

        }
    }
}