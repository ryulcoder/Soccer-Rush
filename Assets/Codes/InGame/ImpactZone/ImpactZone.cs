using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactZone : MonoBehaviour
{
    public static ImpactZone Instance;
    public GoalPost GoalPost;

    [Header("[ Transform ]")]
    public Transform Ball;
    public Transform GoalPoint;
    public Transform CurvePoint;

    [Header("[ Vector ]")]
    [SerializeField] Vector3 startVec;
    [SerializeField] Vector3 curveVec;
    [SerializeField] Vector3 endVec;
    [SerializeField] Vector3 torqueDir;

    [Header("[ GameObject ]")]
    public GameObject Area;
    public GameObject Trigger;
    public GameObject[] Vfxs;

    Vector3 curveDefaultVec, endDefaultVec;

    public float duration = 1.2f;
    public bool right, win, goalEndMove;

    Rigidbody BallRgb;

    Vector3 dir = new(-1, 1, 0);

    float t = 0f;

    public bool impactKickOn;
    
    private void Awake()
    {
        Instance = this;

        BallRgb = Ball.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Area.SetActive(true);
        Trigger.SetActive(true);
    }

    void FixedUpdate()
    {
        if (impactKickOn)
        {
            if (t >= 1)
            {
                impactKickOn = false;
                goalEndMove = true;

                return;
            }
                

            t += Time.deltaTime / duration;

            // Quadratic Bezier 계산
            Vector3 a = Vector3.Lerp(startVec, curveVec, t);
            Vector3 b = Vector3.Lerp(curveVec, endVec, t);
            Ball.position = Vector3.Lerp(a, b, t);

            // 이동 방향에 맞게 회전
            BallRgb.AddTorque((1 - t) * 20 * torqueDir, ForceMode.VelocityChange);
        }

        if (goalEndMove)
        {
            if (!win)
            {
                goalEndMove = false;

                BallRgb.AddForce(Vector3.back * 4 , ForceMode.VelocityChange);
                BallRgb.AddTorque(Vector3.left * 3 , ForceMode.VelocityChange);
            }

            GoalEndMove();
        }

    }

    void GoalEndMove()
    {
        if (endVec.x < 0)
        {
            torqueDir = Vector3.left;

            if (Ball.position.x >= 0 && Ball.position.z <= Player.Instance.transform.position.z + 5)
            {
                BallRgb.velocity = Vector3.zero;
                BallRgb.angularVelocity = Vector3.zero;

                Ball.position = Vector3.up * 1.926f + Vector3.forward * (Player.Instance.transform.position.z + 5);

                goalEndMove = false;
                StartCoroutine(GoalDelay());
                return;
            }
            else
            {
                Ball.position = Vector3.right * Mathf.MoveTowards(Ball.position.x, 0, Time.deltaTime * 6)
                              + Vector3.up * Mathf.MoveTowards(Ball.position.y, 1.926f, Time.deltaTime * 3)
                              + Vector3.forward * Ball.position.z;
            }
        }

        else if (endVec.x > 0)
        {
            torqueDir = Vector3.left;

            if (Ball.position.x <= 0 && Ball.position.z <= Player.Instance.transform.position.z + 5)
            {
                BallRgb.velocity = Vector3.zero;
                BallRgb.angularVelocity = Vector3.zero;

                Ball.position = Vector3.up * 1.926f + Vector3.forward * (Player.Instance.transform.position.z + 5);

                goalEndMove = false;
                StartCoroutine(GoalDelay());
                return;
            }
            else
            {
                Ball.position = Vector3.right * Mathf.MoveTowards(Ball.position.x, 0, Time.deltaTime * 6)
                              + Vector3.up * Mathf.MoveTowards(Ball.position.y, 1.926f, Time.deltaTime * 3)
                              + Vector3.forward * Ball.position.z;
            }
        }

        if (Ball.position.y <= 1.93f)
        {
            BallRgb.AddTorque(torqueDir * 100, ForceMode.Force);

            BallRgb.AddForce(Vector3.back * 100, ForceMode.Force);
        }
    }

    void Reset()
    {
        duration = 1.2f;
        t = 0;

        curveDefaultVec = CurvePoint.position;
        endDefaultVec = GoalPoint.position;

        impactKickOn = false;
        goalEndMove = false;

        Goalkeeper.Instance.Reset();
        GoalPost.Reset();
    }

    public void ImpactKickOn()
    {
        Reset();

        startVec = Ball.position;
        curveVec = curveDefaultVec;
        endVec = endDefaultVec;

        if (right)
        {
            curveVec += 2 * curveVec.x * Vector3.left + 6 * Vector3.left;
            endVec += 2 * endVec.x * Vector3.left + 3 * Vector3.left;

            dir.x = 1;
        } 
        else
        {
            dir.x = -1;
        }
            
        if (!win)
        {
            duration = 1.3f;

            Vector3 copyVec = curveVec;

            curveVec *= 0.7f;
            curveVec.z = copyVec.z;

            copyVec = endVec;

            endVec *= 0.5f;
            endVec.z = copyVec.z;
        }

        torqueDir = Vector3.Cross(dir.normalized, Vector3.forward);

        impactKickOn = true;

        Goalkeeper.Instance.DivingStart();
    }

    public void NoGoal()
    {
        if (!impactKickOn) return;

        impactKickOn = false;

        Player.Instance.GoalCheck(false);

        StartCoroutine(NoGoalDelay());
    }

    IEnumerator NoGoalDelay()
    {
        yield return new WaitForSeconds(3);

        GameManager.Instance.impactFail = true;
    }

    IEnumerator GoalDelay()
    {
        BallMove.instance.ImpactEnd(true);
        GameManager.Instance.GameSpeedUp();

        yield return new WaitForSeconds(1.5f);

        Player.Instance.dribbleSlowStart = true;
        Player.Instance.GetComponent<Animator>().SetTrigger("ReDribble");
        Player.Instance.GetComponent<Animator>().SetTrigger("Dribble");

        Off();
    }

    public void DropBall()
    {
        Ball.SetParent(null, true);

        BallRgb.GetComponent<Collider>().isTrigger = false;
        BallRgb.useGravity = true;

        goalEndMove = true;
    }

    public void GoalAndDropBall()
    {
        impactKickOn = false;
        goalEndMove = true;

        Player.Instance.GoalCheck(true);
        Goalkeeper.Instance.isGoal = true;

        torqueDir = Vector3.Cross(-dir.normalized, Vector3.forward);

        for (int i = 0; i < Vfxs.Length; i++)
        {
            Vfxs[i].SetActive(true);
        }
    }



    public void Off()
    {
        Area.SetActive(false);
        Trigger.SetActive(false);

        StartCoroutine(OffZone());   
    }

    IEnumerator OffZone()
    {
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < Vfxs.Length; i++)
        {
            Vfxs[i].SetActive(false);
        }

        gameObject.SetActive(false);
    }

    
}
