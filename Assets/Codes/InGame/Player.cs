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
            // �� �� ������ update
            if (isMove)
            {
                Vector3 position = PlayerTransform.position;
                
                // ������ġ ���� �� ��ž �� �ʱ�ȭ
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

            // �÷��̾� �ִϸ����� ���� �ִϸ��̼� Ȯ��
            stateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0);

            // �帮����
            if (isDribble)
            {
                if (stateInfo.IsName("Start_Run") || stateInfo.IsName("Dribble_Tree") || stateInfo.IsName("Jump_Run"))
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
            if (isJump) return;

            direction = new(moveDirection, 0, 0);

            next_x += direction.x * distance;

            // ����Ŭ�� üũ
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
                Debug.Log("�̱�Ŭ��!");
                isWaitingForDoubleClick = false; // ��� ���� �ʱ�ȭ
            }
            else
                yield break;
        }
    }

    // ����
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
        // �� �ݶ��̴� Ʈ���� ��
        if (collider.gameObject.name == "Ball")
        {
            ballKick = true;
            if (isJump) isJump = false;
        }

        // ��Ŭ�� ������ ��
        if (collider.gameObject.name == "Takle_Defender")
        {
            PlayerAni.SetTrigger("GetTackled_Front");
            totalSpeed = 0;
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
            totalSpeed = 0;
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
