using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class AchData : ScriptableObject 
{ 
    public enum AchType { bestScore, totalDistance, topStage, killDefenders}
    public enum AchReward { staminaUse, staminaRegen, stageDamage, ball, pointPlus}
    
    public AchType achType;     // ���� �з�
    public AchReward achReward;     // ���� �з�

    public string achName;      // ���� �̸�
    public string effect;       // ���� ȿ�� ����

    public int amount;          // �ʿ� ��
    public int effectAmount;    // ���� �ִ� ��
}
