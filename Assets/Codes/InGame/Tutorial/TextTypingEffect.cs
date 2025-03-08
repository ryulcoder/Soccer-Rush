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

    // content와 같은 인덱스에 오도록
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

        // 내용에 아무것도 없을시
        if (content.Length <= 0)
        {
            typingEnd = true;

            gameObject.SetActive(false);

            yield break;
        }

        NextButton.onClick.RemoveAllListeners();
        NextButton.onClick.AddListener(Next);
        NextButton.gameObject.SetActive(true);

        // 현재 Typing 내용 표시에 같이 활성화 할 오브젝트 활성화
        if (OtherActiveObjects.Length != 0 && OtherActiveObjects[idx] != null && OtherActiveObjects.Length > idx)
            OtherActiveObjects[idx].SetActive(true);

        // Typing 글자 애니 실행
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

        // 마지막 content일시
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

    // 현재 Dotween 비활성화 함수 대기
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

    // 다음 content next함수 대기
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
