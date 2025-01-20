using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool isDribble, isJump, isMove;
    bool ballKick, isWaitingForDoubleClick, getFrontTackle;

    Vector3 direction;

    AnimatorStateInfo stateInfo;

    public void PlayerStart()
    {
        dribbleSlowStart = true;
        start = true;
    }
    public void DontMove()
    {
        dribbleSlowStart = false;
        isJump = false;
        isMove = false;
        ballKick = false;
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

        if (getFrontTackle)
        {
            if (stateInfo.IsName("Dribble") || stateInfo.IsName("GetTackled_Front") || stateInfo.IsName("GetStandTackled_Front"))
            {
                PlayerTransform.position += Vector3.forward * totalSpeed;
            }
            else if (stateInfo.IsName("Fallen"))
            {
                getFrontTackle = false;
                start = false;
                isDribble = false;

                totalSpeed = 0;
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
        if (start) 
        { 
            if (isJump || dribbleSlowStart) { Debug.LogError("��ư ���"); return; }

            direction = new(moveDirection, 0, 0);

            next_x += direction.x * distance;

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
        if (isJump || dribbleSlowStart) { Debug.LogError("��ư ���"); return; }

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
        PlayerAni.SetTrigger(tackleName);
    }


    private void OnTriggerEnter(Collider collider)
    {
        // �� �ݶ��̴� Ʈ���� ��
        if (collider.gameObject.name == "Ball")
        {
            ballKick = true;
            //if (isJump) isJump = false;
        }

        if (collider.gameObject.name == "SlidingTakle_Defender")
        {
            // ��Ŭ�� ������ �� ������
            if (isJump)
            {
                Debug.Log("������ ����");
                return;
            }

            PlayerAni.SetTrigger("GetTackled_Front");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // �� �ݶ��̴��� �־��� ��
        if (collider.gameObject.name == "Ball")
        {
            ballKick = false;

        }
    }

   

}
