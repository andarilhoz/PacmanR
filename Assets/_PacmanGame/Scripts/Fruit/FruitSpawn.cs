using UnityEngine;
using _PacmanGame.Scripts.Actors;

namespace _PacmanGame.Scripts.Fruit
{
    public class FruitSpawn : MonoBehaviour
    {
        public GameObject FruitPrefab;

        //will drop fruit when player eat these amount of dots
        private readonly int[] pointsTrigger = {70, 150};
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
            Instantiate(FruitPrefab, transform.position, Quaternion.identity);
        }
    }
}