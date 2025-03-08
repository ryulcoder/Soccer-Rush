using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTypingEffect : MonoBehaviour
{
    public Text TypingText;
    public float typingDelay;
    public bool typingEnd;

    [Space]
    public Button NextButton;
    public Button EndButton;

    [Space][TextArea]
    public string[] content;

    // content�� ���� �ε����� ������
    public GameObject[] OtherActiveObjects;

    bool typingOn, nextOn, endOn;
    int idx;
    

    void Update()
    {
        if (!typingOn && gameObject.activeSelf && !typingEnd)
        {
            StartCoroutine(TypingAni());
        }
    }

    IEnumerator TypingAni()
    {
        typingOn = true;

        // ���뿡 �ƹ��͵� ������
        if (content.Length <= 0)
        {
            typingEnd = true;

            gameObject.SetActive(false);

            yield break;
        }

        NextButton.onClick.RemoveAllListeners();
        NextButton.onClick.AddListener(Next);
        NextButton.gameObject.SetActive(true);

        // ���� Typing ���� ǥ�ÿ� ���� Ȱ��ȭ �� ������Ʈ Ȱ��ȭ
        if (OtherActiveObjects.Length != 0 && OtherActiveObjects[idx] != null && OtherActiveObjects.Length > idx)
            OtherActiveObjects[idx].SetActive(true);

        // Typing ���� �ִ� ����
        StartCoroutine(TypingEffect(content[idx]));

        yield break;
    }

    IEnumerator TypingEffect(string text)
    {
        TypingText.text = null;

        for (int i = 0; i < text.Length; i++)
        {
            TypingText.text += text[i];
            yield return new WaitForSecondsRealtime(typingDelay);
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // ������ content�Ͻ�
        if (idx + 1 >= content.Length)
        {
            StartCoroutine(WaitEnd());

            yield break;
        }
        else
        {
            StartCoroutine(WaitNext());
        }
    }

    // ���� Dotween ��Ȱ��ȭ �Լ� ���
    IEnumerator WaitEnd()
    {
        typingEnd = true;

        NextButton.gameObject.SetActive(false);

        EndButton.onClick.AddListener(End);

        while (!endOn)
        {
            yield return new WaitForSecondsRealtime(0.2f);
        }

        gameObject.SetActive(false);
        

        yield break;
    }

    // ���� content next�Լ� ���
    IEnumerator WaitNext()
    {
        while (!nextOn)
        {
            yield return new WaitForSecondsRealtime(0.2f);
        }
        
        if (OtherActiveObjects.Length != 0 && OtherActiveObjects[idx] != null && OtherActiveObjects.Length > idx)
            OtherActiveObjects[idx].SetActive(false);

        nextOn = false;
        idx++;

        typingOn = false;
        yield break;
    }
    
    public void End()
    {
        endOn = true;
    }

    public void Next()
    {
        if (nextOn) return;

        nextOn = true;
    }

    
}
