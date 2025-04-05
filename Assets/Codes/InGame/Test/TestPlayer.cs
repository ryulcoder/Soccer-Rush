using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public TestBall TestBall;
    public Animator playerAni;

    public bool on;

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
        yield return new WaitForSeconds(1);

        playerAni.SetTrigger("PenaltyKick");
    }

    public void PenaltyKickOn()
    {
        TestBall.PenalKickOn();
    }
}
