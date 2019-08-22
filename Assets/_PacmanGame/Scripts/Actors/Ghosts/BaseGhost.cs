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
        public Vector2 scatterPoint { get; set; }

        private readonly Vector3 GhostHousePoint = Vector3.zero;

        protected BaseGhostState ghostState;
        protected static Pacman PacmanTarget;

        private bool choosedPath = false;

        public static event Action<GhostStates> TouchGhost;

        protected new virtual void Awake()
        {
            base.Awake();
            ghostState = GetComponent<BaseGhostState>();
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
            
            if ( LevelManager.Instance.CurrentGameState.Equals(LevelManager.GameState.Pause) )
            {
                return;
            }

            if ( ghostState.CurrentStates.Equals(GhostStates.LeavingHouse) )
            {
                LeaveHouse();
            }

            if ( !currentNode.IsIntersection )
            {
                choosedPath = false;
                return;
            }


            if ( !currentNode.IsIntersection || choosedPath && ghostState.IsInUnstableStates() )
            {
                return;
            }

            choosedPath = true;

            Intersection();
        }

        protected override void GetCurrentNode()
        {
            if ( ghostState.changedStateTimer > 0 && previousNode.NodeIntersections != null  && !ghostState.CurrentStates.Equals(GhostStates.LeavingHouse))
            {
                return;
            }

            base.GetCurrentNode();
        }

        protected Node ChooseNode(Func<Node, float> compareFunction, params Node[] nodes)
        {
            var validNodes = new List<Node>();

            foreach (var node in nodes)
            {
                if ( node == null )
                {
                    continue;
                }

                if ( node == previousNode )
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
            if ( node == currentNode.NodeIntersections.Left )
            {
                return Vector2.left;
            }

            if ( node == currentNode.NodeIntersections.Right )
            {
                return Vector2.right;
            }

            if ( node == currentNode.NodeIntersections.Up )
            {
                return Vector2.up;
            }

            return Vector2.down;
        }

        protected virtual void Intersection()
        {
            switch (ghostState.CurrentStates)
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
            var intersections = currentNode.NodeIntersections;
            var chosedNode = ChooseNode(PseudoRandomFloat, intersections.Left, intersections.Down, intersections.Right,
                intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private float PseudoRandomFloat(Node node) => Random.value;

        private void ScatterIntersection()
        {
            var intersections = currentNode.NodeIntersections;
            var chosedNode = ChooseNode(NodeDistanceFromScatterPoint, intersections.Left, intersections.Down,
                intersections.Right, intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private void DeadIntersection()
        {
            if ( previousNode.ThinWall )
            {
                ghostState.ChangeState(GhostStates.LeavingHouse);
            }

            var intersections = currentNode.NodeIntersections;
            var chosedNode = ChooseNode(NodeDistanceFromGhostHouse, intersections.Left, intersections.Down,
                intersections.Right, intersections.Up);
            var direction = GetNodeDirection(chosedNode);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromGhostHouse(Node node) => Vector2.Distance(node.Position, GhostHousePoint);

        private void LeaveHouse()
        {
            if ( currentNode.ThinWall )
            {
                ghostState.NextState();
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

        protected float NodeDistanceFromScatterPoint(Node node) => Vector2.Distance(node.Position, scatterPoint);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ( !other.transform.CompareTag("Player") )
            {
                return;
            }

            if ( ghostState.CurrentStates.Equals(GhostStates.Dead) )
            {
                return;
            }

            TouchGhost?.Invoke(ghostState.CurrentStates);
            if ( ghostState.CurrentStates.Equals(GhostStates.Afraid) )
            {
                ghostState.ChangeState(GhostStates.Dead);
            }
        }

        public void ResetSpeed()
        {
            currentSpeed = baseSpeed;
        }

        public void MultiplySpeed(float multiplier)
        {
            currentSpeed = baseSpeed * multiplier;
        }

        public void ReverseDirection()
        {
            if ( previousNode.NodeIntersections == null )
            {
                return;
            }

            ChangeDirection(GetNodeDirection(previousNode));
        }
    }
}