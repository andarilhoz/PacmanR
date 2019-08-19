using System.Collections.Generic;
using UnityEngine;

namespace _PacmanGame.Scripts.CustomWalls
{
    public class Teleport : MonoBehaviour
    {
        private static List<GameObject> teleports = new List<GameObject>();
        public GameObject twin;
        public bool isLeft;

        private void Awake()
        {
            teleports.Add(gameObject);
        }

        private void Start()
        {
            twin = teleports.Find(g => g != gameObject);
            isLeft = transform.position.x < twin.transform.position.x;
        }

        public void TeleportActor(Actors.Actors actor)
        {            
            actor.transform.position = twin.transform.position;
        }
    }
}