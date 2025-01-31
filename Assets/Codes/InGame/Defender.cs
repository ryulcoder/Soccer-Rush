using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Defender : MonoBehaviour
{
    Animator DefenderAni;

    public string anomalyUserState;

    float floorDis, totalSpeed;
    bool isTackle;
    string anomalyStr;

    AnimatorStateInfo stateInfo;

    public enum States
    {
        Stand_Tackle_Front,
        Sliding_Tackle_Front,
        Sliding_Tackle_Anomaly
    }

    // enum 타입의 변수를 선언
    public States currentState;

    void Awake()
    {
        totalSpeed = 0;
        DefenderAni = gameObject.GetComponent<Animator>();
        floorDis = GameObject.FindGameObjectWithTag("Floor").transform.localScale.x / 3;
    }

    void FixedUpdate()
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

    void Tackle()
    {
        DefenderAni.SetBool(currentState.ToString(), true);

    }

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
        if (collider.gameObject.name == "PlayerTriggerBox")
        {
            if (isTackle) return;

            isTackle = true;

            if (currentState.ToString() == "Sliding_Tackle_Anomaly")
                Tackle(collider.transform);
            else
                Tackle();
        }
    }
}
