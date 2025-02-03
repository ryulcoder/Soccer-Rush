using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public BallMove BallMove;

    public Transform PlayerTransform;
    public Rigidbody PlayerRigibody;
    public Animator PlayerAni;
    public Transform Armature;

    public Transform TileTransform;

    public float speed, jumpSpeed;
    float distance, totalSpeed, prev_x, next_x;

    public bool dribbleSlowStart, getTackled, spinRight;
    public bool start, dontMove, isDribble, isJump, isSpin;

    Vector3 direction;

    AnimatorStateInfo stateInfo;

    public void PlayerStart()
    {
        dribbleSlowStart = true;
        start = true;
    }
    public void DontMove()
    {
        Debug.LogWarning("볼이 수비에 걸림");

        dontMove = true;
        dribbleSlowStart = false;
        isJump = false;

    }

    public void Start()
    {
        isJump = true;

        distance = TileTransform.localScale.x / 3;

        prev_x = PlayerRigibody.position.x;

        PlayerAni.SetTrigger("WaitRun");

    }

    void Update()
    {
        if (start)
        {
            if (!isDribble)
            {
                isDribble = true;
                PlayerAni.SetTrigger("Dribble");
            }

        }

    }


    void FixedUpdate()
    {
        // 플레이어 애니메이터 현재 애니메이션 확인
        stateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0);

        // 태클을 당했을시 특정 모션전 감속 로직
        if (getTackled)
        {
            if (stateInfo.IsName("Fallen") || stateInfo.IsName("GetStandTackled_Front"))
            {
                start = false;
                isDribble = false;

                totalSpeed = 0;
            }
            else
            {
                PlayerTransform.position += Vector3.forward * totalSpeed;
                totalSpeed *= 0.95f;
            }
        }



        if (start)
        {
            Vector3 position = PlayerTransform.position;

            // 좌 우 움직임 로직
            if (next_x != position.x)
            {
                // 도착위치 도달 시 스탑 후 초기화
                if (direction.x > 0 && position.x >= next_x - 1 || direction.x < 0 && position.x <= next_x + 1)
                {
                    PlayerTransform.position = new(next_x, position.y, position.z);

                    PlayerAni.SetTrigger("ReDribble");

                    prev_x = next_x;
                }
                else
                {
                    PlayerTransform.position += 22 * Time.deltaTime * direction;

                }
            }

            // 드리블중
            if (isDribble)
            {
                if (stateInfo.IsName("Start_Run") || stateInfo.IsName("Dribble") || stateInfo.IsName("Jump_Run") 
                    || stateInfo.IsName("Spin_Left") || stateInfo.IsName("Spin_Right")
                    || stateInfo.IsName("Move_Left") || stateInfo.IsName("Move_Right"))
                {
                    // 처음 드리블 시 천천히 속도 올리기
                    if (dribbleSlowStart)
                    {
                        totalSpeed += Time.deltaTime;

                        if (totalSpeed >= speed)
                        {
                            dribbleSlowStart = false;
                            isJump = false;
                            totalSpeed = speed;
                        }
                    }

                    // 드리블 이동
                    PlayerTransform.position += Vector3.forward * totalSpeed;
                }
            }
        }
    }

    // 좌 우 이동
    public void MoveLeftRight(int moveDirection)
    {
        if ((moveDirection > 0 && PlayerTransform.position.x >= distance) || (moveDirection < 0 && PlayerTransform.position.x <= -distance)) return;

        if (!start || dontMove || getTackled || isSpin || isJump || dribbleSlowStart) { Debug.LogWarning("Block"); return; }

        direction = new(moveDirection, 0, 0);

        next_x += moveDirection * distance;

        if (moveDirection > 0)
            PlayerAni.SetTrigger("MoveRight");
        else
            PlayerAni.SetTrigger("MoveLeft");
        

        if (Mathf.Abs(next_x) >= distance)
        {
            next_x = moveDirection * distance;
        }


    }

    // 개인기
    public void Spin()
    {
        if (dontMove || getTackled || isSpin || isJump || dribbleSlowStart) { Debug.LogWarning("Block"); return; }

        isSpin = true;

        if (!spinRight)
        { 
            PlayerAni.SetTrigger("Spin_Left");
            BallMove.SpinMove("Left");
            
        }
        else
        {
            PlayerAni.SetTrigger("Spin_Right");
            BallMove.SpinMove("Right");
        }
    }
    public void SpinEnd()
    {
        isSpin = false;
    }

    // 점프
    public void Jump()
    {
        if (dontMove || getTackled || isSpin || isJump || dribbleSlowStart) { Debug.LogWarning("Block"); return; }

        isJump = true;

        PlayerAni.SetTrigger("Jump");

        BallMove.Flick();
    }

    public void JumpEnd()
    {
        isJump = false;
    }

    public void GetTackled(string tackleName)
    {
        if ((tackleName == "GetTackled_Right" || tackleName == "GetTackled_Left" || tackleName == "GetTackled_Front") && isJump && !dontMove)
        {
            Debug.LogWarning("점프로 태클 피함!!");
            return;
        }
        
        if(tackleName == "GetStandTackled_Front" && isSpin && !dontMove)
        {
            Debug.LogWarning("마르세유로 태클 피함!!");
            return;
        }


        if (getTackled) return;
        getTackled = true;

        PlayerAni.SetTrigger(tackleName);

        Debug.LogWarning("태클 : " + tackleName);
    }

}
