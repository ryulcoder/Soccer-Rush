using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSlide : MonoBehaviour
{
    public RectTransform[] panels; // 이동할 패널들
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 시간

    public int currentStage; // 화면이 시작된 후 중앙에 표시될 스테이지

    // 화면 스테이지 세팅
    public void StageSet()
    {
        if (currentStage >= panels.Length)
            currentStage = panels.Length - 1;
        panels[currentStage].anchoredPosition = Vector3.zero;
        for (int i = 0; i < panels.Length; i++)
        {
            if (i < currentStage)
            {
                panels[i].anchoredPosition = new Vector2(-1920, 0);
            }
            else if (i > currentStage)
            {
                panels[i].anchoredPosition = new Vector2(1920, 0);
            }
        }
    }

    // 좌측 버튼
    public void SlideLeft()
    {
        if (currentStage > 0)
        {
            // 현재 패널 이동
            panels[currentStage].DOAnchorPosX(1920, slideDuration); // 화면 오른쪽으로 이동
            currentStage--;
            // 왼쪽 패널 중앙으로 이동
            panels[currentStage].DOAnchorPosX(0, slideDuration);
        }
    }
    
    // 우측 버튼
    public void SlideRight()
    {
        if (currentStage < panels.Length - 1)
        {
            // 현재 패널 이동
            panels[currentStage].DOAnchorPosX(-1920, slideDuration); // 화면 왼쪽으로 이동
            currentStage++;
            // 오른쪽 패널 중앙으로 이동
            panels[currentStage].DOAnchorPosX(0, slideDuration);
        }
    }
}
