using System;

namespace _PacmanGame.Scripts
{
    public static class ItemTypesEnum
    {

        public static bool IsValidPath(this ItemTypes type)
        {
            switch (type)
            {
                case ItemTypes.Wall:
                    return false;
                default:
                    return true;
            }
        }
        
        public static bool IsGhost(this ItemTypes type)
        {
            switch (type)
            {
                case ItemTypes.Blinky:
                    return true;
                case ItemTypes.Pinky:
                    return true;
                case ItemTypes.Inky:
                    return true;
                default:
                    return false;
            }
        }
        
    }
    public enum ItemTypes
    {
        Empty = 0,
        Wall = 1,
        Point = 2,
        PlayerPos = 3,
        Teleport = 4,
        Blinky = 5,
        Pinky = 7,
        Inky = 8,
        PowerDot = 6,
        ThinWall = 9
    }
}