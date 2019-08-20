using System.Collections.Generic;
using UnityEngine;
using _PacmanGame.Scripts.CustomWalls;
using _PacmanGame.Scripts.Pathfind;

namespace _PacmanGame.Scripts.Actors
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Actors : MonoBehaviour
    {
        public Node previousNode;
        public Node currentNode;
        public Node nextNode;
        
        public float speed;
        protected float currentSpeed;
        public Vector2 currentDirection = Vector2.up;
        public Animator animator;
        
        private Rigidbody2D rb2D;

        protected Vector2 lastDirectionBuffer;
        private float changeDirectionTimeout = 0.3f;
        private float lastDirectionTimeout = 0;

        protected virtual void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            currentNode = LevelManager.LevelGrid.NodeFromWorldPostiion(transform.position);
        }

        protected virtual void Start()
        {
            animator.SetFloat("DirX", currentDirection.x);
            animator.SetFloat("DirY", currentDirection.y);
            currentSpeed = speed;
        }

        public virtual void ChangeDirection(Vector2 direction)
        {
            if ( direction == currentDirection ) return;
            
            lastDirectionBuffer = direction;
            if(lastDirectionTimeout <= 0 )lastDirectionTimeout = changeDirectionTimeout;
            if ( !IsDirectionAvailable(direction) )
            {
                return;
            }

            currentDirection = direction;
            animator.SetFloat("DirX", direction.x);
            animator.SetFloat("DirY", direction.y);
        }
        
        protected bool IsDirectionAvailable(Vector2 direction)
        {
            
            if ( direction == Vector2.left )
            {
                return currentNode.nodeIntersections.Left != null;
            }
            
            if ( direction == Vector2.right )
            {
                return currentNode.nodeIntersections.Right != null;
            }
            
            if ( direction == Vector2.up )
            {
                return currentNode.nodeIntersections.Up != null;
            }
            
            if ( direction == Vector2.down )
            {
                return currentNode.nodeIntersections.Down != null;
            }

            return false;
        }

        protected void FixedUpdate()
        {
            GetCurrentNode();
            TryAgainChangeDirection();
            SetNextNode();
            MoveActor();
        }

        private void GetCurrentNode()
        {
            
            var newNode = LevelManager.LevelGrid.NodeFromWorldPostiion(transform.position);
            if ( Equals(newNode, currentNode) )
            {
                return;
            }
            previousNode = currentNode;
            currentNode = newNode;
        }

        private void SetNextNode()
        {
            if ( currentDirection == Vector2.left )
            {
                nextNode = currentNode.nodeIntersections.Left;
            }
            
            if ( currentDirection == Vector2.right )
            {
                nextNode = currentNode.nodeIntersections.Right;
            }
            
            if ( currentDirection == Vector2.up )
            {
                nextNode = currentNode.nodeIntersections.Up;
            }
            
            if ( currentDirection == Vector2.down )
            {
                nextNode = currentNode.nodeIntersections.Down;
            }
        }

        private void TryAgainChangeDirection()
        {
            if ( lastDirectionTimeout <= 0 )
            {
                lastDirectionTimeout = 0;
                return;
            }
            ChangeDirection(lastDirectionBuffer);
            lastDirectionTimeout -= Time.deltaTime;
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

            Vector3 movePosition = transform.position;
 
            movePosition.x = Mathf.MoveTowards(transform.position.x, target.Position.x , currentSpeed * Time.deltaTime);
            movePosition.y = Mathf.MoveTowards(transform.position.y, target.Position.y , currentSpeed * Time.deltaTime);
 
            rb2D.MovePosition(movePosition);
        }

    }
}
