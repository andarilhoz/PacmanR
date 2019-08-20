using System;
using UnityEngine;
using _PacmanGame.Scripts.Input;
using _PacmanGame.Scripts.Pathfind;
using _PacmanGame.Scripts.Score;

namespace _PacmanGame.Scripts.Actors
{
    public class Pacman : Actors
    {
        public bool isAlive;
        public KeyboardInput InputControll;
        
        public static event Action EatPowerDot;
        public static event Action<int> AddScore;

        protected override void Awake()
        {
            base.Awake();
            InputControll.OnInput += ChangeDirection;
        }

        protected override void Start()
        {
            base.Start();
            ChangeDirection(currentDirection);
        }

        private void OnDrawGizmos()
        {
            if ( currentNode != null )
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(currentNode.Position, Vector2.one * .255f);
            }
        }

        public override void ChangeDirection(Vector2 direction)
        {
            base.ChangeDirection(direction);
            
            var rotate = currentDirection == Vector2.left ? 180 :
                currentDirection == Vector2.down ? 270 :
                currentDirection == Vector2.up ? 90 : 0; 
            transform.localRotation = Quaternion.Euler(0,0,rotate);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ( !other.transform.CompareTag("Dot") && !other.transform.CompareTag("PowerDot") )
            {
                return;
            }

            if ( other.transform.CompareTag("PowerDot") )
            {
                EatPowerDot?.Invoke();
                Destroy(other.gameObject);
                AddScore?.Invoke(50);
                return;
            }

            
            Destroy(other.gameObject);
            AddScore?.Invoke(1);
        }
    }
}