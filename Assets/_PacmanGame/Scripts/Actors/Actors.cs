using System.Collections.Generic;
using UnityEngine;

namespace _PacmanGame.Scripts.Actors
{
    public abstract class Actors : MonoBehaviour
    {
        public float speed;
        public Vector2 currentDirection = Vector2.up;
        public Animator animator;
        
        private const float PACMAN_WALL_OFFSET = 0.3f;
        public Rigidbody2D rigidbody2D;
        private Collider2D collider;

        private Vector2 lastDirectionBuffer;
        private float changeDirectionTimeout = 0.3f;
        private float lastDirectionTimeout = 0;

        private bool IsDirectionAvailable(Vector2 direction)
        {
            var hit = Physics2D.Raycast(transform.position, direction, 100f, LayerMask.GetMask("Wall"));
            if ( hit.transform == null ) return true;
            var distance = Mathf.Abs((hit.transform.localPosition - transform.localPosition).magnitude);
            return distance > PACMAN_WALL_OFFSET;
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
        }
    }
}
