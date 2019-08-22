using System;
using UnityEngine;
using _PacmanGame.Scripts.Map;

namespace _PacmanGame.Scripts.Actors.Ghosts.Behaviours
{
    public class Clyde : BaseGhost
    {
        public Vector2 PinkyScatterPoint = new Vector2(10, 10);
        private const float DISTANCE_OFFSET = MapGenerator.TILE_OFFSET * 8;

        protected override void Awake()
        {
            base.Awake();
            scatterPoint = PinkyScatterPoint;
        }

        protected override void Start()
        {
            base.Start();
            ghostState.InitializeLockedTimer();
        }
        
        public override void ResetActor()
        {
            base.ResetActor();
            ghostState.InitializeLockedTimer();
        }

        protected override void ChasingIntersection()
        {
            var intersections = currentNode.NodeIntersections;
            var distanceMethod = ChooseDistanceMethod();
            var node = ChooseNode(distanceMethod, intersections.Left, intersections.Down, intersections.Right,
                intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
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