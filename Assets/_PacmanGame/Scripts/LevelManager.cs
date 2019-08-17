using UnityEngine;

namespace _PacmanGame.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public MapGeneratorJsonDriven MapGenerator;

        public void Initialize()
        {
            MapGenerator.GenerateMap();
        }

        private void Start()
        {
            Initialize();
        }
    }
}