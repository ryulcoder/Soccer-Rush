using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    // ���� Ÿ��
    public enum States
    {
        Stand_Tackle_Front,
        Sliding_Tackle_Front,
        Sliding_Tackle_Anomaly
    }

    // enum Ÿ���� ������ ����
    public States currentState;

    public string anomalyUserState;
    public bool isTackle;
    public bool isHit;  // ���� �¾Ҵ��� Ȯ��

    [Space]

    public DefenderFootTrigger[] FootTriggers;

    Animator DefenderAni;
    float totalSpeed, playerSubDis;
    string anomalyStr;
    bool isSliding_Front;

    public AnimatorStateInfo stateInfo;

    void Start()
    {
        totalSpeed = 0;
        DefenderAni = gameObject.GetComponent<Animator>();

        isSliding_Front = currentState.ToString() == "Sliding_Tackle_Front";
    }

    void OnEnable()
    {
        isHit = false;
    }

    void FixedUpdate()
    {
        Defender_Update();
    }

    // �޸��� �� �����̵� ��Ŭ�� �ϴ� ����� ������Ʈ ����
    void Defender_Update()
    {
        playerSubDis = Vector3.Distance(Player.Instance.transform.position, transform.position);

        if (isTackle && GameManager.Instance.aroundDefenderClear) Reset();

        else if (playerSubDis <= 100 && GameManager.Instance.aroundDefenderClear) Reset();
        

        stateInfo = DefenderAni.GetCurrentAnimatorStateInfo(0);

        // SlidingTackleFront_Update
        if (isTackle && isSliding_Front && !isHit)
        {
            if (!stateInfo.IsName("Wait") && !stateInfo.IsName("Tackle_Front"))
            {
                // ���� ��Ŭ �� õõ�� �ӵ� �ø���
                if (totalSpeed >= 0.7f)
                {
                    totalSpeed = 0.7f;
                }
                else
                    totalSpeed += 0.1f;

                // �帮�� �̵�
                transform.position += Vector3.back * totalSpeed;
            }

        }

    }

    // ��Ŭ ����
    void Tackle()
    {
        DefenderAni.SetBool(currentState.ToString(), true);
    }
    // Ư�� ����� ���� ��Ŭ �Լ�
    void Tackle(Transform player)
    {
        float playerdis = player.position.x - transform.position.x;

        if (playerdis < -0.5f)
        {
            anomalyStr = "Sliding_Tackle_Right";
            anomalyUserState = "GetTackled_Right";
        }
        else if (playerdis > 0.5f)
        {
            anomalyStr = "Sliding_Tackle_Left";
            anomalyUserState = "GetTackled_Left";
        }
        else
        {
            anomalyStr = "Stand_Tackle_Front";
            anomalyUserState = "GetStandTackled_Front_Anomaly";
        }

        DefenderAni.SetBool(anomalyStr, true);

    }

    // ����
    public void Reset()
    {
        isTackle = false;
        isHit = false;
        totalSpeed = 0; 
        DefenderAni.speed = 1;

        if (currentState.ToString() == "Sliding_Tackle_Anomaly")
        {
            if (anomalyStr != null && anomalyStr != "")
                DefenderAni.SetBool(anomalyStr, false);
        } 
        else
            DefenderAni.SetBool(currentState.ToString(), false);

        anomalyStr = "";
        anomalyUserState = "";

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Ư�� �Ÿ� �÷��̾ Ʈ���ſ� ���� �� ��Ŭ ����
        if (collider.gameObject.name == "PlayerTriggerBox" && !isHit)
        {
            if (isTackle) return;

            isTackle = true;

            FootTriggers[0].CheckLimit();
            FootTriggers[1].CheckLimit();

            if (currentState.ToString() == "Sliding_Tackle_Anomaly")
                Tackle(collider.transform);
            else
                Tackle();
        }
    }

    //// �� ������ �÷��̾� ���� �ʰ� �ݶ��̴� �ۿ� ��
    //public void ShootingHitOffCollider()
    //{
    //    isHit = true;
    //}

    // ���¾� ����
    public void ShootingHitDeath()
    {
        transform.gameObject.SetActive(false);
    }
}
