using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    bool isDribble, isJump, isMove;
    bool isWaitingForDoubleClick, getTackled, dontMove;

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
        isMove = false;
        isWaitingForDoubleClick = false;
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
        // �÷��̾� �ִϸ����� ���� �ִϸ��̼� Ȯ��
        stateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0);

        if (getTackled)
        {
            if (stateInfo.IsName("Fallen") || stateInfo.IsName("GetStandTackled_Front"))
            {
                getTackled = false;
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

            // �� �� ������ update
            if (next_x != position.x)
            {
                // ������ġ ���� �� ��ž �� �ʱ�ȭ
                if (direction.x > 0 && position.x >= next_x - 0.1f || direction.x < 0 && position.x <= next_x + 0.1f)
                {
                    isMove = false;

                    PlayerTransform.position = new(next_x, position.y, position.z);

                    PlayerAni.SetBool("MoveLeft", false);
                    PlayerAni.SetBool("MoveRight", false);

                    prev_x = next_x;
                }
                else
                {
                    PlayerTransform.position += 22 * Time.deltaTime * direction;

                }
            }

            // �帮����
            if (isDribble)
            {
                if (stateInfo.IsName("Start_Run") || stateInfo.IsName("Dribble") || stateInfo.IsName("Jump_Run")
                    || stateInfo.IsName("Move_Left") || stateInfo.IsName("Move_Right"))
                {
                    // ó�� �帮�� �� õõ�� �ӵ� �ø���
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

                    // �帮�� �̵�
                    PlayerTransform.position += Vector3.forward * totalSpeed;
                }


               
            }
        }
    }

    // �� �� �̵�
    public void MoveLeftRight(int moveDirection)
    {
        if (PlayerTransform.position.x > 0 && moveDirection > 0 || PlayerTransform.position.x < 0 && moveDirection < 0) return;

        if (!start || dontMove || isJump || dribbleSlowStart) { Debug.LogError("��ư ���"); return; }

        direction = new(moveDirection, 0, 0);

        next_x += moveDirection * distance;

        // ����Ŭ�� üũ
        StartCoroutine(DoubleClickCheck());

        if (moveDirection > 0)
        {
            PlayerAni.SetBool("MoveRight", true);
            PlayerAni.SetBool("MoveLeft", false);
        }
        else
        {
            PlayerAni.SetBool("MoveLeft", true);
            PlayerAni.SetBool("MoveRight", false);
        }

        if (Mathf.Abs(next_x) >= distance)
        {
            next_x = moveDirection * distance;
        }

        isMove = true;

    }

    IEnumerator DoubleClickCheck()
    {
        // ����Ŭ�� ó��
        if (isWaitingForDoubleClick)
        {
            Debug.Log("����Ŭ��!");
            isWaitingForDoubleClick = false; // ��� ���� �ʱ�ȭ

        }

        // �̱� Ŭ�� ó�� ���
        else
        {
            isWaitingForDoubleClick = true;
            yield return new WaitForSecondsRealtime(0.2f);

            // ��� �� ����Ŭ���� �߻����� ������ �̱� Ŭ�� ó��
            if (isWaitingForDoubleClick)
            {
                isWaitingForDoubleClick = false; // ��� ���� �ʱ�ȭ
            }
            else
                yield break;
        }
    }

    // ����
    public void Jump()
    {
        if (dontMove || isJump || dribbleSlowStart) { Debug.LogError("��ư ���"); return; }

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
        if ((tackleName == "GetTackled_Right" || tackleName == "GetTackled_Left") && isJump && !dontMove) return;
        
        getTackled = true;
        PlayerAni.SetTrigger(tackleName);
    }

}
