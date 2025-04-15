using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipInput : MonoBehaviour
{
    public static SwipInput instance;

    public Swip SwipInputActions;

    public Player Player;

    public Vector2 startTouchPosition, endTouchPosition;
    public Vector2 delta;

    private bool swipeDetected = false;

    public float swipeThreshold = 50f; // 최소 스와이프 거리

    public bool isImpact;

    private void Awake()
    {
        instance = this;

        SwipInputActions = new Swip();
    }
    private void OnEnable()
    {
        SwipInputActions.InputAction.PointerPress.started += OnTouchStarted;
        SwipInputActions.InputAction.PointerPress.canceled += OnTouchEnded;
        SwipInputActions.InputAction.Enable();
    }

    private void OnDisable()
    {
        SwipInputActions.InputAction.PointerPress.started -= OnTouchStarted;
        SwipInputActions.InputAction.PointerPress.canceled -= OnTouchEnded;
        SwipInputActions.InputAction.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        if (isImpact || Time.timeScale == 0)
        {
            startTouchPosition = Vector2.zero;
            endTouchPosition = Vector2.zero;

            return;
        }

        startTouchPosition = SwipInputActions.InputAction.PointerPosition.ReadValue<Vector2>();
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        if (isImpact || Time.timeScale == 0)
        {
            startTouchPosition = Vector2.zero;
            endTouchPosition = Vector2.zero;

            return;
        }

        endTouchPosition = SwipInputActions.InputAction.PointerPosition.ReadValue<Vector2>();

        DetectSwipe();
    }


    
        /*private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (isImpact) return;

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
        }*/

        private void DetectSwipe()
        {
            delta = endTouchPosition - startTouchPosition;

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

        public void Reset()
        {
            isImpact= false;
            swipeDetected = false;

            startTouchPosition = Vector2.zero;
            endTouchPosition = Vector2.zero;
        }

    /*private void OnSwipeLeft() => Debug.Log("Swipe Left!");
    private void OnSwipeRight() => Debug.Log("Swipe Right!");
    private void OnSwipeUp() => Debug.Log("Swipe Up!");
    private void OnSwipeDown() => Debug.Log("Swipe Down!");*/
}
