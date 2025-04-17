using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public float gameSpeed;

    [Header("Tutorial")]
    public GameObject ExDefenders;
    public GameObject TutorialBlur;
    public Button TutorialBtn;
    public GameObject TutorialFloor;
    public GameObject[] TutoUIs;

    [Header("Other")]
    public ExtraScore ExtraScore;
    public GameObject SwipInput;
    public GameObject ShootBtn;
    public GameObject[] Floors;
    public GameObject ResumeCount;

    [Space]
    public bool NoTuto, isTuto, isSelfPause;

    Player Player;

    int tutoNum;
    bool isSet;
        

    void Awake()
    {
#if (UNITY_EDITOR)
        PlayerPrefs.DeleteKey("Tutorial");
#else
        NoTuto = false;
#endif
        TutorialOn(PlayerPrefs.GetInt("Tutorial", 0) == 0 && !NoTuto);
    }

    void Start()
    {
        Player = Player.Instance;
    }

    void Update()
    {
        if (!isSet && tutoNum == 6 && Player.transform.position.z >= 650)
        {
            isSet = true;

            TutorialEnd();
        }
        else if (!isSet && tutoNum == 5 && Player.transform.position.z >= 540)
        {
            isSet = true;

            ShootBtn.SetActive(true);

            TutoralSet(tutoNum);
        }
        else if (!isSet && tutoNum == 3 && Player.transform.position.z >= 390)
        {
            isSet = true;
            TutoralSet(tutoNum);
        }
    }

    void TutorialOn(bool isActive)
    {
        isTuto = isActive;

        TutorialFloor.SetActive(isActive);

        TutoUIs[^1].GetComponent<RectTransform>().anchoredPosition = ShootBtn.GetComponent<RectTransform>().anchoredPosition + Vector2.up * 300;

        if (isActive)
            for (int i = 0; i < Floors.Length; i++)
                Floors[i].transform.position += Vector3.forward * 500;

        ExtraScore.tutorial = isActive;

        ExDefenders.SetActive(isActive);

        SwipInput.SetActive(!isActive);
        ShootBtn.SetActive(!isActive);

        gameObject.SetActive(isActive);
    }


    public void TutoralSet(int num)
    {
        isSelfPause = true;

        Time.timeScale = 0;

        TutorialBtn.onClick.RemoveAllListeners();

        TutorialBtn.onClick.AddListener(() => TutoListener(num));

        TutorialBlur.SetActive(true);

        TutoUIs[num].SetActive(true);
    }

    void TutoListener(int num)
    {
        if (ResumeCount.activeSelf) return;

        tutoNum = num;

        isSelfPause = false;

        Time.timeScale = gameSpeed;

        TutoUIs[num].SetActive(false);

        TutorialBlur.SetActive(false);

        switch (num)
        {
            case 0:
                Player.MoveLeftRight(-1);
                break;

            case 1:
                Player.Jump(); 
                break;

            case 2:
                tutoNum += 1;

                Player.Jump();
                break;

            case 3:
                Player.MoveLeftRight(1);
                break;

            case 4:
                isSet = false;
                tutoNum += 1;

                Player.Spin();
                break;

            case 5:
                isSet = false;
                tutoNum += 1;

                Player.ShootingAni();

                SwipInput.SetActive(true);

                break;
        }
    }

    void TutorialEnd()
    {
        ExDefenders.SetActive(false);
        TutorialFloor.SetActive(false);

        ExtraScore.tutorial = false;
#if !UNITY_EDITOR
        PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.Save();
#endif
        TutorialBlur.SetActive(false);

        gameObject.SetActive(false);

        isTuto = false;
    }

}
