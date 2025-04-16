using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 유저 스탯관리
public class StartManager : MonoBehaviour
{
    public static StartManager Instance;

    public AchData[] achDatas;

    public float staminaUse;
    public float staminaRegen;
    public float impactDmg;
    public float scoreMulti;
    public int ball;

    void Awake()
    {
        // 이미 인스턴스가 존재하면 새로 생긴 것은 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스가 없다면 이걸로 설정하고 씬 전환 시 파괴 안되게 함
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void CheckAch()
    {
        int equippedBall = PlayerPrefs.GetInt("EquippedBall");
        ball = equippedBall;
        Debug.Log(ball);

        if (achDatas == null) 
            return; 
        foreach (AchData data in achDatas)
        {
            if (PlayerPrefs.HasKey(data.achName))
            {
                switch (data.achReward)
                {
                    case AchData.AchReward.staminaUse:
                        staminaUse += data.effectAmount;
                        staminaUse = 20;
                        break;
                    case AchData.AchReward.staminaRegen:
                        staminaRegen += data.effectAmount;
                        staminaRegen = 1;
                        break;
                    case AchData.AchReward.impactDmg:
                        impactDmg += data.effectAmount;
                        break;
                    case AchData.AchReward.pointPlus:
                        scoreMulti += data.effectAmount;
                        break;
                    default:
                        break;
                }

            }
        }
        scoreMulti += 1f;       // 기본 배율 1 추가
    }
}
