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

    // 10�� ī��Ʈ �ٰ� �پ��� �ϱ�
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
