using System;
using UnityEngine;

namespace _PacmanGame.Scripts.InputSystem
{
    public class TouchInput : MonoBehaviour, IInputControll
    {
        public event Action<Vector2> OnInput;

        private const float MAX_SWIPE_TIME = 0.5f;
        private const float MIN_SWIPE_DISTANCE = 0.10f;

        private Vector2 startPos;
        private float startTime;
        
        #region Singleton

        private static TouchInput instance;

        public static TouchInput Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = GameObject.FindObjectOfType<TouchInput>();
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

        public void Update()
        {
            if ( Input.touches.Length <= 0 )
            {
                return;
            }

            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = new Vector2(touch.position.x / (float) Screen.width,
                        touch.position.y / (float) Screen.width);
                    startTime = Time.time;
                    break;
                case TouchPhase.Ended when Time.time - startTime > MAX_SWIPE_TIME:
                    return;
                case TouchPhase.Ended:
                    CalculateMovement(touch);
                    break;
            }
        }

        private void CalculateMovement(Touch touch)
        {
            var endPos = new Vector2(touch.position.x / (float) Screen.width, touch.position.y / (float) Screen.width);

            var swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

            if ( swipe.magnitude < MIN_SWIPE_DISTANCE )
            {
                return;
            }

            var horizontalSwipe = Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y);

            if ( horizontalSwipe )
            {
                if ( swipe.x > 0 )
                {
                    OnInput?.Invoke(Vector2.right);
                    return;
                }

                OnInput?.Invoke(Vector2.left);
                return;
            }

            if ( swipe.y > 0 )
            {
                OnInput?.Invoke(Vector2.up);
                return;
            }

            OnInput?.Invoke(Vector2.down);
        }
    }
}