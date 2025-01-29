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

    public bool dribbleSlowStart, getTackled;
    bool start, isDribble, isJump;
    bool isWaitingForDoubleClick, dontMove;

    Vector3 direction;

    AnimatorStateInfo stateInfo;

    public void PlayerStart()
    {
        dribbleSlowStart = true;
        start = true;
    }
    public void DontMove()
    {
        dontMove = true;
        dribbleSlowStart = false;
        isJump = false;
        isWaitingForDoubleClick = false;
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

        if (!start || dontMove || getTackled || isJump || dribbleSlowStart) { Debug.LogError("버튼 블락"); return; }

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

    IEnumerator DoubleClickCheck()
    {
        // 더블클릭 처리
        if (isWaitingForDoubleClick)
        {
            Debug.Log("더블클릭!");
            isWaitingForDoubleClick = false; // 대기 상태 초기화

        }

        // 싱글 클릭 처리 대기
        else
        {
            isWaitingForDoubleClick = true;
            yield return new WaitForSecondsRealtime(0.2f);

            // 대기 중 더블클릭이 발생하지 않으면 싱글 클릭 처리
            if (isWaitingForDoubleClick)
            {
                isWaitingForDoubleClick = false; // 대기 상태 초기화
            }
            else
                yield break;
        }
    }

    // 점프
    public void Jump()
    {
        if (dontMove || getTackled || isJump || dribbleSlowStart) { Debug.LogError("버튼 블락"); return; }

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

        if (getTackled) return;
        getTackled = true;

        PlayerAni.SetTrigger(tackleName);

        Debug.LogError("태클 : " + tackleName);
    }

}
