using UnityEngine;
using _PacmanGame.Scripts.Map;

namespace _PacmanGame.Scripts.Actors
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Actors : MonoBehaviour
    {
        public Node previousNode;
        public Node currentNode;
        public Node nextNode;

        public float baseSpeed;
        protected float currentSpeed;
        public Vector2 currentDirection = Vector2.up;
        public Animator animator;

        private Rigidbody2D rb2D;

        public Node StartingNode;

        protected virtual void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            currentNode = LevelManager.Instance.LevelGrid.NodeFromWorldPostiion(transform.position);
        }

        protected virtual void Start()
        {
            animator.SetFloat("DirX", currentDirection.x);
            animator.SetFloat("DirY", currentDirection.y);
            currentSpeed = baseSpeed;
        }

        public virtual void ResetActor()
        {
            transform.localPosition = StartingNode.Position;
            currentNode = StartingNode;
            currentSpeed = baseSpeed;
        }

        protected virtual void FixedUpdate()
        {
            if ( LevelManager.Instance.CurrentGameState.Equals(LevelManager.GameState.Pause) )
            {
                return;
            }
            GetCurrentNode();
            SetNextNode();
            MoveActor();
        }

        public virtual void ChangeDirection(Vector2 direction)
        {
            if ( direction == currentDirection )
            {
                return;
            }

            currentDirection = direction;
            animator.SetFloat("DirX", direction.x);
            animator.SetFloat("DirY", direction.y);
        }

        protected bool IsDirectionAvailable(Vector2 direction) => GetNodeInDirection(direction) != null;

        protected Node GetNodeInDirection(Vector2 direction)
        {
            if ( direction == Vector2.left )
            {
                return currentNode.NodeIntersections.Left;
            }

            if ( direction == Vector2.right )
            {
                return currentNode.NodeIntersections.Right;
            }

            if ( direction == Vector2.up )
            {
                return currentNode.NodeIntersections.Up;
            }

            return currentNode.NodeIntersections.Down;
        }


        protected virtual void GetCurrentNode()
        {
            var newNode = LevelManager.Instance.LevelGrid.NodeFromWorldPostiion(transform.position);
            if ( Equals(newNode, currentNode) )
            {
                return;
            }

            if ( StartingNode?.NodeIntersections == null )
            {
                StartingNode = newNode;
            }

            previousNode = currentNode;
            currentNode = newNode;
        }

        private void SetNextNode()
        {
            nextNode = GetNodeInDirection(currentDirection);
        }

        private void MoveActor()
        {
            if ( currentNode.IsTeleport )
            {
                var teleportLeft = currentNode.isLeft && currentDirection == Vector2.left;
                var teleportRight = !currentNode.isLeft && currentDirection == Vector2.right;
                if ( teleportLeft || teleportRight )
                {
                    transform.position = currentNode.TwinTeleport.Position;
                }
            }

            var target = nextNode ?? currentNode;

            var movePosition = transform.localPosition;

            movePosition.x = Mathf.MoveTowards(transform.position.x, target.Position.x, currentSpeed * Time.deltaTime);
            movePosition.y = Mathf.MoveTowards(transform.position.y, target.Position.y, currentSpeed * Time.deltaTime);

            rb2D.MovePosition(movePosition);
        }
    }
}