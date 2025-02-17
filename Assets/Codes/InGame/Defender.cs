using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public bool isHit;

    [Space]

    public DefenderFootTrigger[] FootTriggers;

    Animator DefenderAni;
    BoxCollider boxCollider;
    SkinnedMeshRenderer skinRenderer;

    float fadeDuration = 2f; // 페이드 지속 시간
    float totalSpeed;
    string anomalyStr;

    AnimatorStateInfo stateInfo;

    void Awake()
    {
        totalSpeed = 0;
        DefenderAni = gameObject.GetComponent<Animator>();
        skinRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    void FixedUpdate()
    {
        SlidingTackleFront_Update();
    }

    // 달리기 후 슬라이딩 태클을 하는 수비수 업데이트 로직
    void SlidingTackleFront_Update()
    {
        if (isTackle && currentState.ToString() == "Sliding_Tackle_Front")
        {
            stateInfo = DefenderAni.GetCurrentAnimatorStateInfo(0);

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
            anomalyUserState = "GetStandTackled_Front";
        }

        DefenderAni.SetBool(anomalyStr, true);

    }

    // 리셋
    public void Reset()
    {
        isTackle = false;
        totalSpeed = 0; 
        DefenderAni.speed = 1;

        if (currentState.ToString() == "Sliding_Tackle_Anomaly")
            DefenderAni.SetBool(anomalyStr, false);
        else
            DefenderAni.SetBool(currentState.ToString(), false);

        anomalyStr = "";
        anomalyUserState = "";

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        // 특정 거리 플레이어가 트리거에 들어올 시 태클 시작
        if (collider.gameObject.name == "PlayerTriggerBox")
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

        if (collider.CompareTag("Ball"))
        {
            if (isHit) return;

            isHit = true;
            boxCollider.enabled = false;
            if (skinRenderer != null)
            {
                Debug.Log("랜더러 있음");
                Material material = skinRenderer.material;
                Color startColor = material.color;

                material.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    gameObject.SetActive(false); // 페이드 완료 후 오브젝트 비활성화
                });
            }
            //transform.gameObject.SetActive(false);
        }
    }
}
