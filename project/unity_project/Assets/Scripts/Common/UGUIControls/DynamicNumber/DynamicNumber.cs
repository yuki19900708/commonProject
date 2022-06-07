using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicNumber : MonoBehaviour {

    public Action<float> Event_OnAnimationCompleted;

    public Text numberText;

    public float totalTime;

    private float value;

    private float currentValue;

    [SerializeField]
    private bool isDynamic;

    [SerializeField]
    private EaseType easeType;

    private bool isPlaying = false;

    private float animationTime;

    private float currentNumber;

    private float lastNumber;

    private float timer;

    public float Value
    {
        get { return value; }
        set {
            
            SetValue(value);
        }
    }

    private void SetValue(float value)
    {
        this.value = value;
        if (isDynamic)
        {
            timer = 0;
            animationTime = totalTime;
            lastNumber = currentNumber;
            isPlaying = true;
        }
        else
        {
            numberText.text = value.ToString();
        }
    }


    private void Update()
    {
        if (isPlaying)
        {
            PlayNumberAnimation();
        }
    }

    private void PlayNumberAnimation()
    {
        timer += Time.deltaTime;
        if (timer >= animationTime)
        {
            timer = 0;
            isPlaying = false;
            animationTime = 0;          
            currentNumber = value;
            lastNumber = value;
            numberText.text = value.ToString();
        }
        else
        {
            float result = EaseUtil.EasingMethod(timer, lastNumber, value - lastNumber, animationTime, easeType);
            currentNumber = (int)result;
            numberText.text = ((int)result).ToString();
        }
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Play()
    {
        isPlaying = true;
    }

}
