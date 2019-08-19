using System.Collections.Generic;
using UnityEngine;
using _PacmanGame.Scripts.CustomWalls;

namespace _PacmanGame.Scripts.Actors
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Actors : MonoBehaviour
    {
        public float speed;
        public Vector2 currentDirection = Vector2.up;
        public Animator animator;
        
        private const float WALL_OFFSET = 0.3f;
        private Rigidbody2D rigidbody2D;

        private Vector2 lastDirectionBuffer;
        private float changeDirectionTimeout = 0.3f;
        private float lastDirectionTimeout = 0;

        protected virtual void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private bool IsDirectionAvailable(Vector2 direction)
        {
            var hit = Physics2D.Raycast(transform.position, direction, 100f, LayerMask.GetMask("Wall"));
            if ( hit.transform == null ) return true;
            var distance = Mathf.Abs((hit.transform.localPosition - transform.localPosition).magnitude);
            return distance > WALL_OFFSET;
        }

        public void ChangeDirection(Vector2 direction)
        {
            lastDirectionBuffer = direction;
            if(lastDirectionTimeout <= 0 )lastDirectionTimeout = changeDirectionTimeout;
            if ( direction == currentDirection ) return;

            if ( !IsDirectionAvailable(direction) )
            {
                return;
            }

            currentDirection = direction;
            animator.SetFloat("DirX", direction.x);
            animator.SetFloat("DirY", direction.y);
        }

        private void FixedUpdate()
        {
            TryAgainChangeDirection();
            MoveActor();
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
            if ( !IsDirectionAvailable(currentDirection) )
            {
                rigidbody2D.velocity = Vector2.zero;
                return;
            }

            rigidbody2D.velocity = currentDirection * speed;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ( !other.CompareTag("Teleport") )
            {
                return;
            }

            var teleport = other.GetComponent<Teleport>();
            var teleportLeft = teleport.isLeft && currentDirection == Vector2.left;
            var teleportRight = !teleport.isLeft && currentDirection == Vector2.right;
            if ( !teleportLeft && !teleportRight )
            {
                return;
            }
            teleport.TeleportActor(this);
        }
    }
}
