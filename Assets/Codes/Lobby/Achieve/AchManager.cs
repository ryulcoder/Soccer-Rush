using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchManager : MonoBehaviour
{
    public GameObject bang;
    public AchData[] achDatas;

    public float bestScore;
    public float totalDistance;
    public float breakThrough;
    public float topImpactZone;


    void Awake()
    {
    }

    void Start()
    {
        achDatas = StartManager.Instance.achDatas;

        SetAchStat();
        CheckBangOn();
    }

    void SetAchStat()
    {
        bestScore = PlayerPrefs.GetInt("BestScore");
        totalDistance = PlayerPrefs.GetInt("TotalDistance");
        breakThrough = PlayerPrefs.GetInt("BreakThrough");
        topImpactZone = PlayerPrefs.GetInt("TopImpactZone");
    }

    void CheckBangOn()
    {
        if (achDatas == null)
            return;
        foreach (AchData data in achDatas)
        {
            if (!PlayerPrefs.HasKey(data.achName))
            {
                switch (data.achType)
                {
                    case AchData.AchType.bestScore:
                        if(bestScore > data.amount)
                        {
                            bang.SetActive(true);
                        }
                        break;
                    case AchData.AchType.totalDistance:
                        if (totalDistance > data.amount)
                        {
                            bang.SetActive(true);
                        }
                        break;
                    case AchData.AchType.killDefenders:
                        if (breakThrough > data.amount)
                        {
                            bang.SetActive(true);
                        }
                        break;
                    case AchData.AchType.topStage:
                        if (topImpactZone > data.amount)
                        {
                            bang.SetActive(true);
                        }
                        break;
                    default:
                        break;

                }
            }

        }
    }
}