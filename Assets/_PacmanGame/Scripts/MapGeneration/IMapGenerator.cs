using System;
using UnityEngine;

namespace _PacmanGame.Scripts
{
    public interface IMapGenerator
    {
        MapGeneratorJsonDriven.MapDetails GenerateMap();
    }
}
