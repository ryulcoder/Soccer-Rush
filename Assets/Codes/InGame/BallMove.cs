using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMove : MonoBehaviour
{
    public static BallMove instance;

    Player Player;

    [Header("[ Player ]")]
    Transform PlayerTrans;
    Animator PlayerAni;

    [Header("[ Ball ]")]
    Transform BallTrans;
    Rigidbody BallRigibody;
    Animator BallAni;

    [Space]
    public float speed; 

    [SerializeField] bool kickDelay;
    float dampingFactor = 0.98f;
    public bool deceleration, kick, isTackled;
    
    // ballMove
    float ballMaxY;
    bool moveKick;
    bool ballReset;

    // spinMove
    public bool spin;

    // flick
    bool flick, flickBallDown, ballVelLimit;


    Vector3 movement = Vector3.forward;
    Vector3 torqueDir;

    Vector3 playerVec, ballVec, moveTorqueDir;

    [Header("[ shoot ]")]
    public GameObject HitParticle;
    public float shootSpeed = 10f;  // ���� �̵� �ӵ�
    public float shootDistance = 5f; // ���� �̵��� �Ÿ�
    public Image shootBlurImage;
    public Button shootButton;
    private Vector3 startPosition;
    public bool isShooting = false;
    float ballMaxZ = 100f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Player = Player.Instance;

        PlayerTrans = Player.gameObject.transform;
        PlayerAni = Player.gameObject.GetComponent<Animator>();

        BallTrans = transform;
        BallRigibody = GetComponent<Rigidbody>();
        BallAni = GetComponent<Animator>();

        BallRigibody.maxAngularVelocity = 20;
        
    }

    void FixedUpdate()
    {
        BallDeceleration_Update();

        // ���� á�� ��
        if (kick)
        {
            BallPositionReset_Update();

            playerVec = PlayerTrans.position;
            ballVec = BallTrans.position;

            BallMove_Update();
            BallFlick_Update();
            
        }
        ShootingUpdate();
    }
    void LateUpdate()
    {
        BallSpinEnd_Update();
    }

    // �� �̵�, ȸ�� ���� ������Ʈ ����
    void BallDeceleration_Update()
    {
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
    }
    // ���� ��Ż �� ��ġ ���� ������Ʈ ����
    void BallPositionReset_Update()
    {
        if (!isTackled && !spin && BallTrans.position.z < PlayerTrans.position.z + 3 &&!isShooting && !ballReset)
        {
            ballReset = true;
            Debug.LogWarning("pos: " + BallTrans.position + " �� ��ġ ����");
            BallTrans.position = new Vector3(BallTrans.position.x, 1.926f, PlayerTrans.position.z + 5);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.angularVelocity = Vector3.zero;  // ȸ�� �ӵ��� �ʱ�ȭ
            /*BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * speed, ForceMode.VelocityChange);*/
            StartCoroutine(ResetCooltime());
        }
    }

    // ��/�� �� �÷��̾ ������ �� �÷��̾� ���� �� �̵� Ȥ�� ȸ�� ������Ʈ ����
    void BallMove_Update()
    {
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
    }
    // �÷��̾� ������ �� ���� ������Ʈ ����
    void BallFlick_Update()
    {
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
    // �� ���� �ִϸ��̼��� ������ �� �θ� ���� ������Ʈ ����
    void BallSpinEnd_Update()
    {
        if (spin && !BallAni.isActiveAndEnabled)
        {
            spin = false;
            Player.SpinEnd();

            BallTrans.SetParent(null);

            kickDelay = false;

            BallTrans.position = new Vector3(PlayerTrans.position.x, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.AddForce(1.2f * speed * movement, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
        }
    }

    // �� ��ġ ����
    public void Reset()
    {
        kickDelay = false;
        deceleration = false;
        isTackled = false;
        
        BallTrans.GetComponent<Collider>().isTrigger = false;
        BallRigibody.constraints |= RigidbodyConstraints.FreezePositionX;

        BallRigibody.velocity = Vector3.zero;
        BallRigibody.angularVelocity = Vector3.zero;
        BallTrans.position = new Vector3(PlayerTrans.position.x, 1.926f, PlayerTrans.position.z + 5);
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
            BallAni.SetTrigger("Spin_Left");
        else
            BallAni.SetTrigger("Spin_Right");


    }
    public void SpinMoveEnd()
    {
        BallAni.enabled = false;
    }

    // �÷��̾� ���� �� �� ����
    public void Flick()
    {
        if (flick) return;

        if (LobbyAudioManager.instance != null)
        {
            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.kick);
        }

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
        if (!isTackled && collider.gameObject.name == "TackleFoot" && !isShooting && !collider.gameObject.GetComponent<DefenderFootTrigger>().Defender.isHit)
        {
            Debug.Log("�� ��Ŭ ����");
            Defender defender = collider.GetComponent<DefenderFootTrigger>().Defender;

            // �¿� �����̵� ��Ŭ ȸ��
            if ((defender.anomalyUserState == "GetTackled_Right" || defender.anomalyUserState == "GetTackled_Left") && flick) 
            {
                return;
            }

            // ���� ���ĵ� ��Ŭ ȸ��
            if((defender.currentState.ToString() == "Stand_Tackle_Front" || defender.anomalyUserState == "GetStandTackled_Front") && spin)
            {
                return;
            }

            isTackled = true;
            kick = false;

            // ���� ��Ŭ�� �������� �� �������� �ִϸ��̼� ��������
            if (BallAni.isActiveAndEnabled == true)
            {
                SpinMoveEnd();
                Debug.LogWarning("�� �ִ� ��������");
            }

            // ���� ��Ŭ�� �������� �� �÷��̾� ������ ����
            Player.DontMove();

            // ���� ��Ŭ�� �������� �� ���� �ۿ� ����
            {
                BallTrans.GetComponent<Collider>().isTrigger = true;
                BallRigibody.constraints &= ~RigidbodyConstraints.FreezePositionX;

                BallRigibody.velocity = Vector3.zero;

                Vector3 direction = new Vector3(BallTrans.position.x - collider.transform.position.x, 1, BallTrans.position.z - collider.transform.position.z);

                BallRigibody.AddForce(direction * 100, ForceMode.VelocityChange);
                BallRigibody.AddTorque(direction * 90, ForceMode.VelocityChange);
            }
           
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
        if (!spin && !flick && !kickDelay && !isTackled && collider.gameObject.name == "PlayerFoot" )
        {
            BallRigibody.velocity = Vector3.zero;
            deceleration = true;

            // �� �����ٵ� �̵�, ȸ�� �� �ۿ�
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * speed, ForceMode.VelocityChange);


            // �� ű ������
            kickDelay = true;

            StartCoroutine(KickDelay());

            kick = true;

            return;
        }
       

        HitDefender(collider);

        // ���ϰ� ���� �ٽÿö� �������ֱ�
        if (collider.gameObject.CompareTag("ShootReceive") && isShooting)
        {
            //ballReset = true;
            Debug.LogWarning("pos: " + BallTrans.position + " ���� �� ��ġ ����");
            BallTrans.position = new Vector3(BallTrans.position.x, 1.926f, PlayerTrans.position.z + 4.5f);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.angularVelocity = Vector3.zero;  // ȸ�� �ӵ��� �ʱ�ȭ
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * speed, ForceMode.VelocityChange);

            isShooting = false;
            kickDelay = false;

            StartCoroutine(ResetCooltime());
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
        if (collision.gameObject.CompareTag("Player")){
            Debug.Log("�΋H��");
        }
        //// ���ϰ� ���� �ٽÿö� �������ֱ�
        //if (collision.gameObject.CompareTag("Player") && isShooting)
        //{
        //    ballReset = true;
        //    Debug.LogWarning("pos: " + BallTrans.position + " �� ��ġ ����");
        //    BallTrans.position = new Vector3(0, BallTrans.position.y, PlayerTrans.position.z + 4.5f);

            //    BallRigibody.velocity = Vector3.zero;
            //    BallRigibody.angularVelocity = Vector3.zero;  // ȸ�� �ӵ��� �ʱ�ȭ
            //    BallRigibody.AddForce(1.2f * speed * movement, ForceMode.VelocityChange);
            //    BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);

            //    isShooting = false;
            //    kickDelay = false;

            //    StartCoroutine(ResetCooltime());
            //}
    }

    public void Shoot()
    {
        if (isShooting) return; // �̹� �� ���̸� ����

        if (LobbyAudioManager.instance != null)
        {
            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.shoot);
        }
        isShooting = true;
        kickDelay = true;
        Player.Instance.isShooting = true;

        startPosition = transform.position;
        BallRigibody.velocity = Vector3.zero;
        BallRigibody.AddForce(new(0, 0, 250), ForceMode.VelocityChange);
        BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
        shootBlurImage.fillAmount = 1;
        StartCoroutine(ShootCooltime());
    }

    private IEnumerator ShootCooltime()
    {
        float cooldownTime = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < cooldownTime)
        {
            shootBlurImage.fillAmount = 1 - (elapsedTime / cooldownTime);
            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }
        shootBlurImage.fillAmount = 0; // ���������� 0���� ����
        shootButton.interactable = true;
    }
    
    
    void ShootingUpdate()
    {
        if (transform.position.z - startPosition.z > ballMaxZ && isShooting)
        {
            BallRigibody.velocity = Vector3.zero;
            BallRigibody.AddForce(new(0, 0, -50), ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.left * 90, ForceMode.VelocityChange);

            Debug.Log("�� �������");
        }
    }

    // �������� ������� �°� ���� ���� ������
    void HitDefender(Collider collider)
    {
        if (collider.CompareTag("DefenderHitBox") && isShooting)
        {
            BallRigibody.velocity = Vector3.zero;
            BallRigibody.AddForce(new(0, 0, -50), ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.left * 90, ForceMode.VelocityChange);

            Instantiate(HitParticle, collider.ClosestPoint(transform.position += Vector3.up * 2), Quaternion.identity);
        }
    }

    IEnumerator ResetCooltime()
    {
        yield return new WaitForSeconds(0.5f);
        ballReset = false;
    }
}
