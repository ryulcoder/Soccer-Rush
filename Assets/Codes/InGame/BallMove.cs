using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Player Player;

    [Header("[ Player ]")]
    public Transform PlayerTrans;
    public Animator PlayerAni;

    [Header("[ Ball ]")]
    public Transform BallTrans;
    public Rigidbody BallRigibody;

    [Space]
    public float speed; 

    float dampingFactor = 0.98f;
    float ballMaxY, fallSpeed;

    bool deceleration, kick, moveKick, flick, flickBallDown, ballVelLimit;
    static bool kickDelay;

    Vector3 movement = Vector3.forward;
    Vector3 torqueDir = Vector3.right;

    Vector3 playerVec, ballVec, moveTorqueDir;

    void Start()
    {
        BallRigibody.maxAngularVelocity = 20;
    }

    void FixedUpdate()
    {
        // 볼 이동, 회전 감속 로직 적용
        if (deceleration)
        {
            BallRigibody.angularVelocity *= dampingFactor;
            BallRigibody.velocity *= dampingFactor;

        }

        // 볼을 찼을 시
        if (kick)
        {
            // 공이 이탈 시 위치 조정
            if (BallTrans.position.z < PlayerTrans.position.z + 3)
                BallTrans.position = new Vector3(0, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

            playerVec = PlayerTrans.position;
            ballVec = BallTrans.position;

            // 좌/우 로 플레이어가 움직일 시 플레이어 따라 볼 이동 혹은 회전
            if (ballVec.x != playerVec.x)
            {
                if (!moveKick)
                {
                    if (ballVec.x < playerVec.x)
                        moveTorqueDir = new Vector3(1, 1, 0);
                    else if (ballVec.x > playerVec.x)
                        moveTorqueDir = new Vector3(1, -1, 0);
                    else
                        moveTorqueDir = Vector3.right;
                    

                    BallRigibody.AddTorque(moveTorqueDir * 100, ForceMode.VelocityChange);

                    moveKick = true;
                }
                    
                BallTrans.position = new(playerVec.x, ballVec.y, ballVec.z);
            }
            else
            {
                moveKick = false;
            }

            // 공 띄우기 
            if (flick)
            {
                BallTrans.position += Vector3.forward; // 플레이어 이동 속도와 같이
                BallRigibody.AddForce(Vector3.down * 55, ForceMode.Acceleration); // 볼 떨어지는 가중력
                BallRigibody.AddTorque(torqueDir, ForceMode.Acceleration);

                // 포물선 특정 높이에서 떨어져야 할때
                if (flickBallDown)
                {
                    // 볼이 땅에 닿았을때
                    if (BallTrans.position.y <= 1.52f)
                    {
                        BallRigibody.velocity = Vector3.zero;
                        BallTrans.position = new Vector3(BallTrans.position.x, 1.52f, BallTrans.position.z);

                        BallRigibody.AddForce(movement * 10, ForceMode.VelocityChange);
                        BallRigibody.AddTorque(torqueDir * 100, ForceMode.VelocityChange);

                        flickBallDown = false;
                        flick = false;
                        kickDelay = false;

                        fallSpeed = 0;
                    }

                    return;
                }

                // 최고 높이 확인
                if (BallTrans.position.y > ballMaxY)
                {
                    ballMaxY = BallTrans.position.y;

                    if (!ballVelLimit && BallRigibody.velocity.y > 4)
                        ballVelLimit = true;
                    if (ballVelLimit && BallRigibody.velocity.y <= 4)
                    {
                        ballVelLimit = false;
                        ballMaxY = 1000;
                    }
                }
                else
                {
                    flickBallDown = true;
                }
            }

        }

       

    }

    // 플레이어 점프 시 볼 띄우기
    public void Flick()
    {
        if (flick) return;

        flick = true;
        kickDelay = true;
        ballMaxY = 0;

        BallRigibody.velocity = Vector3.zero;
        Reset();

        BallRigibody.AddForce(new(0, 50, 10), ForceMode.VelocityChange);
        BallRigibody.AddTorque(torqueDir * 100, ForceMode.VelocityChange);
    }

    // 볼 위치 리셋
    public void Reset()
    {
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.52f, PlayerTrans.position.z + 4);
    }

    // 볼 킥 여러번 트리거 방지 딜레이
    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.2f);

        kickDelay = false;
    }


    
    void OnTriggerStay(Collider collider)
    {
        // 플레이어 발 트리거로 인한 볼 이동 혹은 회전
        if (collider.gameObject.name == "PlayerFoot")
        {
            if (kickDelay) return;

            BallRigibody.velocity = Vector3.zero;

            deceleration = true;

            // 볼 리지바디 이동, 회전 힘 작용
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(torqueDir * 100, ForceMode.VelocityChange);


            // 볼 킥 딜레이
            kickDelay = true;

            StartCoroutine(KickDelay());

            kick = true;

            return;
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Defender")
        {
            Vector3 direction = (collision.transform.position - collision.contacts[0].point).normalized;

            BallRigibody.AddForce(direction * speed, ForceMode.Impulse);
            BallRigibody.AddTorque(direction * speed, ForceMode.Impulse);
        }
    }

}
