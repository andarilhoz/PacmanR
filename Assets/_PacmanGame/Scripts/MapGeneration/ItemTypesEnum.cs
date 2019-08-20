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
                case ItemTypes.ThinWall:
                    return false;
                default:
                    return true;
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
        PowerDot = 6,
        ThinWall = 9
    }
}