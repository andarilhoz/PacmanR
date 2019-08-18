using System;
using UnityEngine;

namespace _PacmanGame.Scripts.Input
{
    public class TouchInput : MonoBehaviour, IInputControll
    {
        private void Update()
        {
            /* TODO listen for swipe */
        }

        public event Action<Vector2> OnInput;
    }
}