using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penalty : MonoBehaviour
{
    public static Penalty Instance;

    [Header("[ Transform ]")]
    public Transform Ball;
    public Transform GoalPoint;
    public Transform CurvePoint;

    [Header("[ Vector ]")]
    [SerializeField] Vector3 startVec;
    [SerializeField] Vector3 curveVec;
    [SerializeField] Vector3 endVec;
    [SerializeField] Vector3 torqueDir;

    public float duration = 1.2f;
    public bool right, goalEndMove;

    Rigidbody BallRgb;

    Vector3 dir = new(-1, 1, 0);

    float t = 0f;

    bool penalKickOn;
    
    private void Awake()
    {
        Instance = this;

        if (right)
        {
            CurvePoint.position += 2 * CurvePoint.position.x * Vector3.left + 6 * Vector3.left;
            GoalPoint.position += 2 * GoalPoint.position.x * Vector3.left + 3 * Vector3.left;
        }

        startVec = Ball.position;
        curveVec = CurvePoint.position;
        endVec = GoalPoint.position;

        BallRgb = Ball.GetComponent<Rigidbody>();
        BallRgb.maxAngularVelocity = 20; // 테스트만
    }

    void Start()
    {
        Reset();

        if (!TestPlayer.Instance.on)
            PenalKickOn();
    }

    void Reset()
    {
        duration = 1.2f;

        t = 0;

        penalKickOn = false;
        goalEndMove = false;
        
        Goalkeeper.Instance.Reset();
    }

    void FixedUpdate()
    {
        if (penalKickOn)
        {
            if (t >= 1)
            {
                penalKickOn = false;
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
            GoalEndMove();
        }

    }

    void GoalEndMove()
    {
        if (endVec.x < 0)
        {
            if (Ball.position.x >= 0)
            {
                Ball.position = new Vector3(0, 1.926f, Ball.position.z);

                BallRgb.velocity = Vector3.zero;
                BallRgb.angularVelocity = Vector3.zero;

                goalEndMove = false;
                return;
            }
            else
            {
                Ball.position = new Vector3(Mathf.MoveTowards(Ball.position.x, 0, Time.deltaTime * 6), Mathf.MoveTowards(Ball.position.y, 1.926f, Time.deltaTime * 3), Ball.position.z);
            }
        }

        else if (endVec.x > 0)
        {
            if (Ball.position.x <= 0)
            {
                Ball.position = new Vector3(0, 1.926f, Ball.position.z);

                BallRgb.velocity = Vector3.zero;
                BallRgb.angularVelocity = Vector3.zero;

                goalEndMove = false;
                return;
            }
            else
            {
                Ball.position = new Vector3(Mathf.MoveTowards(Ball.position.x, 0, Time.deltaTime * 6), Mathf.MoveTowards(Ball.position.y, 1.926f, Time.deltaTime * 3), Ball.position.z);
            }
        }

        if (Ball.position.y <= 1.93f)
        {
            BallRgb.AddTorque(torqueDir * 5, ForceMode.Force);

            BallRgb.AddForce(Vector3.back * 5, ForceMode.Acceleration);
        }
    }

    public void PenalKickOn()
    {
        startVec = Ball.position;
        endVec = GoalPoint.position;
        curveVec = CurvePoint.position;

        if (right)
            dir.x = 1;
        else
            dir.x = -1;

        torqueDir = Vector3.Cross(dir.normalized, Vector3.forward);

        penalKickOn = true;

        Goalkeeper.Instance.DivingStart();
    }

    public void NoGoal()
    {
        if (!penalKickOn) return;

        penalKickOn = false;

        TestPlayer.Instance.GoalCheck(false);

        // GameOver
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball") && penalKickOn)
        {
            penalKickOn = false;
            goalEndMove = true;

            TestPlayer.Instance.GoalCheck(true);
            Goalkeeper.Instance.isGoal = true;

            torqueDir = Vector3.Cross(-dir.normalized, Vector3.forward);

            Debug.LogWarning("Goal");
        }
    }
}
