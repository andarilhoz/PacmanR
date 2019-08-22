using System;
using UnityEngine;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    [RequireComponent(typeof(BaseGhost))]
    public class BaseGhostState : MonoBehaviour
    {
        public GhostStates CurrentStates;
        private GhostStates previousStates;
        private float modeTimer = Mathf.Infinity;
        private int stateIteraction = 0;

        public float changedStateTimer { get; set; }
        private const float CHANGE_STATE_COOLDOWN = .5f;

        private const float DEAD_SPEED_MULTIPLIER = 4f;
        private const float AFRAID_SPEED_MULTIPLIER = .5f;
        private const float AFRAID_TIME = 6f;

        private BaseGhost baseGhost;

        private readonly (GhostStates, float)[] StateTimers =
        {
            (GhostStates.Scatter, 7), (GhostStates.Chasing, 20), (GhostStates.Scatter, 7),
            (GhostStates.Chasing, 20), (GhostStates.Scatter, 5), (GhostStates.Chasing, 20),
            (GhostStates.Scatter, 5), (GhostStates.Chasing, 20),
        };

        private void Awake()
        {
            baseGhost = GetComponent<BaseGhost>();
        }

        private void Start()
        {
            Pacman.EatPowerDot += () => ChangeState(GhostStates.Afraid);
        }

        private void Update()
        {
            if ( LevelManager.Instance.CurrentGameState.Equals(LevelManager.GameState.Pause) )
            {
                return;
            }
            UpdateStateTimer();
        }

        public bool IsInUnstableStates() =>
            CurrentStates.Equals(GhostStates.Chasing) || CurrentStates.Equals(GhostStates.Afraid);

        public void NextState()
        {
            if ( stateIteraction >= StateTimers.Length )
            {
                ChangeState(GhostStates.Chasing);
                modeTimer = Mathf.Infinity;
                return;
            }

            ChangeState(StateTimers[stateIteraction].Item1);
            modeTimer = StateTimers[stateIteraction].Item2;
            stateIteraction++;
        }

        public void ChangeState(GhostStates newStates)
        {
            if ( newStates == CurrentStates )
            {
                if ( !newStates.Equals(GhostStates.Afraid) )
                {
                    return;
                }

                baseGhost.animator.SetBool("AfraidLowTime", false);
                modeTimer = AFRAID_TIME;
                return;
            }

            if ( newStates.Equals(GhostStates.Afraid) &&
                 (CurrentStates.Equals(GhostStates.Dead) || CurrentStates.Equals(GhostStates.Locked)) )
            {
                return;
            }

            switch (CurrentStates)
            {
                case GhostStates.Afraid:
                    baseGhost.animator.SetBool("IsAfraid", false);
                    baseGhost.animator.SetBool("AfraidLowTime", false);
                    baseGhost.ResetSpeed();
                    break;
                case GhostStates.Dead:
                    baseGhost.animator.SetBool("Dead", false);
                    baseGhost.ResetSpeed();
                    break;
            }

            baseGhost.ReverseDirection();

            changedStateTimer = CHANGE_STATE_COOLDOWN;
            switch (newStates)
            {
                case GhostStates.Locked:
                    break;
                case GhostStates.Scatter:
                    break;
                case GhostStates.Chasing:
                    break;
                case GhostStates.Afraid:
                    baseGhost.animator.SetBool("IsAfraid", true);
                    baseGhost.MultiplySpeed(AFRAID_SPEED_MULTIPLIER);
                    modeTimer = AFRAID_TIME;
                    break;
                case GhostStates.Dead:
                    baseGhost.animator.SetBool("Dead", true);
                    baseGhost.MultiplySpeed(DEAD_SPEED_MULTIPLIER);
                    modeTimer = Mathf.Infinity;
                    break;
            }

            previousStates = CurrentStates;
            CurrentStates = newStates;
        }

        private void UpdateStateTimer()
        {
            if ( CurrentStates.Equals(GhostStates.Afraid) && modeTimer < AFRAID_TIME * 0.3f )
            {
                baseGhost.animator.SetBool("AfraidLowTime", true);
            }

            if ( changedStateTimer > 0 )
            {
                changedStateTimer -= Time.deltaTime;
            }

            if ( modeTimer > 0 )
            {
                modeTimer -= Time.deltaTime;
                return;
            }

            NextState();
        }
    }
}