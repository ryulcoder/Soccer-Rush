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
    public Animator BallAni;

    [Space]
    public float speed; 

    static bool kickDelay;
    float dampingFactor = 0.98f;
    public bool deceleration, kick, isTackled;
    
    // ballMove
    float ballMaxY;
    bool moveKick;

    // spinMove
    float next_x;
    public bool spin, spin_move, spin_revert;
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
                Debug.LogWarning("pos: " + BallTrans.position + " �� ��ġ ����");
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

    private void LateUpdate()
    {
        // �ִϸ��̼��� ������ �� �θ� ����
        if (spin && !BallAni.isActiveAndEnabled)
        {
            spin = false;
            Player.SpinEnd();

            BallTrans.SetParent(null); 

            kickDelay = false;

            BallTrans.position = new Vector3(0, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.AddForce(1.2f * speed * movement, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
        }
    }

    // �� ��ġ ����
    public void Reset()
    {
        BallTrans.GetComponent<Collider>().isTrigger = false;
        BallRigibody.constraints |= RigidbodyConstraints.FreezePositionX;

        BallRigibody.velocity = Vector3.zero;
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.926f, PlayerTrans.position.z + 4.5f);
    }

    // �÷��̾� ���ɰ��α� �� �� ������
    public void SpinMove(string dir)
    {
        if (spin) return;

        spin = true;
        kickDelay = true;

        BallAni.Rebind();
        BallTrans.SetParent(PlayerTrans);

        BallAni.enabled = true;

        if (dir == "Left")
        {
            BallAni.SetTrigger("Spin_Left");
            //torqueDir = new Vector3(1, -1, 0);
            //next_x = PlayerTrans.position.x - 3;
        }
        else
        {
            BallAni.SetTrigger("Spin_Right");
            //BallAni.SetTrigger("BallSpin_Right");
            //spin_right = true;
            //torqueDir = new Vector3(1, 1, 0);
            //next_x = PlayerTrans.position.x + 3;
        }
    }
    public void SpinMoveEnd()
    {
        BallAni.enabled = false;
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
    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
    }


    void OnTriggerEnter(Collider collider)
    {
        // ��Ŭ�� ������ ��
        if (!isTackled && collider.gameObject.name == "TackleFoot")
        {
            Defender defender = collider.GetComponent<DefenderFootTrigger>().Defender;

            // �¿� �����̵� ��Ŭ ȸ��
            if ((defender.anomalyUserState == "GetTackled_Right" || defender.anomalyUserState == "GetTackled_Left") && flick) 
            {
                //StartCoroutine(Delay(0.1f));
                return;
            }

            // ���� ���ĵ� ��Ŭ ȸ��
            if((defender.currentState.ToString() == "Stand_Tackle_Front" || defender.anomalyUserState == "GetStandTackled_Front") && spin)
            {
                //StartCoroutine(Delay(0.1f));
                return;
            }

            isTackled = true;
            kick = false;

            if (BallAni.isActiveAndEnabled == true)
            {
                SpinMoveEnd();
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

            StartCoroutine(Delay(0.2f));

            Player.GetTackled(stateName);
        }

        // �÷��̾� �� Ʈ���ŷ� ���� �� �̵� Ȥ�� ȸ��
        if (!spin && !flick && !kickDelay && !isTackled && collider.gameObject.name == "PlayerFoot")
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
