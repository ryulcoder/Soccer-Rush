using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goalkeeper : MonoBehaviour
{
    public static Goalkeeper Instance;

    public Transform GoalKeeperTrans;
    public Transform GoalKeeperHip;
    public Animator GoalkeeperAni;

    public Transform GoalPoint;

    Vector3 divingDir;

    float reactSpeed = 0.1f;
    float divingSpeed;
    bool right, jump, fall, nextY;

    public bool isGoal;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GoalKeeperTrans.localPosition = 9 * Vector3.back;

        if (Penalty.Instance.right)
        {
            right = true;
            divingDir = (Vector3.up + Vector3.right).normalized;
            divingDir.x = 1.7f;
        }
        else
        {
            right = false;
            divingDir = (Vector3.up + Vector3.left).normalized;
            divingDir.x = -1.7f;
        }

        divingSpeed = 10;
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

            GoalKeeperTrans.localPosition += divingSpeed * Time.deltaTime * divingDir;
            divingSpeed += Time.deltaTime * 7;

        }

        if (fall) 
        {
            if (GoalKeeperTrans.localPosition.y <= 0)
            {
                fall = false;

                GoalKeeperTrans.localPosition = new(GoalKeeperTrans.localPosition.x, 0, GoalKeeperTrans.localPosition.z);
                return;
            }
            else if (!nextY && GoalKeeperTrans.localPosition.y <= 1)
            {
                nextY = true;
                divingSpeed = 1;
            }

            GoalKeeperTrans.localPosition += divingSpeed * Time.deltaTime * divingDir;
            divingSpeed += Time.deltaTime * 15;
        }
    }

    public void Reset()
    {
        right = false;
        jump = false; 
        fall = false;
        isGoal = false;
        nextY = false;

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

        if (right)
            divingDir = Vector3.down + Vector3.right * 0.5f;
        else
            divingDir = Vector3.down + Vector3.left * 0.5f;
    }

    public void DivingEnd()
    {
        if (!isGoal)
            Goalkeeper_Hands.instance.DropBall();

        StartCoroutine(DivingEndDelay());  
    }


    IEnumerator DivingEndDelay()
    {
        yield return new WaitForSeconds(0.2f);

        if (isGoal)
            GoalkeeperAni.SetTrigger("Goal");
        else
            GoalkeeperAni.SetTrigger("NoGoal");
    }
}
