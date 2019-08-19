using UnityEngine;

namespace _PacmanGame.Scripts.Actors.Ghosts
{
    public abstract class BaseGhost : Actors
    {
        public GhostState CurrentState;

        protected static Pacman PacmanTarget;

        protected override void Awake()
        {
            base.Awake();
            
            if ( PacmanTarget != null ) return;
            PacmanTarget = FindObjectOfType<Pacman>();
        }
    }
}