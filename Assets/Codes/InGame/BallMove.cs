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
    public Animation BallAni;

    [Space]
    public float speed; 

    static bool kickDelay;
    float dampingFactor = 0.98f;
    bool deceleration, kick, isTackled;
    
    // ballMove
    float ballMaxY;
    bool moveKick;

    // spinMove
    float next_x;
    bool spin, spin_move, spin_revert;
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
            if (!isTackled && !spin && BallTrans.position.z < PlayerTrans.position.z + 3)
            {
                Debug.LogWarning("�� ��ġ ����");
                BallTrans.position = new Vector3(0, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

                BallRigibody.velocity = Vector3.zero;
                BallRigibody.AddForce(1.2f * speed * movement, ForceMode.VelocityChange);
                BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
            }
                

            playerVec = PlayerTrans.position;
            ballVec = BallTrans.position;

            // ��/�� �� �÷��̾ ������ �� �÷��̾� ���� �� �̵� Ȥ�� ȸ��
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

            // �� ���� ���� Ȯ��
            if (spin && BallAni.isPlaying == false)
            {
                SpinMoveEnd();
            }

            // �� ���� 
            if (flick)
            {
                BallTrans.position += Vector3.forward * 0.9f; // �÷��̾� �̵� �ӵ��� ����
                BallRigibody.AddForce(Vector3.down * 100, ForceMode.Acceleration); // �� �������� ���߷�
                BallRigibody.AddTorque(Vector3.right, ForceMode.Acceleration);

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

        BallRigibody.velocity = new Vector3(BallRigibody.velocity.x, 0, BallRigibody.velocity.z);
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.926f, PlayerTrans.position.z + 4.5f);
    }

    // �÷��̾� ���ɰ��α� �� �� ������
    public void SpinMove(string dir)
    {
        if (spin) return;

        spin = true;
        kickDelay = true;

        BallTrans.SetParent(PlayerTrans);

        if (dir == "Left")
        {
            BallAni.Play("Spin_Left");
            //torqueDir = new Vector3(1, -1, 0);
            //next_x = PlayerTrans.position.x - 3;
        }
        else
        {
            //BallAni.SetTrigger("BallSpin_Right");
            //spin_right = true;
            //torqueDir = new Vector3(1, 1, 0);
            //next_x = PlayerTrans.position.x + 3;
        }
    }
    public void SpinMoveEnd()
    {
        BallTrans.SetParent(null);
        Player.SpinEnd();

        kickDelay = false;
        spin = false;
    }

    // �÷��̾� ���� �� �� ����
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

    // �� ű ������ Ʈ���� ���� ������
    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.2f);

        kickDelay = false;
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);
    }


    void OnTriggerEnter(Collider collider)
    {
        // ��Ŭ�� ������ ��
        if (!isTackled && collider.gameObject.name == "TackleFoot")
        {
            isTackled = true;
            deceleration = false;
            kick = false;

            Defender defender = collider.GetComponent<DefenderFootTrigger>().Defender;

            // �¿� �����̵� ��Ŭ ȸ��
            if ((defender.anomalyUserState == "GetTackled_Right" || defender.anomalyUserState == "GetTackled_Left") && flick) 
            { 
                isTackled = false;
                return;
            }

            // ���� ���ĵ� ��Ŭ ȸ��
            if((defender.currentState.ToString() == "Stand_Tackle_Front" || defender.anomalyUserState == "GetStandTackled_Front") && spin)
            {
                isTackled = false;
                return;
            }
                
            if (BallAni.isPlaying)
            {
                BallAni.Stop();
                Debug.LogWarning("�� �ִ� ��ž");
            }

            

            Player.DontMove();

            BallTrans.GetComponent<Collider>().isTrigger = true;
            BallRigibody.constraints &= ~RigidbodyConstraints.FreezePositionX;

            BallRigibody.velocity = Vector3.zero;

            Vector3 direction = new Vector3(BallTrans.position.x - collider.transform.position.x, 1, BallTrans.position.z - collider.transform.position.z);

            BallRigibody.AddForce(direction * 100, ForceMode.VelocityChange);
            BallRigibody.AddTorque(direction * 90, ForceMode.VelocityChange);

            Debug.LogWarning("���� ĿƮ");

            // ���� ��Ŭ ������ �� �÷��̾� ���� ��� 
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

            StartCoroutine(Delay());

            Player.GetTackled(stateName);
        }

        // �÷��̾� �� Ʈ���ŷ� ���� �� �̵� Ȥ�� ȸ��
        if (!kickDelay && !isTackled && collider.gameObject.name == "PlayerFoot")
        {
            BallRigibody.velocity = Vector3.zero;

            deceleration = true;

            // �� �����ٵ� �̵�, ȸ�� �� �ۿ�
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);


            // �� ű ������
            kickDelay = true;

            StartCoroutine(KickDelay());

            kick = true;

            return;
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
                BallRigibody.AddTorque(Vector3.right * 10, ForceMode.VelocityChange);

                flickBallDown = false;
                flick = false;
                kickDelay = false;

                return;
            }
        }

    }

}
