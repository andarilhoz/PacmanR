using UnityEngine;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public class Blinky : BaseGhost
    {
        public Vector2 BlinkyScatterPoint = new Vector2(10, 10);

        protected override void Awake()
        {
            base.Awake();
            scatterPoint = BlinkyScatterPoint;
        }

        protected override void Start()
        {
            base.Start();
            ghostState.NextState();
        }

        protected override void ChasingIntersection()
        {
            var intersections = currentNode.nodeIntersections;
            var node = ChooseNode(NodeDistanceFromPacman, intersections.Left, intersections.Down, intersections.Right,
                intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }
    }
}