using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ���� ���Ȱ���
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
        // �̹� �ν��Ͻ��� �����ϸ� ���� ���� ���� �ı�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // �ν��Ͻ��� ���ٸ� �̰ɷ� �����ϰ� �� ��ȯ �� �ı� �ȵǰ� ��
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
        scoreMulti += 1f;       // �⺻ ���� 1 �߰�
    }
}
