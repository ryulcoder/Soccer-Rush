using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootTrigger : MonoBehaviour
{
    public Player Player;

    bool getTackle;

    private void OnTriggerEnter(Collider collider)
    {
        // 태클을 당했을 시
        if (!getTackle && collider.gameObject.name == "TackleFoot")
        {
            getTackle = true;

            Defender defender = collider.gameObject.GetComponent<DefenderFootTrigger>().Defender;

            string stateName = defender.currentState.ToString();

            switch (stateName)
            {
                case "Stand_Tackle_Front":
                    stateName = "GetStandTackled_Front";
                    break;
                case "Sliding_Tackle_Front":
                    stateName = "GetTackled_Front";
                    break;
                case "Sliding_Tackle_Anomaly":
                    stateName = defender.anomalyUserState;
                    break;
            }

            Player.GetTackled(stateName);

            getTackle = false;
        }
    }
}
