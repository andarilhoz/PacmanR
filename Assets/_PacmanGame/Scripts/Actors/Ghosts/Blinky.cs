using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _PacmanGame.Scripts.ExtensionHelpers;
using _PacmanGame.Scripts.Pathfind;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public class Blinky : BaseGhost
    {
        public Vector2 BlinkyScatterPoint = new Vector2(10,10);

        protected override void Awake()
        {
            base.Awake();
            scatterPoint = BlinkyScatterPoint;
        }

        protected override void Start()
        {
            base.Start();
            NextState();
        }

        protected override void Intersection()
        {
            base.Intersection();
            if ( !CurrentState.Equals(GhostState.Chasing) )
            {
                return;
            }
            var intersections = currentNode.nodeIntersections;
            var node = ChooseNode(NodeDistanceFromPacman, intersections.Left, intersections.Down, intersections.Right, intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromPacman(Node node)
        {
            return Vector2.Distance(node.Position, PacmanTarget.transform.position);
        }
    }
}