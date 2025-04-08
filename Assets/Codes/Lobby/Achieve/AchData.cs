using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class AchData : ScriptableObject 
{ 
    public enum AchType { bestScore, totalDistance, topStage, killDefenders}
    public enum AchReward { staminaUse, staminaRegen, impactDmg , ball, pointPlus}


    public Sprite staminaUseImage;
    public Sprite staminaRegenImage;
    public Sprite impactDmgImage;
    public Sprite ballImage;
    public Sprite pointPlusImage;


    public AchType achType;     // ���� �з�
    public AchReward achReward;     // ���� �з�

    public string achName;      // ���� �̸�
    public string effect;       // ���� ȿ�� ����

    public int amount;          // �ʿ� ��
    public float effectAmount;    // ���� �ִ� ��

    public Sprite GetRewardSprite()
    {
        switch (achReward)
        {
            case AchReward.staminaUse: return staminaUseImage;
            case AchReward.staminaRegen: return staminaRegenImage;
            case AchReward.impactDmg: return impactDmgImage;
            case AchReward.ball: return ballImage;
            case AchReward.pointPlus: return pointPlusImage;
            default: return null;
        }
    }
}
