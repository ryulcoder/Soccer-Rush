using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BallData;

public class BallAc : MonoBehaviour
{
    public BallData ballData;

    bool get;

    public int amount;

    Slider slider;
    TextMeshProUGUI sliderText;
    public TextMeshProUGUI effectText;
    Toggle toggle;

    public GameObject GetButton;
    Button GetButtonUi;

    public GameObject EquipButton;
    Button EquipButtonUi;

    public GameObject Equipped;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        toggle = GetComponentInChildren<Toggle>();
        sliderText = slider.GetComponentInChildren<TextMeshProUGUI>();

        EquipButtonUi = EquipButton.GetComponent<Button>();
        EquipButtonUi.onClick.AddListener(ClickEquip);

        GetButtonUi = EquipButton.GetComponent<Button>();
        GetButtonUi.onClick.AddListener(ClickGet);

        if (ballData.basic)
        {
            get = true;
        }
        SetPlayerPrefs();
    }

    private void OnEnable()
    {
        effectText.text = ballData.ballAblity;
        
        if (get)
        {
            GetState();
        }
        else
        {
            SetSlider();
        }

    }

    private void Update()
    {
        if (!get)
            return;

        if (toggle.isOn)
        {
            Equipped.SetActive(true);
            EquipButton.SetActive(false);
        }
    }

    // �޼� �����̴� ����
    void SetSlider()
    {
        if (slider == null)
            return;

        float amount;

        if (ballData.goalType == GoalType.totaldistance)
        {
            amount = PlayerPrefs.GetInt("TotalDistance");
            Debug.Log(amount);
            sliderText.text = "TotalDistance" + amount.ToString()+ "<#b3bedb>/"+ ballData.amount.ToString();

        }
        else if(ballData.goalType == GoalType.bestDistance)
        {
            amount = PlayerPrefs.GetInt("bestDistance");
            sliderText.text = "bestDistance" + amount.ToString() + "<#b3bedb>/" + ballData.amount.ToString();
        }
        else
        {
            amount = 0;
        }

        float goalRate = amount / ballData.amount;
        slider.value = goalRate;
        Debug.Log(goalRate);


        if (goalRate >= 1)
        {
            NoGetState();
        }
    }

    // �޼������� ���� ������ �ʾ�����
    void NoGetState()
    {
        slider.gameObject.SetActive(false);
        GetButton.SetActive(true);
    }

    // �̹� ���� �����϶�
    void GetState()
    {
        slider.gameObject.SetActive(false);
        if ((int)ballData.ballType == PlayerPrefs.GetInt("BallNum"))
        {
            Equipped.SetActive(true);
        }
        else
        {
            EquipButton.SetActive(true);
        }
    }

    void ClickGet()
    {
        GetButton.SetActive(false);
        EquipButton.SetActive(true);
        get = true;
    }

    void ClickEquip()
    {
        PlayerPrefs.SetInt("BallNum", (int)ballData.ballType);
        PlayerPrefs.Save();
        toggle.isOn = true;
    }

    void SetPlayerPrefs()
    {
        PlayerPrefs.SetInt("TotalDistance",100);
        PlayerPrefs.Save();

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
