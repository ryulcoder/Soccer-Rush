using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public static TestPlayer Instance;

    public Animator playerAni;

    public bool on;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (on)
        {
            playerAni.SetTrigger("Penal");

            StartCoroutine(KickDelay());
        }
    }

    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.4f);

        playerAni.SetTrigger("PenaltyKick");

        //Time.timeScale = 0.2f;
    }

    public void PenaltyKickOn()
    {
        Penalty.Instance.PenalKickOn();

        //Time.timeScale = 1;
    }

    public void GoalCheck(bool isGoal)
    {
        if (isGoal)
            playerAni.SetTrigger("Goal");
        else
            playerAni.SetTrigger("NoGoal");
    }
}
