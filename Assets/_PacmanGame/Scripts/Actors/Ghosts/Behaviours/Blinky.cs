using UnityEngine;

namespace _PacmanGame.Scripts.Actors.Ghosts.Behaviours
{
    public class Blinky : BaseGhost
    {
        public Vector2 BlinkyScatterPoint = new Vector2(10, 10);

        protected override void Awake()
        {
            base.Awake();
            ScatterPoint = BlinkyScatterPoint;
        }

        protected override void Start()
        {
            base.Start();
            GhostState.NextState();
        }

        public override void ResetActor()
        {
            base.ResetActor();
            ScatterPoint = BlinkyScatterPoint;
            GhostState.NextState();
        }

        protected override void ChasingIntersection()
        {
            var intersections = CurrentNode.NodeNeighbors;
            var node = ChooseNode(NodeDistanceFromPacman, intersections.Left, intersections.Down, intersections.Right,
                intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }
    }
}