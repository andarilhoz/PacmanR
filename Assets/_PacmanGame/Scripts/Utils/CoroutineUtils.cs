using System;
using System.Collections;
using UnityEngine;

namespace _PacmanGame.Scripts.Utils
{
    public class CoroutineUtils
    {
        //used on Webgl build
        public static IEnumerator WaitSecondsCoroutine(float timeInSeconds, Action callback)
        {
            yield return new WaitForSeconds(timeInSeconds);
            callback();
        }
    }
}