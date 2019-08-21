using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _PacmanGame.Scripts.ExtensionHelpers;
using _PacmanGame.Scripts.Pathfind;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public class Inky : BaseGhost
    {
        public Vector2 InkyScatterPoint = new Vector2(10,10);
        public const float TILE_OFFSET = .255f;

        private Blinky blinky;
        protected override void Awake()
        {
            base.Awake();
            scatterPoint = InkyScatterPoint;
        }
        
        protected override void Start(){
            base.Start();
            blinky = FindObjectOfType<Blinky>();
        }

        protected override void Intersection()
        {
            base.Intersection();
            if ( !CurrentState.Equals(GhostState.Chasing) )
            {
                return;
            }
            var intersections = currentNode.nodeIntersections;
            var node = ChooseNode(NodeDistanceFromBlinkyAndPacman, intersections.Left, intersections.Down, intersections.Right, intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromBlinkyAndPacman(Node node)
        {
            var aheadOfPacman = (Vector2) (PacmanTarget.transform.position - blinky.transform.position) + (PacmanTarget.currentDirection * 10 * TILE_OFFSET);
            return Vector2.Distance(node.Position, aheadOfPacman);
        }
    }
}