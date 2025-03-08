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

    Player Player;

    [Space][SerializeField] int tutoNum;
    bool isSet;

    void Start()
    {
        Player = Player.Instance;

        //TutorialOn(PlayerPrefs.GetInt("Tutorial", 0) == 0);
    }

    void Update()
    {
        if (!isSet && tutoNum >= 7 && Player.transform.position.z >= 650)
        {
            isSet = true;

            TutoralSet(tutoNum);
        }
        else if (!isSet && tutoNum == 6 && Player.transform.position.z >= 540)
        {
            isSet = true;

            ShootBtn.SetActive(true);

            TutoralSet(tutoNum);
        }
        else if (!isSet && tutoNum == 4 && Player.transform.position.z >= 390)
        {
            isSet = true;
            TutoralSet(tutoNum);
        }
    }

    void TutorialOn(bool isActive)
    {
        TutorialFloor.SetActive(isActive);

        if (isActive)
            for (int i = 0; i < Floors.Length; i++)
                Floors[i].transform.position += Vector3.forward * 500;

        ExtraScore.tutorial = isActive;

        ExDefenders.SetActive(isActive);

        SwipInput.SetActive(!isActive);
        ShootBtn.SetActive(!isActive);

        gameObject.SetActive(isActive);

        TutoralSet(tutoNum);
    }


    public void TutoralSet(int num)
    {
        TutorialBtn.onClick.RemoveAllListeners();

        TutorialBtn.onClick.AddListener(() => TutoListener(num));

        TutorialBlur.SetActive(true);

        TutoUIs[num].SetActive(true);

        Time.timeScale = 0;
    }

    void TutoListener(int num)
    {
        tutoNum = num;

        Time.timeScale = gameSpeed;

        TutoUIs[num].SetActive(false);

        TutorialBlur.SetActive(false);

        switch (num)
        {
            case 0:
                break;

            case 1:
                Player.MoveLeftRight(-1);
                break;

            case 2:
                Player.Jump(); 
                break;

            case 3:
                tutoNum += 1;

                Player.Jump();
                break;

            case 4:
                Player.MoveLeftRight(1);
                break;

            case 5:
                isSet = false;
                tutoNum += 1;

                Player.Spin();
                break;

            case 6:
                isSet = false;
                tutoNum += 1;

                Player.ShootingAni();
                break;

            case 7:
                tutoNum += 1;
                TutorialEnd();
                break;
        }
    }

    void TutorialEnd()
    {
        SwipInput.SetActive(true);

        ExDefenders.SetActive(false);
        TutorialFloor.SetActive(false);

        ExtraScore.tutorial = false;

        Time.timeScale = gameSpeed;

        /*PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.Save();*/

        TutorialBlur.SetActive(false);

        gameObject.SetActive(false);
    }

}
