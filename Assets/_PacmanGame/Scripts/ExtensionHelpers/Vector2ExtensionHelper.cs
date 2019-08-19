using UnityEngine;

namespace _PacmanGame.Scripts.ExtensionHelpers
{
    public static class Vector2ExtensionHelper
    {
        public static Vector2 DirectionDecision(this Vector2 vector)
        {
            Vector2 returnVector;
            if ( vector.x > vector.y )
            {
                returnVector = new Vector2(Mathf.Sign(vector.x), 0);
                return returnVector;
            }
            returnVector = new Vector2(0, Mathf.Sign(vector.x));
            return returnVector;
        }
    }
}