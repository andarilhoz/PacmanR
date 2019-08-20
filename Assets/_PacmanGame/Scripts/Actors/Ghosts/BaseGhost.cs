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
        
        public GhostState CurrentState;
        private GhostState PreviousState;
        
        protected static Pacman PacmanTarget;

        private bool choosedPath = false;

        private float afraidSpeedMultiplier = .5f;

        protected float modeTimer = 0;
        protected int stateIteraction = 0;
         
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
                return;
            }

            if ( CurrentState.Equals(GhostState.Afraid) )
            {
                animator.SetBool("IsAfraid", false);
                currentSpeed = speed;
            }

            if ( newState.Equals(GhostState.Afraid) && (CurrentState.Equals(GhostState.Dead) || CurrentState.Equals(GhostState.Locked)) )
            {
                return;
            }

            ChangeDirection(GetNodeDirection(previousNode));
            
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
            if ( !currentNode.IsIntersection )
            {
                choosedPath = false;
                return;
            }

            if ( !currentNode.IsIntersection || choosedPath )
            {
                return;
            }

            choosedPath = true;
            Intersection();
        }

        private void UpdateStateTimer()
        {
            if ( modeTimer <= 0 )
            {
                NextState();
                return;
            }

            modeTimer -= Time.deltaTime;
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
            if ( CurrentState.Equals(GhostState.Afraid) )
            {
                AfraidIntersection();
                return;
            }

            if ( CurrentState.Equals(GhostState.Scatter) )
            {
                ScatterIntersection();
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

        private float NodeDistanceFromScatterPoint(Node node)
        {
            return Vector2.Distance(node.Position, scatterPoint);
        }

    }
}