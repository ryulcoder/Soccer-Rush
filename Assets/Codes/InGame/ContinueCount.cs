using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueCount : MonoBehaviour
{
    public Text countText;
    private int count;

    public GameObject freeImage;

    void OnEnable()
    {
        if (!PlayerPrefs.HasKey("FreeRevive"))
        {
            freeImage.SetActive(true);
        }
        count = 8;
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

    public void Revive()
    {
        if (!PlayerPrefs.HasKey("FreeRevive"))
        {
            PlayerPrefs.SetInt("FreeRevive", 1);
            GameManager.Instance.PlayerRevive();
        }
        else
        {
            GameManager.Instance.ReviveAd();
        }
    }
}
