using System;
using System.Threading.Tasks;
using UnityEngine;
using _PacmanGame.Scripts.Actors.Ghosts;
using _PacmanGame.Scripts.Canvas;
using _PacmanGame.Scripts.InputSystem;
using _PacmanGame.Scripts.Utils;

namespace _PacmanGame.Scripts.Actors
{
    public class Pacman : Actors
    {
        public static event Action EatPowerDot;
        public static event Action Die;
        public static event Action EatDot;
        public static event Action EatGhost;
        public static event Action EatFruit;
        public static event Action<int> AddScore;

        public float GhostEatComboTimer = 2f;
        private float comboTimer = 0;
        private int comboCounter = 0;
        private readonly int[] comboValues = {200, 400, 800, 1600};

        private Vector2 lastDirectionBuffer;
        private const float CHANGE_DIRECTION_TIMEOUT = 0.3f;
        private float lastDirectionTimeout = 0;

        private const float DIE_TIMER = .5f;

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
            ChangeDirection(CurrentDirection);
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
            Animator.SetBool("IsAlive", true);
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

            LevelManager.Instance.PauseGame();
#if UNITY_WEBGL
            StartCoroutine(CoroutineUtils.WaitSecondsCoroutine(DIE_TIMER, () =>
            {
                Die?.Invoke();
                Animator.SetBool("IsAlive", false);
            }));
#else
            await Task.Delay(TimeSpan.FromSeconds(DIE_TIMER));
            Die?.Invoke();
            Animator.SetBool("IsAlive", false);
#endif
        }

        private void EatGhostCombo()
        {
            comboTimer = GhostEatComboTimer;
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
                lastDirectionTimeout = CHANGE_DIRECTION_TIMEOUT;
            }

            if ( !IsDirectionAvailable(direction) )
            {
                return;
            }

            base.ChangeDirection(direction);

            var rotate = CurrentDirection == Vector2.left ? 180 :
                CurrentDirection == Vector2.down ? 270 :
                CurrentDirection == Vector2.up ? 90 : 0;
            transform.localRotation = Quaternion.Euler(0, 0, rotate);
        }

        /// <summary>
        /// This method trys to repeat player move to give a more fluid feeling. 
        /// </summary>
        private void TryAgainChangeDirection()
        {
            if ( !CurrentNode.IsIntersection )
            {
                return;
            }

            if ( lastDirectionTimeout <= 0 )
            {
                lastDirectionTimeout = 0;
                return;
            }

            ChangeDirection(lastDirectionBuffer);
            lastDirectionTimeout -= Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other) //The Eating actions are made here.
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