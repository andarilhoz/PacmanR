using System;
using System.Collections.Generic;
using UnityEngine;
using _PacmanGame.Scripts.Pathfind;
using Random = UnityEngine.Random;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public abstract class BaseGhost : Actors
    {
        public Vector2 scatterPoint = Vector2.zero;
        private Vector3 GhostHousePoint = Vector3.zero;
        
        public static event Action<GhostState> TouchGhost;
        
        public GhostState CurrentState;
        private GhostState PreviousState;
        
        protected static Pacman PacmanTarget;

        private bool choosedPath = false;

        private float afraidSpeedMultiplier = .5f;

        protected float modeTimer = Mathf.Infinity;
        protected int stateIteraction = 0;


        private float changedStateTimer = 0;
        private float changedStateCoolDown = .5f; 
         
        protected (GhostState, float)[] StateTimers = new (GhostState, float)[]
        {
            (GhostState.Scatter, 7),
            (GhostState.Chasing, 20),
            (GhostState.Scatter, 7),
            (GhostState.Chasing, 20),
            (GhostState.Scatter, 5),
            (GhostState.Chasing, 20),
            (GhostState.Scatter, 5),
            (GhostState.Chasing, 20),
        };

        private float AfraidTime = 6f;

        protected new virtual void Awake()
        {
            base.Awake();
            
            if ( PacmanTarget != null ) return;
            PacmanTarget = FindObjectOfType<Pacman>();
        }

        protected new virtual void Start()
        {
            base.Start();
            ChangeState(GhostState.Locked);
            Pacman.EatPowerDot += () => ChangeState(GhostState.Afraid);
        }

        protected void NextState()
        {
            if ( stateIteraction >= StateTimers.Length )
            {
                ChangeState(GhostState.Chasing);
                modeTimer = Mathf.Infinity;
                return;
            }

            ChangeState(StateTimers[stateIteraction].Item1);
            modeTimer = StateTimers[stateIteraction].Item2;
            stateIteraction++;
        }


        protected void ChangeState(GhostState newState)
        {
            if ( newState == CurrentState )
            {
                if ( newState.Equals(GhostState.Afraid) )
                {
                    animator.SetBool("AfraidLowTime", false);
                    modeTimer = AfraidTime;
                }

                return;
            }

            if ( CurrentState.Equals(GhostState.Afraid) )
            {
                animator.SetBool("IsAfraid", false);
                animator.SetBool("AfraidLowTime", false);
                currentSpeed = speed;
            }

            if ( CurrentState.Equals(GhostState.Dead) )
            {
                animator.SetBool("Dead", false);
                currentSpeed = speed;
            }

            if ( newState.Equals(GhostState.Afraid) && (CurrentState.Equals(GhostState.Dead) || CurrentState.Equals(GhostState.Locked)) )
            {
                return;
            }

            ChangeDirection(GetNodeDirection(previousNode));
            changedStateTimer = changedStateCoolDown;
            switch (newState)
            {
                case GhostState.Locked:
                    break;
                case GhostState.Scatter:
                    break;
                case GhostState.Chasing:
                    break;
                case GhostState.Afraid:
                    animator.SetBool("IsAfraid", true);
                    currentSpeed = speed * afraidSpeedMultiplier;
                    modeTimer = AfraidTime;
                    break;
                case GhostState.Dead:
                    animator.SetBool("Dead", true);
                    currentSpeed = speed * 4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            PreviousState = CurrentState;
            CurrentState = newState;
        }

        protected new void FixedUpdate()
        {
            base.FixedUpdate();
            
            UpdateStateTimer();
            if(CurrentState.Equals(GhostState.Locked)) LeaveHouse();
            if ( !currentNode.IsIntersection )
            {
                choosedPath = false;
                return;
            }

            var unstableStates = CurrentState.Equals(GhostState.Chasing) || CurrentState.Equals(GhostState.Afraid);  

            if ( !currentNode.IsIntersection || (choosedPath && unstableStates))
            {
                return;
            }

            choosedPath = true;
            Intersection();
        }
        
        protected override void GetCurrentNode()
        {
            if ( changedStateTimer > 0 )
            {
                return;
            }
            base.GetCurrentNode();
        }

        private void UpdateStateTimer()
        {
            if ( CurrentState.Equals(GhostState.Afraid) && modeTimer < AfraidTime * 0.3f )
            {
                animator.SetBool("AfraidLowTime", true);
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
        
        protected Node ChooseNode( Func<Node, float> compareFunction, params Node[] nodes)
        {
            var validNodes = new List<Node>();
            
            foreach (var node in nodes)
            {
                if ( node == null ) continue;
                if( node == previousNode) continue;
                validNodes.Add(node);
            }
            validNodes.Sort((v1,v2) => compareFunction(v1).CompareTo(compareFunction(v2)));
            return validNodes[0];
        }
        
        protected Vector2 GetNodeDirection(Node node)
        {
            if ( node == currentNode.nodeIntersections.Left )
            {
                return Vector2.left;
            }
            
            if ( node == currentNode.nodeIntersections.Right )
            {
                return Vector2.right;
            }
            
            if ( node == currentNode.nodeIntersections.Up )
            {
                return Vector2.up;
            }

            return Vector2.down;

        }

        protected virtual void Intersection()
        {
            switch (CurrentState)
            {
                case GhostState.Afraid:
                    AfraidIntersection();
                    return;
                case GhostState.Scatter:
                    ScatterIntersection();
                    return;
                case GhostState.Dead:
                    DeadIntersection();
                    return;
            }
        }

        private void AfraidIntersection()
        {
            var intersections = currentNode.nodeIntersections;
            var chosedNode = ChooseNode( PseudoRandomFloat, intersections.Left, intersections.Down, intersections.Right, intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private float PseudoRandomFloat(Node node)
        {
            return Random.value;
        }

        private void ScatterIntersection()
        {
            var intersections = currentNode.nodeIntersections;
            var chosedNode = ChooseNode( NodeDistanceFromScatterPoint, intersections.Left, intersections.Down, intersections.Right, intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }
        
        private void DeadIntersection()
        {
            if ( previousNode.ThinWall )
            {
                ChangeState(GhostState.Locked);
            }
            var intersections = currentNode.nodeIntersections;
            var chosedNode = ChooseNode( NodeDistanceFromGhostHouse, intersections.Left, intersections.Down, intersections.Right, intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromGhostHouse(Node node)
        {
            return Vector2.Distance(node.Position, GhostHousePoint);
        }

        private void LeaveHouse()
        {
            if ( currentNode.ThinWall )
            {
                NextState();
            }

            if ( IsDirectionAvailable(Vector2.up) )
            {
                ChangeDirection(Vector2.up);
                return;
            }

            if ( IsDirectionAvailable(Vector2.left) )
            {
                ChangeDirection(Vector2.left);
                return;
            }
            
            ChangeDirection(Vector2.right);
        }

        protected float NodeDistanceFromScatterPoint(Node node)
        {
            return Vector2.Distance(node.Position, scatterPoint);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ( !other.transform.CompareTag("Player") )
            {
                return;
            }

            if ( CurrentState.Equals(GhostState.Dead) )
            {
                return;
            }

            TouchGhost?.Invoke(CurrentState);

            if ( CurrentState.Equals(GhostState.Afraid) )
            {
                ChangeState(GhostState.Dead);
                modeTimer = Mathf.Infinity;
            }
        }
    }
}