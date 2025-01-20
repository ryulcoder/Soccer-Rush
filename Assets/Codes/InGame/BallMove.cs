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

    [Space]
    public float speed; 

    float dampingFactor = 0.98f;
    float ballMaxY, fallSpeed;

    bool deceleration, kick, moveKick, flick, flickBallDown, ballVelLimit, isTackled;
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
        // �� �̵�, ȸ�� ���� ���� ����
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

        // ���� á�� ��
        if (kick)
        {
            // ���� ��Ż �� ��ġ ����
            if (!isTackled && BallTrans.position.z < PlayerTrans.position.z + 3)
                BallTrans.position = new Vector3(0, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

            playerVec = PlayerTrans.position;
            ballVec = BallTrans.position;

            // ��/�� �� �÷��̾ ������ �� �÷��̾� ���� �� �̵� Ȥ�� ȸ��
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

            // �� ���� 
            if (flick)
            {
                BallTrans.position += Vector3.forward * 0.9f; // �÷��̾� �̵� �ӵ��� ����
                BallRigibody.AddForce(Vector3.down * 100, ForceMode.Acceleration); // �� �������� ���߷�
                BallRigibody.AddTorque(torqueDir, ForceMode.Acceleration);

                // �ְ� ���� Ȯ��
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

    // �� ��ġ ����
    public void Reset()
    {
        BallTrans.GetComponent<Collider>().isTrigger = false;
        BallRigibody.constraints |= RigidbodyConstraints.FreezePositionX;

        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.52f, PlayerTrans.position.z + 4.5f);
    }


    // �÷��̾� ���� �� �� ����
    public void Flick()
    {
        if (flick) return;

        flick = true;
        kickDelay = true;
        ballMaxY = 0;

        BallRigibody.velocity = Vector3.zero;
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.52f, PlayerTrans.position.z + 4.5f);

        BallRigibody.AddForce(new(0, 55, 9), ForceMode.VelocityChange);
        BallRigibody.AddTorque(torqueDir * 100, ForceMode.VelocityChange);
    }

    // �� ű ������ Ʈ���� ���� ������
    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.2f);

        kickDelay = false;
    }

    
    void OnTriggerEnter(Collider collider)
    {
        // ���� ��Ŭ�� ������ ��
        if (!isTackled && collider.gameObject.name == "TackleFoot")
        {
            kick = false;
            isTackled = true;

            Player.DontMove();

            BallTrans.GetComponent<Collider>().isTrigger = true;
            BallRigibody.constraints &= ~RigidbodyConstraints.FreezePositionX;

            BallRigibody.velocity = Vector3.zero;

            Vector3 direction = new Vector3(BallTrans.position.x - collider.transform.position.x, 1, BallTrans.position.z - collider.transform.position.z);

            BallRigibody.AddForce(direction * 100, ForceMode.VelocityChange);
            BallRigibody.AddTorque(direction * 100, ForceMode.VelocityChange);

            Debug.Log("���� ĿƮ");
        }

        // �÷��̾� �� Ʈ���ŷ� ���� �� �̵� Ȥ�� ȸ��
        if (!kickDelay && !isTackled && collider.gameObject.name == "PlayerFoot")
        {
            BallRigibody.velocity = Vector3.zero;

            deceleration = true;

            // �� �����ٵ� �̵�, ȸ�� �� �ۿ�
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(torqueDir * 100, ForceMode.VelocityChange);


            // �� ű ������
            kickDelay = true;

            StartCoroutine(KickDelay());

            kick = true;

            return;
        }

       


    }

    private void OnTriggerExit(Collider collider)
    {
        if (!isTackled && collider.gameObject.name == "PlayerFoot")
        {
            isTackled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ���� �������
        if (collision.gameObject.CompareTag("Floor"))
        {
            // ������ Ư�� ���̿��� �������� �Ҷ�
            if (flickBallDown)
            {
                Player.JumpEnd();

                BallRigibody.velocity = new(BallRigibody.velocity.x, 0, BallRigibody.velocity.z);

                BallRigibody.AddForce(movement * speed / 2, ForceMode.VelocityChange);
                BallRigibody.AddTorque(torqueDir * 10, ForceMode.VelocityChange);

                flickBallDown = false;
                flick = false;
                kickDelay = false;

                fallSpeed = 0;
                return;
            }
        }

    }

}
