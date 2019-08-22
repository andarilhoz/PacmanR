using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _PacmanGame.Scripts.InputSystem;

namespace _PacmanGame.Scripts.Canvas
{
    public class InstructionsFadeout : MonoBehaviour
    {
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Timer;
        public Image FadeImage;

        public static event Action StartGame;
        public static event Action StartIntro;

        private const string KEYBOARD_INPUT_DESCRIPTION = "Pressione uma seta para inciar";
        private const string TOUCH_INPUT_DESCRIPTION = "Deslize para um lado para inciar";

        private const float TIMER_PAUSE = 5f;
        private float timerCountDown;

        private bool fading = false;

        private bool timerEnd = false;

        
        private void Start()
        {
            if ( Application.platform.Equals(RuntimePlatform.Android) )
            {
                Description.text = TOUCH_INPUT_DESCRIPTION;
            }
            else
            {
                Description.text = KEYBOARD_INPUT_DESCRIPTION;
            }

            timerCountDown = TIMER_PAUSE;

            KeyboardInput.Instance.OnInput += InitializeFadeIn;
            TouchInput.Instance.OnInput += InitializeFadeIn;
        }

        private void Update()
        {
            //if player inputed an direction, will fade the black screen out
            FadeOut();
            //will do an countdown to start the game.
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
            Description.gameObject.SetActive(false);
            Timer.gameObject.SetActive(true);

            timerCountDown = TIMER_PAUSE;
        }

        private void FadeOut()
        {
            if ( FadeImage.color.a <= 0 || !fading )
            {
                return;
            }

            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b,
                FadeImage.color.a - Time.deltaTime);
            Description.color = new Color(Description.color.r, Description.color.g, Description.color.b,
                Description.color.a - Time.deltaTime);
        }
    }
}