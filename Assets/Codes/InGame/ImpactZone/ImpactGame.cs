using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImpactGame : MonoBehaviour
{
    public Slider slider;
    public float increaseAmount = 0.1f; // ��ư �ѹ��� ������ ��ġ
    float maxIncreasePerSecond; // �ʴ� �ִ� ������
    public float decreaseRate = 0.5f; // �ʴ� ���ҷ�

    private float accumulatedIncrease = 0f;
    private float timer = 0f;

    public bool playerWin;
    bool canEnemy;          // �� ���� ����

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
        // ���ʸ��� ���� ������ �ʱ�ȭ
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
        // ���� �ʰ� �� �۵����� ����
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

            // ī��Ʈ�� 3 ������ ��� �ؽ�Ʈ ���� ���������� ����
            if (count <= 3)
            {
                countText.color = Color.red;
            }
            else
            {
                countText.color = Color.white; // �⺻ �� (���ϴ� ������ ���� ����)
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
                slider.value = Mathf.Max(slider.value, 0f); // ���� ����
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
