using System.Collections;
using System.Collections.Generic;
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
    public Animator BallAni;

    [Space]
    public float speed; 

    static bool kickDelay;
    float dampingFactor = 0.98f;
    public bool deceleration, kick, isTackled;
    
    // ballMove
    float ballMaxY;
    bool moveKick;

    // spinMove
    float next_x;
    public bool spin, spin_move, spin_revert;
    public bool spin_right;

    // flick
    bool flick, flickBallDown, ballVelLimit;


    Vector3 movement = Vector3.forward;
    Vector3 torqueDir;

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
            if (Player.dribbleSlowStart)
            {
                BallRigibody.velocity = new Vector3(BallRigibody.velocity.x, BallRigibody.velocity.y, 35);
            }

            if (flick && BallRigibody.velocity.z > 10)
            {
                BallRigibody.velocity = new Vector3(BallRigibody.velocity.x, BallRigibody.velocity.y, 9);
            }

            BallRigibody.angularVelocity *= dampingFactor;
            BallRigibody.velocity *= dampingFactor;

            //Debug.Log(BallRigibody.velocity);
        }

        // 볼을 찼을 시
        if (kick)
        {
            // 공이 이탈 시 위치 조정
            if (!isTackled && !spin && BallTrans.position.z < PlayerTrans.position.z + 3)
            {
                Debug.LogWarning("pos: " + BallTrans.position + " 볼 위치 조정");
                BallTrans.position = new Vector3(0, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

                BallRigibody.velocity = Vector3.zero;
                BallRigibody.AddForce(1.2f * speed * movement, ForceMode.VelocityChange);
                BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
            }


            playerVec = PlayerTrans.position;
            ballVec = BallTrans.position;

            // 좌/우 로 플레이어가 움직일 시 플레이어 따라 볼 이동 혹은 회전
            if (!spin && ballVec.x != playerVec.x)
            {
                if (!moveKick)
                {
                    if (ballVec.x < playerVec.x)
                        moveTorqueDir = new Vector3(1, 1, 0);
                    else if (ballVec.x > playerVec.x)
                        moveTorqueDir = new Vector3(1, -1, 0);
                    else
                        moveTorqueDir = Vector3.right;
                    

                    BallRigibody.AddTorque(moveTorqueDir * 90, ForceMode.VelocityChange);

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
                BallTrans.position += Vector3.forward * 0.9f; // 플레이어 이동 속도와 같이
                BallRigibody.AddForce(Vector3.down * 100, ForceMode.Acceleration); // 볼 떨어지는 가중력
                BallRigibody.AddTorque(Vector3.right, ForceMode.Acceleration);

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

    private void LateUpdate()
    {
        // 애니메이션이 끝났을 때 부모 해제
        if (spin && !BallAni.isActiveAndEnabled)
        {
            spin = false;
            Player.SpinEnd();

            BallTrans.SetParent(null); 

            kickDelay = false;

            BallTrans.position = new Vector3(0, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.AddForce(1.2f * speed * movement, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
        }
    }

    // 볼 위치 리셋
    public void Reset()
    {
        BallTrans.GetComponent<Collider>().isTrigger = false;
        BallRigibody.constraints |= RigidbodyConstraints.FreezePositionX;

        BallRigibody.velocity = Vector3.zero;
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.926f, PlayerTrans.position.z + 4.5f);
    }

    // 플레이어 스핀개인기 시 볼 움직임
    public void SpinMove(string dir)
    {
        if (spin) return;

        spin = true;
        kickDelay = true;

        BallAni.Rebind();
        BallTrans.SetParent(PlayerTrans);

        BallAni.enabled = true;

        if (dir == "Left")
        {
            BallAni.SetTrigger("Spin_Left");
            //torqueDir = new Vector3(1, -1, 0);
            //next_x = PlayerTrans.position.x - 3;
        }
        else
        {
            BallAni.SetTrigger("Spin_Right");
            //BallAni.SetTrigger("BallSpin_Right");
            //spin_right = true;
            //torqueDir = new Vector3(1, 1, 0);
            //next_x = PlayerTrans.position.x + 3;
        }
    }
    public void SpinMoveEnd()
    {
        BallAni.enabled = false;
    }

    // 플레이어 점프 시 볼 띄우기
    public void Flick()
    {
        if (flick) return;

        flick = true;
        kickDelay = true;
        ballMaxY = 0;
        

        BallRigibody.velocity = Vector3.zero;
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.926f, PlayerTrans.position.z + 4.5f);

        BallRigibody.AddForce(new(0, 55, 9), ForceMode.VelocityChange);
        BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
    }

    // 볼 킥 여러번 트리거 방지 딜레이
    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.2f);

        kickDelay = false;
    }
    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
    }


    void OnTriggerEnter(Collider collider)
    {
        // 태클을 당했을 때
        if (!isTackled && collider.gameObject.name == "TackleFoot")
        {
            Defender defender = collider.GetComponent<DefenderFootTrigger>().Defender;

            // 좌우 슬라이딩 태클 회피
            if ((defender.anomalyUserState == "GetTackled_Right" || defender.anomalyUserState == "GetTackled_Left") && flick) 
            {
                //StartCoroutine(Delay(0.1f));
                return;
            }

            // 정면 스탠드 태클 회피
            if((defender.currentState.ToString() == "Stand_Tackle_Front" || defender.anomalyUserState == "GetStandTackled_Front") && spin)
            {
                //StartCoroutine(Delay(0.1f));
                return;
            }

            isTackled = true;
            kick = false;

            if (BallAni.isActiveAndEnabled == true)
            {
                SpinMoveEnd();
                Debug.LogWarning("볼 애니 스탑");
            }

            

            Player.DontMove();

            BallTrans.GetComponent<Collider>().isTrigger = true;
            BallRigibody.constraints &= ~RigidbodyConstraints.FreezePositionX;

            BallRigibody.velocity = Vector3.zero;

            Vector3 direction = new Vector3(BallTrans.position.x - collider.transform.position.x, 1, BallTrans.position.z - collider.transform.position.z);

            BallRigibody.AddForce(direction * 100, ForceMode.VelocityChange);
            BallRigibody.AddTorque(direction * 90, ForceMode.VelocityChange);

            Debug.LogWarning("수비 커트");

            // 볼만 태클 당했을 시 플레이어 강제 모션 
            string stateName = defender.currentState.ToString();

            switch (stateName)
            {
                case "Stand_Tackle_Front":
                    stateName = "GetStandTackled_Front";
                    break;
                case "Sliding_Tackle_Front":
                    stateName = "GetTackled_Front";
                    break;
                case "Sliding_Tackle_Anomaly":
                    stateName = defender.anomalyUserState;
                    break;
            }

            StartCoroutine(Delay(0.2f));

            Player.GetTackled(stateName);
        }

        // 플레이어 발 트리거로 인한 볼 이동 혹은 회전
        if (!spin && !flick && !kickDelay && !isTackled && collider.gameObject.name == "PlayerFoot")
        {
            BallRigibody.velocity = Vector3.zero;

            deceleration = true;

            // 볼 리지바디 이동, 회전 힘 작용
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);


            // 볼 킥 딜레이
            kickDelay = true;

            StartCoroutine(KickDelay());

            kick = true;

            return;
        }

       


    }


    private void OnCollisionEnter(Collision collision)
    {
        // 볼이 땅에 닿았을때
        if (collision.gameObject.CompareTag("Floor"))
        {
            // 포물선 특정 높이에서 떨어져야 할때
            if (flickBallDown)
            {
                Player.JumpEnd();

                BallRigibody.velocity = new(BallRigibody.velocity.x, 0, BallRigibody.velocity.z);

                BallRigibody.AddForce(movement * speed / 2, ForceMode.VelocityChange);
                BallRigibody.AddTorque(Vector3.right * 10, ForceMode.VelocityChange);

                flickBallDown = false;
                flick = false;
                kickDelay = false;

                return;
            }
        }

    }

}
