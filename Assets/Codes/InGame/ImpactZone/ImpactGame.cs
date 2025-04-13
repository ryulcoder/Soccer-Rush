using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImpactGame : MonoBehaviour
{
    public Slider slider;
    public float increaseAmount = 0.1f; // 버튼 한번에 증가할 수치
    float maxIncreasePerSecond; // 초당 최대 증가량
    public float decreaseRate = 0.5f; // 초당 감소량

    private float accumulatedIncrease = 0f;
    private float timer = 0f;

    public bool playerWin;
    bool canEnemy;          // 적 공격 가능

    public Text countText;
    int count = 5;
    public GameObject TapButton;
    Button button;

    private void Awake()
    {
        maxIncreasePerSecond = 10 * increaseAmount;
        button = TapButton.GetComponent<Button>();
    }

    private void OnEnable()
    {
        slider.value = 5f;
        count = 5;
        button.interactable = false;
        canEnemy = false;
        slider.gameObject.SetActive(false);
        countText.gameObject.SetActive(false);
        TapButton.SetActive(false);
    }

    void Update()
    {
        // 매초마다 누적 증가량 초기화
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            accumulatedIncrease = 0f;
            timer = 0f;
        }
        EnemyImpact();
    }

    public void OnButtonClick()
    {
        // 제한 초과 시 작동하지 않음
        if (accumulatedIncrease + increaseAmount > maxIncreasePerSecond)
            return;

        slider.value += increaseAmount;
        accumulatedIncrease += increaseAmount;
    }

    public void StartGame()
    {
        slider.gameObject.SetActive(true);
        countText.gameObject.SetActive(true);
        TapButton.SetActive(true);
        StartCoroutine(ReadyImpact());
    }

    IEnumerator ReadyImpact()
    {
        countText.text = "Ready";
        countText.color = Color.white;
        yield return new WaitForSeconds(1f);
        countText.text = "Start";
        yield return new WaitForSeconds(1f);
        canEnemy = true;
        StartCoroutine(ReduceCount());
    }


    IEnumerator ReduceCount()
    {
        button.interactable = true;
        while(count > 0)
        {
            countText.text = count.ToString();

            // 카운트가 3 이하일 경우 텍스트 색을 빨간색으로 변경
            if (count <= 3)
            {
                countText.color = Color.red;
            }
            else
            {
                countText.color = Color.white; // 기본 색 (원하는 색으로 변경 가능)
            }

            yield return new WaitForSeconds(1f);
            count--;
        }

        if(count <= 0)
        {
            canEnemy = false;
            button.interactable = false;
            StartCoroutine(ImpactEnd());
        }
    }

    void EnemyImpact()
    {
        if (canEnemy)
        {
            if (slider.value > 0f)
            {
                slider.value -= decreaseRate * Time.deltaTime;
                slider.value = Mathf.Max(slider.value, 0f); // 음수 방지
            }
        }
    }

    IEnumerator ImpactEnd()
    {
        yield return new WaitForSeconds(1f);

        if(slider.value > 5f)
        {
            playerWin = true;
        }
        else
        {
            playerWin=false;
        }

        gameObject.SetActive(false);
        Debug.Log(playerWin);
    }
}
