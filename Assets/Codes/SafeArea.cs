using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform rectTransform; //safearea ��ũ��Ʈ �ٴ� ������Ʈ �־��ֱ�
    Rect safeArea;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    public RectTransform TopBar;
    public RectTransform BottomBar;

    string deviceModel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        deviceModel = SystemInfo.deviceModel;
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        //safeArea�� �޾Ƽ� min ��Ŀ�� max ��Ŀ�� Position �ο�
        //�ȼ��� ��ȯ�Ǵ� ��Ŀ�� �ֱ� ���ؼ��� ������ ��ȯ �ʿ�
        safeArea = Screen.safeArea;
        minAnchor = safeArea.position;
        maxAnchor = minAnchor + safeArea.size;

        //�ν����� ������Ƽ�� ������� �� �ְ� ������ ��ȯ �� �Ҵ�
        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;

        // �����е�, ������ ���� ���
        if (TopBar && maxAnchor.y == 1)
        {
            TopBar.anchoredPosition = new Vector2(TopBar.anchoredPosition.x, 30);

        }

        if (BottomBar && minAnchor.y != 0)
        {
            BottomBar.anchoredPosition = new Vector2(BottomBar.anchoredPosition.x, -15);

        }




    }

}