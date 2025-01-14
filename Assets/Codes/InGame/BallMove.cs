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
        // �� �̵�, ȸ�� ���� ���� ����
        if (deceleration)
        {
            BallRigibody.angularVelocity *= dampingFactor;
            BallRigibody.velocity *= dampingFactor;

        }

        // ���� á�� ��
        if (kick)
        {
            // ���� ��Ż �� ��ġ ����
            if (BallTrans.position.z < PlayerTrans.position.z + 3)
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
                BallTrans.position += Vector3.forward; // �÷��̾� �̵� �ӵ��� ����
                BallRigibody.AddForce(Vector3.down * 55, ForceMode.Acceleration); // �� �������� ���߷�
                BallRigibody.AddTorque(torqueDir, ForceMode.Acceleration);

                // ������ Ư�� ���̿��� �������� �Ҷ�
                if (flickBallDown)
                {
                    // ���� ���� �������
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

    // �÷��̾� ���� �� �� ����
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

    // �� ��ġ ����
    public void Reset()
    {
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.52f, PlayerTrans.position.z + 4);
    }

    // �� ű ������ Ʈ���� ���� ������
    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.2f);

        kickDelay = false;
    }


    
    void OnTriggerStay(Collider collider)
    {
        // �÷��̾� �� Ʈ���ŷ� ���� �� �̵� Ȥ�� ȸ��
        if (collider.gameObject.name == "PlayerFoot")
        {
            if (kickDelay) return;

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
