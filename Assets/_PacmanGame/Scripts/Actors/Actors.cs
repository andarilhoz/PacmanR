using System.Collections.Generic;
using UnityEngine;

namespace _PacmanGame.Scripts.Actors
{
    public abstract class Actors : MonoBehaviour
    {
        public Vector2 position;
        public float speed;
        public Vector2 facingDirection;
        public Vector2 currentDirection;

        private Vector2[] directions = new[] {Vector2.down, Vector2.left, Vector2.right, Vector2.up};

        private Dictionary<Vector2, bool> AvailableDirections()
        {
            return new Dictionary<Vector2, bool>()
            {
                {Vector2.down, IsDirectionAvailable(Vector2.down)},
                {Vector2.left, IsDirectionAvailable(Vector2.left)},
                {Vector2.right, IsDirectionAvailable(Vector2.right)},
                {Vector2.up, IsDirectionAvailable(Vector2.up)}
            };
        }

        private bool IsDirectionAvailable(Vector2 direction)
        {
            var hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Wall"));
            var distance = Mathf.Abs(hit.point.y - transform.position.y);
            Debug.Log($"direction: {direction} distancia: {distance}");
            return distance > 0;
        }

        public void ChangeDirection(Vector2 direction)
        {
            var availableDirections = AvailableDirections();
            if ( availableDirections[direction] )
            {
                currentDirection = direction;
            }
        }

        private void Awake()
        {
            ChangeDirection(Vector2.left);
        }
    }
}
