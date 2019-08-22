using UnityEngine;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Score;

namespace _PacmanGame.Scripts.Fruit
{
    public class FruitSpawn : MonoBehaviour
    {
        public GameObject fruitPrefab;

        private int[] pointsTrigger = {1, 150};
        private int fruitsDisplaced = 0;

        private int dotCounter = 0;

        private void Start()
        {
            Pacman.EatDot += () => CheckIfShouldDropFruit(++dotCounter);
        }

        private void CheckIfShouldDropFruit(int point)
        {
            if ( fruitsDisplaced >= pointsTrigger.Length )
            {
                return;
            }

            var targetPoitns = pointsTrigger[fruitsDisplaced];
            if ( point >= targetPoitns )
            {
                DisplaceFruit();
            }
        }

        private void DisplaceFruit()
        {
            fruitsDisplaced++;
            Instantiate(fruitPrefab, transform.position, Quaternion.identity);
        }
    }
}