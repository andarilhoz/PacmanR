using UnityEngine;
using _PacmanGame.Scripts.Map;

namespace _PacmanGame.Scripts.Actors
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Actors : MonoBehaviour
    {
        public Node PreviousNode;
        public Node CurrentNode;
        public Node NextNode;

        public float BaseSpeed;
        protected float CurrentSpeed;
        public Vector2 CurrentDirection = Vector2.up;
        public Animator Animator;

        private Rigidbody2D rb2D;

        public Node StartingNode;

        protected virtual void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            CurrentNode = LevelManager.Instance.LevelGrid.NodeFromWorldPostiion(transform.position);
        }

        protected virtual void Start()
        {
            Animator.SetFloat("DirX", CurrentDirection.x);
            Animator.SetFloat("DirY", CurrentDirection.y);
            CurrentSpeed = BaseSpeed;
        }

        public virtual void ResetActor()
        {
            transform.localPosition = StartingNode.Position;
            CurrentNode = StartingNode;
            CurrentSpeed = BaseSpeed;
        }

        protected virtual void FixedUpdate()
        {
            if ( LevelManager.Instance.GetCurrentState().Equals(LevelManager.GameState.Pause) )
            {
                return;
            }

            GetCurrentNode();
            SetNextNodeBasedOnDirection();
            MoveActor();
        }

        public virtual void ChangeDirection(Vector2 direction)
        {
            if ( direction == CurrentDirection )
            {
                return;
            }

            CurrentDirection = direction;
            Animator.SetFloat("DirX", direction.x);
            Animator.SetFloat("DirY", direction.y);
        }

        protected bool IsDirectionAvailable(Vector2 direction) => GetNodeInDirection(direction) != null;

        protected Node GetNodeInDirection(Vector2 direction)
        {
            if ( direction == Vector2.left )
            {
                return CurrentNode.NodeNeighbors.Left;
            }

            if ( direction == Vector2.right )
            {
                return CurrentNode.NodeNeighbors.Right;
            }

            if ( direction == Vector2.up )
            {
                return CurrentNode.NodeNeighbors.Up;
            }

            return CurrentNode.NodeNeighbors.Down;
        }

        protected virtual void GetCurrentNode()
        {
            var newNode = LevelManager.Instance.LevelGrid.NodeFromWorldPostiion(transform.position);
            if ( Equals(newNode, CurrentNode) )
            {
                return;
            }

            if ( StartingNode?.NodeNeighbors == null )
            {
                StartingNode = newNode;
            }

            PreviousNode = CurrentNode;
            CurrentNode = newNode;
        }

        private void SetNextNodeBasedOnDirection()
        {
            NextNode = GetNodeInDirection(CurrentDirection);
        }

        private void MoveActor()
        {
            if ( CurrentNode.IsTeleport )
            {
                var teleportLeft = CurrentNode.IsLeft && CurrentDirection == Vector2.left;
                var teleportRight = !CurrentNode.IsLeft && CurrentDirection == Vector2.right;
                if ( teleportLeft || teleportRight )
                {
                    transform.position = CurrentNode.TwinTeleport.Position;
                }
            }

            var target = NextNode ?? CurrentNode; // if next node is a wall, move closest to the current node

            var movePosition = transform.localPosition;

            movePosition.x = Mathf.MoveTowards(transform.position.x, target.Position.x, CurrentSpeed * Time.deltaTime);
            movePosition.y = Mathf.MoveTowards(transform.position.y, target.Position.y, CurrentSpeed * Time.deltaTime);

            rb2D.MovePosition(movePosition);
        }
    }
}