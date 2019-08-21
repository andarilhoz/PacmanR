using UnityEngine;
using _PacmanGame.Scripts.Pathfind;

namespace _PacmanGame.Scripts.Actors.Ghosts
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
            ghostState.ChangeState(GhostStates.Locked);
            blinky = FindObjectOfType<Blinky>();
        }

        protected override void ChasingIntersection()
        {
            var intersections = currentNode.nodeIntersections;
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