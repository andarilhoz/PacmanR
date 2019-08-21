using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _PacmanGame.Scripts.ExtensionHelpers;
using _PacmanGame.Scripts.Pathfind;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public class Pinky : BaseGhost
    {
        public Vector2 PinkyScatterPoint = new Vector2(10,10);
        public const float TILE_OFFSET = .255f;

        protected override void Awake()
        {
            base.Awake();
            scatterPoint = PinkyScatterPoint;
        }

        protected override void Intersection()
        {
            base.Intersection();
            if ( !CurrentState.Equals(GhostState.Chasing) )
            {
                return;
            }
            var intersections = currentNode.nodeIntersections;
            var node = ChooseNode(NodeDistanceFromAheadOfPacman, intersections.Left, intersections.Down, intersections.Right, intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromAheadOfPacman(Node node)
        {
            var aheadOfPacman = (Vector2) PacmanTarget.transform.position + (PacmanTarget.currentDirection * 4 * TILE_OFFSET);
            return Vector2.Distance(node.Position, aheadOfPacman);
        }
    }
}