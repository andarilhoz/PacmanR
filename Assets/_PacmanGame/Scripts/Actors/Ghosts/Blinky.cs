using UnityEngine;
using _PacmanGame.Scripts.ExtensionHelpers;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public class Blinky : BaseGhost
    {
        private void Update()
        {
            Debug.DrawLine(transform.position, PacmanTarget.transform.position, Color.green);
            MoveTowardsPacman();
        }

        private void MoveTowardsPacman()
        {
            var distance = (Vector2) (PacmanTarget.transform.position - transform.position);
            var direction = distance.DirectionDecision();
        }
    }
}