using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPost : MonoBehaviour
{
    bool isGoal;

    public void Reset()
    {
        isGoal = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball") && !isGoal)
        {
            isGoal = true;

            if (LobbyAudioManager.instance != null)
            {

                LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.goalNet);

            }


            ImpactZone.Instance.GoalAndDropBall();
        }
    }
}
