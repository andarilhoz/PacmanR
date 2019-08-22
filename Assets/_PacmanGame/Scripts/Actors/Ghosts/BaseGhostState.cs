using UnityEngine;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    [RequireComponent(typeof(BaseGhost))]
    public class BaseGhostState : MonoBehaviour
    {
        public GhostStates CurrentState;
        public float lockedTime;
        
        private GhostStates previousStates;
        private float modeTimer;
        private float initialModeTimer;
        private int stateIteraction = 0;

        public float changedStateTimer { get; set; }
        private const float CHANGE_STATE_COOLDOWN = .5f;

        private const float DEAD_SPEED_MULTIPLIER = 4f;
        private const float AFRAID_SPEED_MULTIPLIER = .5f;
        private const float AFRAID_TIME = 6f;

        private BaseGhost baseGhost;
        private SpriteRenderer spriteRender;

        private GhostStates initialState;

        private readonly (GhostStates, float)[] StateTimers =
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
            if ( LevelManager.Instance.CurrentGameState.Equals(LevelManager.GameState.Pause) )
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

        public void InitializeLockedTimer()
        {
            modeTimer = lockedTime;
        }

        public bool IsInUnstableStates() =>
            CurrentState.Equals(GhostStates.Chasing) || CurrentState.Equals(GhostStates.Afraid);

        public void NextState()
        {
            if ( CurrentState.Equals(GhostStates.InHouse) )
            {
                ChangeState(GhostStates.LeavingHouse);
                return;
            }
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
            if ( newStates == CurrentState )
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
                 (CurrentState.Equals(GhostStates.Dead) || CurrentState.Equals(GhostStates.LeavingHouse)) )
            {
                return;
            }

            switch (CurrentState)
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
                case GhostStates.LeavingHouse:
                    modeTimer = Mathf.Infinity;
                    break;
                case GhostStates.Scatter:
                    break;
                case GhostStates.Chasing:
                    break;
                case GhostStates.Afraid:
                    if ( InsideHouseStates() ) return;
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

            previousStates = CurrentState;
            CurrentState = newStates;
        }
        
        private bool InsideHouseStates()
        {
            return CurrentState.Equals( GhostStates.InHouse) || CurrentState.Equals(GhostStates.LeavingHouse );
        }

        private void UpdateStateTimer()
        {
            if ( CurrentState.Equals(GhostStates.Afraid) && modeTimer < AFRAID_TIME * 0.3f )
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