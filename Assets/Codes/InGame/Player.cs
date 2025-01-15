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

    bool start;
    public bool dribbleSlowStart;
    public bool isDribble, isJump, isMove;
    bool ballKick, isWaitingForDoubleClick;

    Vector3 direction;

    AnimatorStateInfo stateInfo;

    public void PlayerStart()
    {
        dribbleSlowStart = true;
        start = true;
    }

    public void Start()
    {
        isJump = true; isMove = true;

        distance = TileTransform.localScale.y / 3;

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
        if (start)
        {
            // 좌 우 움직임 update
            if (isMove)
            {
                Vector3 position = PlayerTransform.position;
                
                // 도착위치 도달 시 스탑 후 초기화
                if (direction.x > 0 && position.x >= next_x - 0.1f || direction.x < 0 && position.x <= next_x + 0.1f)
                {
                    isMove = false;

                    PlayerTransform.position = new(next_x, position.y, position.z);

                    PlayerAni.SetFloat("Horizontal", 0);

                    prev_x = next_x;
                }
                else
                {
                    PlayerTransform.position += 22 * Time.deltaTime * direction;

                    float moveDis = Mathf.Abs(next_x - prev_x);

                    if (direction.x > 0)
                        PlayerAni.SetFloat("Horizontal", next_x - position.x < moveDis / 2 ? next_x - position.x : moveDis - (next_x - position.x));
                    else
                        PlayerAni.SetFloat("Horizontal", position.x - next_x < moveDis / 2 ? next_x - position.x : -moveDis + position.x - next_x);
                }
            }

            // 플레이어 애니메이터 현재 애니메이션 확인
            stateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0);

            // 드리블중
            if (isDribble)
            {
                if (stateInfo.IsName("Start_Run") || stateInfo.IsName("Dribble_Tree") || stateInfo.IsName("Jump_Run"))
                {
                    // 처음 드리블 시 천천히 속도 올리기
                    if (dribbleSlowStart)
                    {
                        totalSpeed += Time.deltaTime;

                        if (totalSpeed >= speed)
                        {
                            dribbleSlowStart = false;
                            isJump = false;
                            isMove = false;

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
        if (start) 
        { 
            if (isJump) return;

            direction = new(moveDirection, 0, 0);

            next_x += direction.x * distance;

            // 더블클릭 체크
            StartCoroutine(DoubleClickCheck());

            if (Mathf.Abs(next_x) >= distance)
            {
                next_x = moveDirection * distance;
            }

            isMove = true;
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
                Debug.Log("싱글클릭!");
                isWaitingForDoubleClick = false; // 대기 상태 초기화
            }
            else
                yield break;
        }
    }

    // 점프
    public void Jump()
    {
        if (isJump || isMove) return;

        isJump = true;

        PlayerAni.SetTrigger("Jump");

        BallMove.Flick();
    }

    public void JumpEnd()
    {
        isJump = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // 볼 콜라이더 트리거 시
        if (collider.gameObject.name == "Ball")
        {
            ballKick = true;
            if (isJump) isJump = false;
        }

        // 태클을 당했을 시
        if (collider.gameObject.name == "Takle_Defender")
        {
            PlayerAni.SetTrigger("GetTackled_Front");
            totalSpeed = 0;
        }

        if (collider.gameObject.name == "SlidingTakle_Defender")
        {
            // 태클을 당했을 때 점프시
            if (isJump)
            {
                Debug.Log("점프중 무적");
                return;
            }

            PlayerAni.SetTrigger("GetTackled_Front");
            totalSpeed = 0;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // 볼 콜라이더랑 멀어질 시
        if (collider.gameObject.name == "Ball")
        {
            ballKick = false;

        }
    }

}
