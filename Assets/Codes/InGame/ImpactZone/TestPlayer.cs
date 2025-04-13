using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public static TestPlayer Instance;

    public Animator playerAni;

    public bool on;

    bool right;

    float returnZero = 1;

    Vector3 defaultVec;

    AnimatorStateInfo stateInfo;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        right = ImpactZone.Instance.right;

        if (on)
        {
            returnZero = 1;

            playerAni.SetTrigger("Impact");

            StartCoroutine(KickDelay());
        }
    }

    void FixedUpdate()
    {
        stateInfo = playerAni.GetCurrentAnimatorStateInfo(0);

        if (on && (stateInfo.IsName("Shoot_Right") || stateInfo.IsName("Shoot_Center")))
        {
            if (returnZero == 0 && transform.position.x <= 0) 
                on = false;

            transform.position = Vector3.MoveTowards(transform.position, defaultVec + 2 * returnZero * Vector3.left, Time.deltaTime);
        }
    }

    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.4f);

        if (right)
            playerAni.SetTrigger("Impact_Kick_Right");
        else
            playerAni.SetTrigger("Impact_Kick_Left");

        //Time.timeScale = 0.2f;
    }

    public void ImpactZoneKickOn()
    {
        returnZero = 0;

        ImpactZone.Instance.ImpactKickOn();

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
