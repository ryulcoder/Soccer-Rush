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


    public AchType achType;     // 업적 분류
    public AchReward achReward;     // 업적 분류

    public string achName;      // 업적 이름
    public string effect;       // 업적 효과 설명

    public int amount;          // 필요 양
    public float effectAmount;    // 스탯 주는 양

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
