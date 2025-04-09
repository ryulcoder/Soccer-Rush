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
        count = 5;
        StartCoroutine(StartImpact());

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

    IEnumerator StartImpact()
    {
        yield return new WaitForSeconds(1f);
        TapButton.SetActive(true);
        yield return new WaitForSeconds(1f);
        canEnemy = true;
        StartCoroutine(ReduceCount());
    }

    public void OnButtonClick()
    {
        // ���� �ʰ� �� �۵����� ����
        if (accumulatedIncrease + increaseAmount > maxIncreasePerSecond)
            return;

        slider.value += increaseAmount;
        accumulatedIncrease += increaseAmount;
    }

    IEnumerator ReduceCount()
    {
        countText.gameObject.SetActive(true);
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
            ImpactEnd();
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

    void ImpactEnd()
    {
        if(slider.value > 5f)
        {
            playerWin = true;
        }
        else
        {
            playerWin=false;
        }

        Debug.Log(playerWin);
    }

    private void OnDisable()
    {
        countText.gameObject.SetActive(false);
        slider.value = 5f;
        count = 5;
        button.interactable = false;

    }
}
