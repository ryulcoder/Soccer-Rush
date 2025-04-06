using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TouchInput : MonoBehaviour
{
    private bool touchDetected = false;

    public Slider TouchSlider;

    private void Update()
    {
        if (Touchscreen.current == null) return;

        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (!touchDetected)
            {
                touchDetected = true;
            }
        }
        else if (touchDetected)
        {
            DetectTouch();
            touchDetected = false;
        }
    }

    private void DetectTouch()
    {
        TouchSlider.value += 5;
    }
}
