using System;
using UnityEngine;

namespace _PacmanGame.Scripts.Input
{
    public class KeyboardInput: MonoBehaviour, IInputControll
    {
        public event Action<Vector2> OnInput;
        
        private void Update()
        {
            ListenInput();
        }

        public void ListenInput()
        {
            var rightMove = UnityEngine.Input.GetKeyDown(KeyCode.RightArrow);
            var leftMove = UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow);
            var downMove = UnityEngine.Input.GetKeyDown(KeyCode.DownArrow);
            var upMove = UnityEngine.Input.GetKeyDown(KeyCode.UpArrow);
            
            if ( !rightMove && !leftMove && !downMove && !upMove )
            {
                return;
            }

            var direction = rightMove ? Vector2.right :
                leftMove ? Vector2.left :
                downMove ? Vector2.down : Vector2.up;

            OnInput?.Invoke(direction);
        }
    }
}