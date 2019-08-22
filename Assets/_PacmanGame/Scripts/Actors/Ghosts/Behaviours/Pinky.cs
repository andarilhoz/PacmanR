using UnityEngine;
using _PacmanGame.Scripts.Map;

namespace _PacmanGame.Scripts.Actors.Ghosts.Behaviours
{
    public class Pinky : BaseGhost
    {
        public Vector2 PinkyScatterPoint = new Vector2(10, 10);

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

        protected override void ChasingIntersection()
        {
            var intersections = currentNode.NodeIntersections;
            var node = ChooseNode(NodeDistanceFromAheadOfPacman, intersections.Left, intersections.Down,
                intersections.Right, intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromAheadOfPacman(Node node)
        {
            var aheadOfPacman = (Vector2) PacmanTarget.transform.position +
                                PacmanTarget.currentDirection * 4 * MapGenerator.TILE_OFFSET;
            return Vector2.Distance(node.Position, aheadOfPacman);
        }
    }
}