using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BallData;

public class Achievements : MonoBehaviour
{
    public AchData achData;

    bool get;

    public int amount;

    Slider slider;
    TextMeshProUGUI sliderText;
    public TextMeshProUGUI effectText;

    public GameObject GetButton;
    Button GetButtonUi;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        sliderText = slider.GetComponentInChildren<TextMeshProUGUI>();


        GetButtonUi = GetButton.GetComponent<Button>();
        GetButtonUi.onClick.AddListener(ClickGet);

        SetPlayerPrefs();
    }

    private void OnEnable()
    {
        effectText.text = achData.effect;

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
    }

    // 달성 슬라이더 조정
    void SetSlider()
    {
        if (slider == null)
            return;

        float amount;

        if (ballData.goalType == GoalType.totaldistance)
        {
            amount = PlayerPrefs.GetInt("TotalDistance");
            Debug.Log(amount);
            sliderText.text = "TotalDistance" + amount.ToString() + "<#b3bedb>/" + ballData.amount.ToString();

        }
        else if (ballData.goalType == GoalType.bestDistance)
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

    // 달성했지만 겟은 누르지 않았을때
    void NoGetState()
    {
        slider.gameObject.SetActive(false);
        GetButton.SetActive(true);
    }

    // 이미 받은 상태일때
    void GetState()
    {
        slider.gameObject.SetActive(false);
        if ((int)ballData.ballType == PlayerPrefs.GetInt("BallNum"))
        {
            //Equipped.SetActive(true);
        }
    }

    void ClickGet()
    {
        GetButton.SetActive(false);
        get = true;
    }

    void ClickEquip()
    {
        PlayerPrefs.SetInt("BallNum", (int)ballData.ballType);
        PlayerPrefs.Save();
    }

    void SetPlayerPrefs()
    {
        PlayerPrefs.SetInt("TotalDistance", 100);
        PlayerPrefs.Save();

    }

    // 완료했을시 밑에 내리기
    void CompleteDown()
    {

    }
    //// 슬라이더 수치 정해주기
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
