using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    Animator DefenderAni;

    public string anomalyUserState;

    bool isTackle;
    float floorDis;
    string anomalyStr;

    public enum States
    {
        Stand_Tackle_Front,
        Sliding_Tackle_Front,
        Sliding_Tackle_Anomaly
    }

    // enum 타입의 변수를 선언
    public States currentState;

    private void Awake()
    {
        DefenderAni = gameObject.GetComponent<Animator>();
        floorDis = GameObject.FindGameObjectWithTag("Floor").transform.localScale.y / 3;
    }

    void Tackle()
    {
        DefenderAni.SetBool(currentState.ToString(), true);

    }

    void Tackle(Transform player)
    {
        float playerdis = player.position.x - transform.position.x;

        if (playerdis < 0)
        {
            anomalyStr = "Sliding_Tackle_Right";
            anomalyUserState = "GetTackled_Right";
        }
        else if (playerdis > 0)
        {
            anomalyStr = "Sliding_Tackle_Left";
            anomalyUserState = "GetTackled_Left";
        }
        else
        {
            anomalyStr = "Stand_Tackle_Front";
            anomalyUserState = "GetStandTackled_Front";
        }

        if (Mathf.Abs(playerdis) / floorDis >= 2)
            DefenderAni.speed = 1.5f;

        DefenderAni.SetBool(anomalyStr, true);

    }

    public void Reset()
    {
        isTackle = false;
        DefenderAni.speed = 1;

        if (currentState.ToString() == "Sliding_Tackle_Anomaly")
            DefenderAni.SetBool(anomalyStr, false);
        else
            DefenderAni.SetBool(currentState.ToString(), false);
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
