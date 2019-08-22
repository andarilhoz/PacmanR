using System;
using System.Collections.Generic;
using UnityEngine;
using _PacmanGame.Scripts.Map;
using Random = UnityEngine.Random;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    [RequireComponent(typeof(BaseGhostState))]
    public abstract class BaseGhost : Actors
    {
        public Vector2 ScatterPoint { get; set; }

        private readonly Vector3 ghostHousePoint = Vector3.zero;

        protected BaseGhostState GhostState;
        protected static Pacman PacmanTarget;

        private bool choosedPath = false;

        public static event Action<GhostStates> TouchGhost;

        protected new virtual void Awake()
        {
            base.Awake();
            GhostState = GetComponent<BaseGhostState>();
            if ( PacmanTarget != null )
            {
                return;
            }

            PacmanTarget = FindObjectOfType<Pacman>();
        }

        protected static float NodeDistanceFromPacman(Node node) =>
            Vector2.Distance(node.Position, PacmanTarget.transform.position);


        protected new void FixedUpdate()
        {
            base.FixedUpdate();

            if ( LevelManager.Instance.GetCurrentState().Equals(LevelManager.GameState.Pause) )
            {
                return;
            }

            if ( GhostState.CurrentState.Equals(GhostStates.LeavingHouse) )
            {
                LeaveHouse();
            }

            if ( !CurrentNode.IsIntersection )
            {
                choosedPath = false;
                return;
            }


            if ( !CurrentNode.IsIntersection || choosedPath && GhostState.IsInUnstableState() )
            {
                return;
            }

            choosedPath = true;

            Intersection();
        }

        private void OnDestroy()
        {
            TouchGhost = states => { };
        }

        public override void ResetActor()
        {
            base.ResetActor();
            GhostState.ResetActor();
            choosedPath = false;
            Animator.SetBool("IsAfraid", false);
            Animator.SetBool("AfraidLowTime", false);
            Animator.SetBool("Dead", false);
        }

        protected override void GetCurrentNode()
        {
            if ( GhostState.ChangedStateTimer > 0 && PreviousNode.NodeNeighbors != null &&
                 !GhostState.CurrentState.Equals(GhostStates.LeavingHouse) )
            {
                return;
            }

            base.GetCurrentNode();
        }
        
        // will choose the best node given each ghost behaviour
        protected Node ChooseNode(Func<Node, float> compareFunction, params Node[] nodes)
        {
            var validNodes = new List<Node>();

            foreach (var node in nodes)
            {
                if ( node == null )
                {
                    continue;
                }

                if ( node == PreviousNode )
                {
                    continue;
                }

                validNodes.Add(node);
            }

            validNodes.Sort((v1, v2) => compareFunction(v1).CompareTo(compareFunction(v2)));
            return validNodes[0];
        }

        protected Vector2 GetNodeDirection(Node node)
        {
            if ( node == CurrentNode.NodeNeighbors.Left )
            {
                return Vector2.left;
            }

            if ( node == CurrentNode.NodeNeighbors.Right )
            {
                return Vector2.right;
            }

            if ( node == CurrentNode.NodeNeighbors.Up )
            {
                return Vector2.up;
            }

            return Vector2.down;
        }
        
        // everytime the ghost went in an intersection, it has to choose a path based on its state
        private void Intersection()
        {
            switch (GhostState.CurrentState)
            {
                case GhostStates.Afraid:
                    AfraidIntersection();
                    return;
                case GhostStates.Scatter:
                    ScatterIntersection();
                    return;
                case GhostStates.Dead:
                    DeadIntersection();
                    return;
                case GhostStates.LeavingHouse:
                    break;
                case GhostStates.Chasing:
                    ChasingIntersection();
                    break;
            }
        }

        //This class should be override by each individual ghosts
        protected virtual void ChasingIntersection()
        {
        }
        
        
        private void AfraidIntersection()
        {
            var intersections = CurrentNode.NodeNeighbors;
            var chosedNode = ChooseNode(RandomFloat, intersections.Left, intersections.Down, intersections.Right,
                intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private static float RandomFloat(Node node) => Random.value;

        private void ScatterIntersection()
        {
            var intersections = CurrentNode.NodeNeighbors;
            var chosedNode = ChooseNode(NodeDistanceFromScatterPoint, intersections.Left, intersections.Down,
                intersections.Right, intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private void DeadIntersection()
        {
            if ( PreviousNode.ThinWall )
            {
                GhostState.ChangeState(GhostStates.LeavingHouse);
            }

            var intersections = CurrentNode.NodeNeighbors;
            var chosedNode = ChooseNode(NodeDistanceFromGhostHouse, intersections.Left, intersections.Down,
                intersections.Right, intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromGhostHouse(Node node) => Vector2.Distance(node.Position, ghostHousePoint);

        private void LeaveHouse()
        {
            if ( CurrentNode.ThinWall )
            {
                GhostState.NextState();
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

        protected float NodeDistanceFromScatterPoint(Node node) => Vector2.Distance(node.Position, ScatterPoint);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ( !other.transform.CompareTag("Player") )
            {
                return;
            }

            if ( GhostState.CurrentState.Equals(GhostStates.Dead) )
            {
                return;
            }

            TouchGhost?.Invoke(GhostState.CurrentState);
            if ( GhostState.CurrentState.Equals(GhostStates.Afraid) )
            {
                GhostState.ChangeState(GhostStates.Dead);
            }
        }

        public void ResetSpeed()
        {
            CurrentSpeed = BaseSpeed;
        }

        public void MultiplySpeed(float multiplier)
        {
            CurrentSpeed = BaseSpeed * multiplier;
        }

        public void ReverseDirection()
        {
            if ( PreviousNode.NodeNeighbors == null || PreviousNode.ThinWall || CurrentNode.ThinWall )
            {
                return;
            }

            ChangeDirection(GetNodeDirection(PreviousNode));
        }
    }
}