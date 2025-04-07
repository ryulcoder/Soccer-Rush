using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class AchData : ScriptableObject 
{ 
    public enum AchType { bestScore, totalDistance, topStage, killDefenders}
    public enum AchReward { staminaUse, staminaRegen, stageDamage, ball, pointPlus}
    
    public AchType achType;     // 업적 분류
    public AchReward achReward;     // 업적 분류

    public string achName;      // 업적 이름
    public string effect;       // 업적 효과 설명

    public int amount;          // 필요 양
    public int effectAmount;    // 스탯 주는 양
}
