using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goalkeeper : MonoBehaviour
{
    public static Goalkeeper Instance;

    public Transform ImpactZoneArea;
    public Transform GoalKeeperTrans;
    public Transform GoalKeeperArmature;
    public Transform GoalPoint;

    public Animator GoalkeeperAni;

    [SerializeField] float reactSpeed = 0.1f;
    float divingSpeed;
    bool right, jump, fall;

    public bool isGoal;

    private void Awake()
    {
        Instance = this;
    }


    void FixedUpdate()
    {
        if (jump)
        {
            if (GoalKeeperTrans.localPosition.y >= GoalPoint.localPosition.y)
            {
                jump =false;
                fall = true;

                return;
            }

            GoalKeeperTrans.localPosition += divingSpeed * Time.deltaTime * Vector3.up;

        }

        if (fall) 
        {
            if (GoalKeeperTrans.localPosition.y <= 0)
            {
                fall = false;

                GoalKeeperTrans.localPosition = Vector3.right * GoalKeeperTrans.localPosition.x + Vector3.forward * GoalKeeperTrans.localPosition.z;
                return;
            }

            GoalKeeperTrans.localPosition += divingSpeed * Time.deltaTime * Vector3.down;
        }

    }


    public void Reset()
    {
        right = ImpactZone.Instance.right;

        divingSpeed = 10;

        jump = false; 
        fall = false;
        isGoal = false;

        GoalkeeperAni.Play("Idle", -1, 0f);
    }


    public void DivingStart()
    {
        StartCoroutine(DivingDelay());
    }

    IEnumerator DivingDelay()
    {
        yield return new WaitForSeconds(reactSpeed);

        if (right)
            GoalkeeperAni.SetTrigger("DivingLeft");
        else
            GoalkeeperAni.SetTrigger("DivingRight");
    }

    public void DivingJump()
    {
        jump = true;
    }
    public void DivingFall()
    {
        divingSpeed = 8;

        jump = false;
        fall = true;
    }

    public void DivingEnd()
    {
        if (isGoal)
            GoalkeeperAni.SetTrigger("Goal");
        else
        {
            ImpactZone.Instance.DropBall();
            GoalkeeperAni.SetTrigger("NoGoal");
        }

        Debug.LogWarning("DivingEnd");
    }

}
