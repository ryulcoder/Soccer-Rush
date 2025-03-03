using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingImageDisappear : MonoBehaviour
{
    Slider loadingSlider;
    public GameObject LoadingImage;
    float duration = 3f; // 3�� ���� ä���
    float elapsed = 0f;  // ��� �ð�

    void Awake()
    {
        loadingSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(WaitLoading());
    }

    IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(1.2f);
        while(loadingSlider.value < 1)
        {
            elapsed += Time.deltaTime * 1.3f;
            loadingSlider.value = Mathf.Lerp(0, 1, elapsed / duration); 
            yield return null;
        }
        LoadingImage.SetActive(false);
        GameManager.Instance.GameStart();
    }
}
