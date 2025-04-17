using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipAni : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject ResumeCount;

    public enum SwipDirection
    {
        Left, Right, Up, Down
    }

    public SwipDirection swipIdx;

    [Space]
    public float swipSpeed = 0.03f;
    public float swipRange = 400;

    [Space]
    public Image Swip;
    RectTransform swipRect;

    public Sprite Click_Sprite;
    public Sprite NotClick_Sprite;

    Vector2[] swipDirVecs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    Color swipColorAZero;
    Vector2 maxVec;
    float maxFloat, swipFloat;
    bool swipOn, coroutineOn;
    

    void Start()
    {
        if (Swip == null)
            Swip = gameObject.GetComponent<Image>();

        swipRect = Swip.GetComponent<RectTransform>();

        Swip.sprite = NotClick_Sprite;

        maxVec = swipDirVecs[(int)swipIdx] * swipRange / 2;
        maxFloat = maxVec.x + maxVec.y;

        swipRect.anchoredPosition = -maxVec;

        swipColorAZero = new Color(Swip.color.r, Swip.color.g, Swip.color.b, 0);

        coroutineOn = true;
    }

    void Update()
    {
        if (coroutineOn)
        {
            StartCoroutine(ClickWait());
        }

        if (swipOn && !(PausePanel.activeSelf || ResumeCount.activeSelf))
        {
            SwipMove();
        }
    }

    IEnumerator ClickWait()
    {
        coroutineOn = false;

        swipRect.anchoredPosition = swipDirVecs[(int)swipIdx] * -swipRange / 2;

        Swip.sprite = NotClick_Sprite;
        Swip.color = Color.white;

        yield return new WaitForSecondsRealtime(0.2f);

        Swip.sprite = Click_Sprite;

        yield return new WaitForSecondsRealtime(0.2f);

        swipOn = true;
    }

    void SwipMove()
    {
        Vector2 swipPos = swipRect.anchoredPosition;
        swipFloat = swipPos.x + swipPos.y;

        if ((maxFloat > 0 && swipFloat >= maxFloat * 0.95f) || (maxFloat < 0 && swipFloat <= maxFloat * 0.95f))
        {
            coroutineOn = true;
            swipOn = false;
            return; 
        }

        Vector2 lerpVec = Vector2.Lerp(swipPos, maxVec, Time.unscaledDeltaTime * swipSpeed);

        swipRect.anchoredPosition = lerpVec;

        Swip.color = Color.Lerp(Swip.color, swipColorAZero, Time.unscaledDeltaTime * swipSpeed);
    }
}
