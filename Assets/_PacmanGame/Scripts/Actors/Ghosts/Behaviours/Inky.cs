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
            ScatterPoint = InkyScatterPoint;
        }

        protected override void Start()
        {
            base.Start();
            blinky = FindObjectOfType<Blinky>();
            CurrentDirection = Vector2.right;
            GhostState.InitializeLockedTimer();
        }

        public override void ResetActor()
        {
            base.ResetActor();
            CurrentDirection = Vector2.right;
            GhostState.InitializeLockedTimer();
        }


        protected override void ChasingIntersection()
        {
            var intersections = CurrentNode.NodeIntersections;
            var node = ChooseNode(NodeDistanceFromBlinkyAndPacman, intersections.Left, intersections.Down,
                intersections.Right, intersections.Up);
            var direction = GetNodeDirection(node);
            ChangeDirection(direction);
        }

        private float NodeDistanceFromBlinkyAndPacman(Node node)
        {
            var aheadOfPacman = (Vector2) (PacmanTarget.transform.position - blinky.transform.position) +
                                PacmanTarget.CurrentDirection * 10 * MapGenerator.TILE_OFFSET;
            return Vector2.Distance(node.Position, aheadOfPacman);
        }
    }
}