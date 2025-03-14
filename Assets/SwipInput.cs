using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipInput : MonoBehaviour
{
    public Player Player;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool swipeDetected = false;

    public float swipeThreshold = 50f; // 최소 스와이프 거리

    private void Update()
    {
        if (Touchscreen.current == null) return;

        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (!swipeDetected)
            {
                startTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                swipeDetected = true;
            }
        }
        else if (swipeDetected)
        {
            endTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            DetectSwipe();
            swipeDetected = false;
        }
    }

    private void DetectSwipe()
    {
        Vector2 delta = endTouchPosition - startTouchPosition;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) // 좌우 스와이프
        {
            if (delta.x > swipeThreshold)
            {
                Player.MoveLeftRight(1);
                Debug.Log("오른쪽 슬라이드");
            }
            else if (delta.x < -swipeThreshold)
            {
                Player.MoveLeftRight(-1);
                Debug.Log("왼쪽 슬라이드");
            }
        }
        else // 상하 스와이프
        {
            if (delta.y > swipeThreshold)
            {
                Player.Jump();
                Debug.Log("위쪽 슬라이드");
            }
            else if (delta.y < -swipeThreshold)
            {
                Player.Spin();
                Debug.Log("아래쪽 슬라이드");
            }

        }
    }

    private void OnSwipeLeft() => Debug.Log("Swipe Left!");
    private void OnSwipeRight() => Debug.Log("Swipe Right!");
    private void OnSwipeUp() => Debug.Log("Swipe Up!");
    private void OnSwipeDown() => Debug.Log("Swipe Down!");
}
