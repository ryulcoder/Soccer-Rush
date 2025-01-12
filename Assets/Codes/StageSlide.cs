using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSlide : MonoBehaviour
{
    public RectTransform[] panels; // �̵��� �гε�
    public float slideDuration = 0.5f; // �����̵� �ִϸ��̼� �ð�

    public int currentStage; // ȭ���� ���۵� �� �߾ӿ� ǥ�õ� ��������

    // ȭ�� �������� ����
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

    // ���� ��ư
    public void SlideLeft()
    {
        if (currentStage > 0)
        {
            // ���� �г� �̵�
            panels[currentStage].DOAnchorPosX(1920, slideDuration); // ȭ�� ���������� �̵�
            currentStage--;
            // ���� �г� �߾����� �̵�
            panels[currentStage].DOAnchorPosX(0, slideDuration);
        }
    }
    
    // ���� ��ư
    public void SlideRight()
    {
        if (currentStage < panels.Length - 1)
        {
            // ���� �г� �̵�
            panels[currentStage].DOAnchorPosX(-1920, slideDuration); // ȭ�� �������� �̵�
            currentStage++;
            // ������ �г� �߾����� �̵�
            panels[currentStage].DOAnchorPosX(0, slideDuration);
        }
    }
}
