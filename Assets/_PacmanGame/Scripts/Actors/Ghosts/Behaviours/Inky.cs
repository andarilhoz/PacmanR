using UnityEngine;
using _PacmanGame.Scripts.Map;

namespace _PacmanGame.Scripts.Actors.Ghosts.Behaviours
{
    public class Inky : BaseGhost
    {
        public Vector2 InkyScatterPoint = new Vector2(10, 10);

        private Blinky blinky;

        protected override void Awake()
        {
            base.Awake();
            scatterPoint = InkyScatterPoint;
        }

        protected override void Start()
        {
            base.Start();
            blinky = FindObjectOfType<Blinky>();
            currentDirection = Vector2.right;
            ghostState.InitializeLockedTimer();
        }

        protected override void ChasingIntersection()
        {
            var intersections = currentNode.NodeIntersections;
            var node = ChooseNode(NodeDistanceFromBlinkyAndPacman, intersections.Left, intersections.Down,
                intersections.Right, intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromBlinkyAndPacman(Node node)
        {
            var aheadOfPacman = (Vector2) (PacmanTarget.transform.position - blinky.transform.position) +
                                PacmanTarget.currentDirection * 10 * MapGenerator.TILE_OFFSET;
            return Vector2.Distance(node.Position, aheadOfPacman);
        }
    }
}