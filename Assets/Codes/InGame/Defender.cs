using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    // 수비 타입
    public enum States
    {
        Stand_Tackle_Front,
        Sliding_Tackle_Front,
        Sliding_Tackle_Anomaly
    }

    // enum 타입의 변수를 선언
    public States currentState;

    public string anomalyUserState;
    public bool isTackle;
    public bool isHit;  // 공에 맞았는지 확인

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

    // 달리기 후 슬라이딩 태클을 하는 수비수 업데이트 로직
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
                // 러닝 태클 시 천천히 속도 올리기
                if (totalSpeed >= 0.7f)
                {
                    totalSpeed = 0.7f;
                }
                else
                    totalSpeed += 0.1f;

                // 드리블 이동
                transform.position += Vector3.back * totalSpeed;
            }

        }

    }

    // 태클 시작
    void Tackle()
    {
        DefenderAni.SetBool(currentState.ToString(), true);
    }
    // 특수 수비수 전용 태클 함수
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

    // 리셋
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
        // 특정 거리 플레이어가 트리거에 들어올 시 태클 시작
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

    //// 공 맞을시 플레이어 죽지 않게 콜라이더 작용 끔
    //public void ShootingHitOffCollider()
    //{
    //    isHit = true;
    //}

    // 공맞아 뒤짐
    public void ShootingHitDeath()
    {
        transform.gameObject.SetActive(false);
    }
}
