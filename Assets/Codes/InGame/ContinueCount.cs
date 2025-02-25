using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueCount : MonoBehaviour
{
    public Text countText;
    private int count;

    void OnEnable()
    {
        count = 10;
        StartCoroutine(CountReduce());
    }

    // 10초 카운트 줄고 줄어들게 하기
    IEnumerator CountReduce()
    {
        while(count > 0)
        {
            countText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        gameObject.SetActive(false);
    }
}
