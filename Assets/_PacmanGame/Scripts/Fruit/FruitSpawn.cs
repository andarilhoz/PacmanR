using UnityEngine;
using _PacmanGame.Scripts.Actors;

namespace _PacmanGame.Scripts.Fruit
{
    public class FruitSpawn : MonoBehaviour
    {
        public GameObject FruitPrefab;

        //will drop fruit when player eat these amount of dots
        private const int FIRST_FRUIT_DOT_COUNT = 70;
        private const int SECOND_FRUIT_DOT_COUNT = 150;

        private readonly int[] pointsTrigger = {FIRST_FRUIT_DOT_COUNT, SECOND_FRUIT_DOT_COUNT};
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