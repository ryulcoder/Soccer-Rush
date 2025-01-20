using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{

    bool isTackle;
    public string anomalyStr;

    public enum States
    {
        Stand_Tackle_Front,
        Sliding_Tackle_Front,
        Sliding_Tackle_Anomaly
    }

    // enum 타입의 변수를 선언
    public States currentState;

    void Tackle()
    {
        gameObject.GetComponent<Animator>().SetBool(currentState.ToString(), true);

    }

    void Tackle(Transform player)
    {
        float playerX = player.position.x;

        if (playerX < 0)
            anomalyStr = "Sliding_Tackle_Right";
        
        else if (playerX > 0)
            anomalyStr = "Sliding_Tackle_Left";
        
        else
            anomalyStr = "Stand_Tackle_Front";
        
        gameObject.GetComponent<Animator>().SetBool(anomalyStr, true);

    }

    public void Reset()
    {
        isTackle = false;

        gameObject.GetComponent<Animator>().SetBool(currentState.ToString(), false);
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
