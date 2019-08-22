using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _PacmanGame.Scripts.InputSystem;

public class InstructionsFadeout : MonoBehaviour
{
    public TextMeshProUGUI description;
    public TextMeshProUGUI Timer;
    public Image fadeImage;
    
    public static event Action StartGame; 

    private const string KEYBOARD_INPUT_DESCRIPTION = "Pressione uma seta para inciar";
    private const string TOUCH_INPUT_DESCRIPTION = "Deslize para um lado para inciar";
    
    private const float TIMER_PAUSE = 3f;
    private float timerCountDown;
    
    private bool fading = false;

    private bool timerEnd = false;
    // Start is called before the first frame update
    private void Start()
    {
        if ( Application.platform.Equals(RuntimePlatform.Android) )
        {
            description.text = TOUCH_INPUT_DESCRIPTION;
            return;
        }
        description.text = KEYBOARD_INPUT_DESCRIPTION;

        timerCountDown = TIMER_PAUSE;

        KeyboardInput.Instance.OnInput += InitializeFadeIn;
        TouchInput.Instance.OnInput += InitializeFadeIn;
    }

    private void Update()
    {
        FadeIn();
        CountDown();
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
        
        description.gameObject.SetActive(false);
        Timer.gameObject.SetActive(true);
        
        timerCountDown = TIMER_PAUSE;
    }

    private void FadeIn()
    {

        if( fadeImage.color.a <= 0 ||  !fading )
        {
            return;
        }
        
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - Time.deltaTime);
        description.color = new Color(description.color.r, description.color.g, description.color.b, description.color.a - Time.deltaTime); 
    }

}
