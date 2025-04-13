using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootTrigger : MonoBehaviour
{
    public Player Player;
    Defender Defender;

    bool getTackle;
    string stateName;

    private void OnTriggerEnter(Collider collider)
    {
        // 태클을 당했을 시
        if (!getTackle && collider.gameObject.name == "TackleFoot"&& !collider.gameObject.GetComponent<DefenderFootTrigger>().Defender.isHit)
        {
            getTackle = true;

            Defender = collider.gameObject.GetComponent<DefenderFootTrigger>().Defender;

            stateName = Defender.currentState.ToString();

            switch (stateName)
            {
                case "Stand_Tackle_Front":
                    stateName = "GetStandTackled_Front";
                    break;
                case "Sliding_Tackle_Front":
                    stateName = "GetTackled_Front";
                    break;
                case "Sliding_Tackle_Anomaly":
                    stateName = Defender.anomalyUserState;
                    break;
            }

            Player.GetTackled(stateName);

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (getTackle && collider.gameObject.name == "TackleFoot")
        {
            getTackle = false;
        }
    }
}
