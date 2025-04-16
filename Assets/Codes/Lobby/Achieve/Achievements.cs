using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AchData;

public class Achievements : MonoBehaviour
{
    public AchData achData;

    bool get;

    public int amount;

    public Image image;

    Slider slider;
    TextMeshProUGUI sliderText;
    public TextMeshProUGUI effectText;

    public GameObject GetButton;
    Button GetButtonUi;

    public GameObject completePanel;
    public GameObject completeSign;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        sliderText = slider.GetComponentInChildren<TextMeshProUGUI>();


        GetButtonUi = GetButton.GetComponent<Button>();
        GetButtonUi.onClick.AddListener(ClickGet);

        //SetPlayerPrefs();
    }

    void Start()
    {
        effectText.text = achData.effect;
        SetImage();
    }

    private void OnEnable()
    {
        Complete();
        GetDown();
        SetSlider();
    }

    private void Update()
    {
        if (!get)
            return;
    }

    // �޼� �����̴� ����
    void SetSlider()
    {
        if (slider == null)
            return;

        float amount;

        switch (achData.achType)
        {
            case AchType.bestScore:
                amount = PlayerPrefs.GetInt("BestScore");
                sliderText.text = amount.ToString() + "<#b3bedb>/" + achData.amount.ToString();
                break;
            case AchType.totalDistance:
                amount = PlayerPrefs.GetInt("TotalDistance");
                sliderText.text = amount.ToString() + "<#b3bedb>/" + achData.amount.ToString();
                break;
            case AchType.killDefenders:
                amount = PlayerPrefs.GetInt("BreakThrough");
                sliderText.text = amount.ToString() + "<#b3bedb>/" + achData.amount.ToString();
                break;
            case AchType.topStage:
                amount = PlayerPrefs.GetInt("TopImpactZone");
                sliderText.text = amount.ToString() + "<#b3bedb>/" + achData.amount.ToString();
                break;
            default:
                amount = 0;
                break;
        }

        float goalRate = amount / achData.amount;
        slider.value = goalRate;
        Debug.Log(goalRate);


        if (goalRate >= 1)
        {
            NoGetState();
        }
    }

    void SetImage()
    {
        switch (achData.achReward)
        {
            case AchReward.staminaUse:
                image.sprite = achData.staminaUseImage;
                break;
            case AchReward.staminaRegen:
                image.sprite = achData.staminaRegenImage;
                break;
            case AchReward.impactDmg:
                image.sprite = achData.impactDmgImage;
                break;
            case AchReward.pointPlus:
                image.sprite = achData.pointPlusImage;
                break;
            case AchReward.ball:
                break;
            default:
                break;
        }
    }
    // �޼������� ���� ������ �ʾ�����
    void NoGetState()
    {
        if (!get)
        {
            slider.gameObject.SetActive(false);
            GetButton.SetActive(true);
        }
    }

    void ClickGet()
    {
        GetButton.SetActive(false);
        get = true;
        PlayerPrefs.SetFloat(achData.achName, achData.effectAmount);
        PlayerPrefs.Save();
        GetDown();
    }

    // �����
    void SetPlayerPrefs()
    {
        PlayerPrefs.SetInt("BestScore", 4000);
        PlayerPrefs.SetInt("TotalDistance", 1500);
        PlayerPrefs.SetInt("BreakThrough", 1500);
        PlayerPrefs.SetInt("TopImpactZone", 5);
        PlayerPrefs.Save();

    }

    // �Ϸ�������
    void Complete()
    {
        if (!get)
        { 
            if (PlayerPrefs.HasKey(achData.achName))
            {
                get = true;
            }
        }
    }

    // ������ ������ �Լ�
    void GetDown()
    {
        if (get)
        {
            GetButton.SetActive(false);
            slider.gameObject.SetActive(false);
            completePanel.SetActive(true);
            completeSign.SetActive(true);
            gameObject.transform.SetAsLastSibling();
        }
    }

    //// �����̴� ��ġ �����ֱ�
    //void SetSliderText()
    //{
    //    switch (ballData.goalType)
    //    {
    //        case GoalType.distance:
    //            sliderText.text = ballData.amount.ToString();

    //            break;
    //        case GoalType.point:
    //            break;
    //    }
    //    sliderText.text = ballData.amount.ToString();
    //}
}
