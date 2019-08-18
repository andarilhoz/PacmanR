using System;
using UnityEngine;

namespace _PacmanGame.Scripts.Input
{
    public interface IInputControll
    {
        event Action<Vector2> OnInput;
    }
}