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
        Debug.LogWarning("���� ���� �ɸ�");

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
        // �÷��̾� �ִϸ����� ���� �ִϸ��̼� Ȯ��
        stateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0);

        // ��Ŭ�� �������� Ư�� ����� ���� ����
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

            // �� �� ������ ����
            if (next_x != position.x)
            {
                // ������ġ ���� �� ��ž �� �ʱ�ȭ
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

            // �帮����
            if (isDribble)
            {
                if (stateInfo.IsName("Start_Run") || stateInfo.IsName("Dribble") || stateInfo.IsName("Jump_Run") 
                    || stateInfo.IsName("Spin_Left") || stateInfo.IsName("Spin_Right")
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

    // ���α�
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

    // ����
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
            Debug.LogWarning("������ ��Ŭ ����!!");
            return;
        }
        
        if(tackleName == "GetStandTackled_Front" && isSpin && !dontMove)
        {
            Debug.LogWarning("���������� ��Ŭ ����!!");
            return;
        }


        if (getTackled) return;
        getTackled = true;

        PlayerAni.SetTrigger(tackleName);

        Debug.LogWarning("��Ŭ : " + tackleName);
    }

}
