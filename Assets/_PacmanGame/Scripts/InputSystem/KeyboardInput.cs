using System;
using UnityEngine;

namespace _PacmanGame.Scripts.InputSystem
{
    public class KeyboardInput : MonoBehaviour, IInputControll
    {
        public event Action<Vector2> OnInput;

        #region Singleton

        private static KeyboardInput instance;

        public static KeyboardInput Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = GameObject.FindObjectOfType<KeyboardInput>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            if ( instance == null )
            {
                instance = this;
            }
        }

        #endregion

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

            Vector2 direction;

            if ( rightMove )
            {
                direction = Vector2.right;
            }
            else if ( leftMove )
            {
                direction = Vector2.left;
            }
            else if ( downMove )
            {
                direction = Vector2.down;
            }
            else
            {
                direction = Vector2.up;
            }

            OnInput?.Invoke(direction);
        }
    }
}