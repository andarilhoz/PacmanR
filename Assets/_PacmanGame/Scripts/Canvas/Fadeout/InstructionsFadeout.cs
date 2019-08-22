﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _PacmanGame.Scripts.InputSystem;

namespace _PacmanGame.Scripts.Canvas.Fadeout
{
    public class InstructionsFadeout : MonoBehaviour
    {
        public TextMeshProUGUI description;
        public TextMeshProUGUI Timer;
        public Image fadeImage;

        public static event Action StartGame;
        public static event Action StartIntro;

        private const string KEYBOARD_INPUT_DESCRIPTION = "Pressione uma seta para inciar";
        private const string TOUCH_INPUT_DESCRIPTION = "Deslize para um lado para inciar";

        private const float TIMER_PAUSE = 5f;
        private float timerCountDown;

        private bool fading = false;

        private bool timerEnd = false;

        // Start is called before the first frame update
        private void Start()
        {
            if ( Application.platform.Equals(RuntimePlatform.Android) )
            {
                description.text = TOUCH_INPUT_DESCRIPTION;
            }
            else
            {
                description.text = KEYBOARD_INPUT_DESCRIPTION;
            }

            timerCountDown = TIMER_PAUSE;

            KeyboardInput.Instance.OnInput += InitializeFadeIn;
            TouchInput.Instance.OnInput += InitializeFadeIn;
        }

        private void Update()
        {
            FadeIn();
            CountDown();
        }

        private void OnDestroy()
        {
            StartGame = () => { };
            StartIntro = () => { };
        }

        private void CountDown()
        {
            if ( !fading )
            {
                return;
            }

            if ( timerCountDown >= 0 )
            {
                timerCountDown -= Time.deltaTime;
                Timer.text = Mathf.Round(timerCountDown).ToString();
                return;
            }

            if ( timerEnd )
            {
                return;
            }

            timerEnd = true;
            Timer.gameObject.SetActive(false);
            StartGame?.Invoke();
        }

        private void InitializeFadeIn(Vector2 direction)
        {
            if ( fading )
            {
                return;
            }

            fading = true;
            StartIntro?.Invoke();
            description.gameObject.SetActive(false);
            Timer.gameObject.SetActive(true);

            timerCountDown = TIMER_PAUSE;
        }

        private void FadeIn()
        {
            if ( fadeImage.color.a <= 0 || !fading )
            {
                return;
            }

            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b,
                fadeImage.color.a - Time.deltaTime);
            description.color = new Color(description.color.r, description.color.g, description.color.b,
                description.color.a - Time.deltaTime);
        }
    }
}