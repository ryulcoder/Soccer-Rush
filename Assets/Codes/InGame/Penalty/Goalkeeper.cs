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
            if (!nextY && GoalKeeperTrans.localPosition.y >= GoalPoint.localPosition.y)
                nextY = true;
            
            if (nextY && GoalKeeperTrans.localPosition.y >= 1)
            {
                jump = false;
            }
            else if (nextY)
            {
                GoalKeeperTrans.localPosition += divingSpeed * Time.deltaTime * Vector3.down;
                divingSpeed += Time.deltaTime * 5;
            }
            else
            {
                GoalKeeperTrans.localPosition += divingSpeed * Time.deltaTime * divingDir;
                divingSpeed += Time.deltaTime * 5;
            }
                
        }



        if (fall && GoalKeeperTrans.localPosition.y <= 0)
        {
            fall = false;

            GoalKeeperTrans.localPosition = new(GoalKeeperTrans.localPosition.x, 0,GoalKeeperTrans.localPosition.z);
        }
        else if (fall)
        {
            GoalKeeperTrans.localPosition += divingSpeed * Time.deltaTime * Vector3.down;
            divingSpeed += Time.deltaTime * 4;
        }


    }

    public void Reset()
    {
        right = false;
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
        nextY = false;
    }
    public void DivingFall()
    {
        divingSpeed = 20;
        nextY = true;

        Debug.LogWarning("Fall");
    }

    public void Fallen()
    {
        jump = false;
        fall = true;

        Debug.LogWarning("Fallen");
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
