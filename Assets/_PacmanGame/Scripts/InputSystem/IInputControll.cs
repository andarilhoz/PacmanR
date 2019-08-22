using System;
using UnityEngine;

namespace _PacmanGame.Scripts.InputSystem
{
    public interface IInputControll
    {
        event Action<Vector2> OnInput;
    }
}