using UnityEngine;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    [RequireComponent(typeof(BaseGhost))]
    public class BaseGhostState : MonoBehaviour
    {
        public GhostStates CurrentState;
        public float LockedTime;

        private GhostStates previousStates;
        private float modeTimer;
        private float initialModeTimer;
        private int stateIteraction = 0;

        public float ChangedStateTimer { get; set; }
        private const float CHANGE_STATE_COOLDOWN = .5f;

        private const float DEAD_SPEED_MULTIPLIER = 4f;
        private const float AFRAID_SPEED_MULTIPLIER = .5f;
        private const float AFRAID_TIME = 6f;

        private BaseGhost baseGhost;
        private SpriteRenderer spriteRender;

        private GhostStates initialState;

        private readonly (GhostStates, float)[] stateTimers =
        {
            (GhostStates.Scatter, 7), (GhostStates.Chasing, 20), (GhostStates.Scatter, 7),
            (GhostStates.Chasing, 20), (GhostStates.Scatter, 5), (GhostStates.Chasing, 20),
            (GhostStates.Scatter, 5), (GhostStates.Chasing, 20),
        };

        private void Awake()
        {
            baseGhost = GetComponent<BaseGhost>();
            spriteRender = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Pacman.EatPowerDot += () => ChangeState(GhostStates.Afraid);
            Pacman.Die += () => spriteRender.enabled = false;
            initialModeTimer = modeTimer;
            initialState = CurrentState;
        }

        private void Update()
        {
            if ( LevelManager.Instance.GetCurrentState().Equals(LevelManager.GameState.Pause) )
            {
                return;
            }

            UpdateStateTimer();
        }

        public void ResetActor()
        {
            CurrentState = initialState;
            previousStates = GhostStates.InHouse;
            stateIteraction = 0;
            modeTimer = initialModeTimer;
            spriteRender.enabled = true;
        }
        
        //The timer before ghosts are alowed to leave the square
        public void InitializeLockedTimer()
        {
            modeTimer = LockedTime;
        }
        
        // states that could change on a brief moment
        public bool IsInUnstableState() =>
            CurrentState.Equals(GhostStates.Chasing) || CurrentState.Equals(GhostStates.Afraid);
        
        
        //choose the next state based on the stateTimers list
        public void NextState()
        {
            if ( CurrentState.Equals(GhostStates.InHouse) )
            {
                ChangeState(GhostStates.LeavingHouse);
                return;
            }

            if ( stateIteraction >= stateTimers.Length )
            {
                ChangeState(GhostStates.Chasing);
                modeTimer = Mathf.Infinity;
                return;
            }

            ChangeState(stateTimers[stateIteraction].Item1);
            modeTimer = stateTimers[stateIteraction].Item2;
            stateIteraction++;
        }
        
        public void ChangeState(GhostStates newStates)
        {
            if ( newStates == CurrentState )
            {
                if ( !newStates.Equals(GhostStates.Afraid) )
                {
                    return;
                }

                baseGhost.Animator.SetBool("AfraidLowTime", false);
                modeTimer = AFRAID_TIME;
                return;
            }

            if ( newStates.Equals(GhostStates.Afraid) &&
                 (CurrentState.Equals(GhostStates.Dead) || CurrentState.Equals(GhostStates.LeavingHouse)) )
            {
                return;
            }

            switch (CurrentState)
            {
                case GhostStates.Afraid:
                    baseGhost.Animator.SetBool("IsAfraid", false);
                    baseGhost.Animator.SetBool("AfraidLowTime", false);
                    baseGhost.ResetSpeed();
                    break;
                case GhostStates.Dead:
                    baseGhost.Animator.SetBool("Dead", false);
                    baseGhost.ResetSpeed();
                    break;
            }

            baseGhost.ReverseDirection();

            ChangedStateTimer = CHANGE_STATE_COOLDOWN;
            switch (newStates)
            {
                case GhostStates.LeavingHouse:
                    modeTimer = Mathf.Infinity;
                    break;
                case GhostStates.Scatter:
                    break;
                case GhostStates.Chasing:
                    break;
                case GhostStates.Afraid:
                    if ( InsideHouseStates() )
                    {
                        return;
                    }

                    baseGhost.Animator.SetBool("IsAfraid", true);
                    baseGhost.MultiplySpeed(AFRAID_SPEED_MULTIPLIER);
                    modeTimer = AFRAID_TIME;
                    break;
                case GhostStates.Dead:
                    baseGhost.Animator.SetBool("Dead", true);
                    baseGhost.MultiplySpeed(DEAD_SPEED_MULTIPLIER);
                    modeTimer = Mathf.Infinity;
                    break;
            }

            previousStates = CurrentState;
            CurrentState = newStates;
        }
        
        // inside middle square
        private bool InsideHouseStates() =>
            CurrentState.Equals(GhostStates.InHouse) || CurrentState.Equals(GhostStates.LeavingHouse);

        private void UpdateStateTimer()
        {
            if ( CurrentState.Equals(GhostStates.Afraid) && modeTimer < AFRAID_TIME * 0.3f )
            {
                baseGhost.Animator.SetBool("AfraidLowTime", true);
            }

            if ( ChangedStateTimer > 0 )
            {
                ChangedStateTimer -= Time.deltaTime;
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