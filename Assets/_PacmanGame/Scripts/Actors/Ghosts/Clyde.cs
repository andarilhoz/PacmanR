using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _PacmanGame.Scripts.ExtensionHelpers;
using _PacmanGame.Scripts.Pathfind;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public class Clyde : BaseGhost
    {
        public Vector2 PinkyScatterPoint = new Vector2(10,10);
        public const float DISTANCE_OFFSET = .255f * 8;

        protected override void Awake()
        {
            base.Awake();
            scatterPoint = PinkyScatterPoint;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(PacmanTarget.transform.position, DISTANCE_OFFSET);
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(currentNode.Position, Vector2.one * .255f);;
        }

        protected override void Intersection()
        {
            base.Intersection();
            if ( !CurrentState.Equals(GhostState.Chasing) )
            {
                return;
            }
            var intersections = currentNode.nodeIntersections;
            var distanceMethod = ChooseDistanceMethod();
            var node = ChooseNode(distanceMethod, intersections.Left, intersections.Down, intersections.Right, intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromPacman(Node node)
        {
            return Vector2.Distance(node.Position, PacmanTarget.transform.position);
        }

        private Func<Node, float> ChooseDistanceMethod()
        {
            var distanceFromPacman = Vector2.Distance(transform.position, PacmanTarget.transform.position);
            if ( distanceFromPacman > DISTANCE_OFFSET )
            {
                return NodeDistanceFromPacman;
            }

            return NodeDistanceFromScatterPoint;
        }
    }
}