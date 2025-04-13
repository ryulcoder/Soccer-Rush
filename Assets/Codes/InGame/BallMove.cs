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

    [Header("[ Ball ]")]
    Transform BallTrans;
    Rigidbody BallRigibody;
    Animator BallAni;

    [Space]
    public float speed;

    [SerializeField] bool kickDelay;
    float dampingFactor = 0.98f;
    public bool deceleration, kick, isTackled, resetKick, isImpact;

    // ballMove
    float ballMaxY;
    bool moveKick;
    bool ballReset;

    // spinMove
    public bool spin;

    // flick
    bool flick, flickBallDown, ballVelLimit;


    Vector3 movement = Vector3.forward;

    Vector3 playerVec, ballVec, moveTorqueDir;

    [Header("[ shoot ]")]
    public GameObject HitParticle;
    public float shootSpeed = 10f;  // 공의 이동 속도
    public float shootDistance = 5f; // 공이 이동할 거리
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

        BallTrans = transform;
        BallRigibody = GetComponent<Rigidbody>();
        BallAni = GetComponent<Animator>();

        BallRigibody.maxAngularVelocity = 20;

    }

    void FixedUpdate()
    {
        BallDeceleration_Update();

        // 볼을 찼을 시
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

    // 볼 이동, 회전 감속 업데이트 로직
    void BallDeceleration_Update()
    {
        if (deceleration)
        {
            if (Player.dribbleSlowStart)
            {
                BallRigibody.velocity = Vector3.right * BallRigibody.velocity.x + Vector3.up * BallRigibody.velocity.y + Vector3.forward * 35;
            }

            if (flick && BallRigibody.velocity.z > 10)
            {
                BallRigibody.velocity = Vector3.right * BallRigibody.velocity.x + Vector3.up * BallRigibody.velocity.y + Vector3.forward * 9;
            }

            BallRigibody.angularVelocity *= dampingFactor;
            BallRigibody.velocity *= dampingFactor;

            //Debug.Log(BallRigibody.velocity);
        }
    }
    // 공이 이탈 시 위치 조정 업데이트 로직
    void BallPositionReset_Update()
    {
        if (!isImpact && !isTackled && !spin && BallTrans.position.z < PlayerTrans.position.z + 3 && !isShooting && !ballReset)
        {
            ballReset = true;
            Debug.LogWarning("pos: " + BallTrans.position + " 볼 위치 조정");
            BallTrans.position = new Vector3(BallTrans.position.x, 1.926f, PlayerTrans.position.z + 5);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.angularVelocity = Vector3.zero;  // 회전 속도도 초기화
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * speed, ForceMode.VelocityChange);
            StartCoroutine(ResetCooltime());
        }
    }

    // 좌/우 로 플레이어가 움직일 시 플레이어 따라 볼 이동 혹은 회전 업데이트 로직
    void BallMove_Update()
    {
        if (!isImpact && !spin && ballVec.x != playerVec.x)
        {
            if (!moveKick)
            {
                if (ballVec.x < playerVec.x)
                    moveTorqueDir = Vector3.right * 0.3f + Vector3.up;
                else if (ballVec.x > playerVec.x)
                    moveTorqueDir = Vector3.right * 0.3f + Vector3.down;
                else
                    moveTorqueDir = Vector3.right;

                moveTorqueDir = Vector3.Cross(moveTorqueDir.normalized, Vector3.forward);

                BallRigibody.angularVelocity = Vector3.zero;
                BallRigibody.AddTorque(moveTorqueDir * 90, ForceMode.VelocityChange);

                moveKick = true;
            }

            BallTrans.position = Vector3.right * playerVec.x + Vector3.up * ballVec.y + Vector3.forward * ballVec.z;
        }
        else
        {
            moveKick = false;
        }
    }
    // 플레이어 점프시 공 띄우기 업데이트 로직
    void BallFlick_Update()
    {
        if (flick)
        {
            BallTrans.position += Vector3.forward * 0.9f; // 플레이어 이동 속도와 같이
            BallRigibody.AddForce(Vector3.down * 100, ForceMode.Acceleration); // 볼 떨어지는 가중력
            BallRigibody.AddTorque(Vector3.right, ForceMode.Acceleration);


            // 최고 높이 확인
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
    // 볼 스핀 애니메이션이 끝났을 때 부모 해제 업데이트 로직
    void BallSpinEnd_Update()
    {
        if (spin && !BallAni.isActiveAndEnabled)
        {
            spin = false;
            Player.SpinEnd();

            BallTrans.SetParent(null);

            kickDelay = false;

            BallTrans.position = Vector3.right * PlayerTrans.position.x + Vector3.up * BallTrans.position.y + Vector3.forward * (PlayerTrans.position.z + 4.5f);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.angularVelocity = Vector3.zero;

            BallRigibody.AddForce(1.2f * speed * movement, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
        }
    }

    // 볼 위치 리셋
    public void Reset()
    {
        {
            kickDelay = false;
            deceleration = false;
            isTackled = false;
            ballReset = false;
            spin = false;
            flick = false;
            flickBallDown = false;
            ballVelLimit = false;
            isShooting = false;
            resetKick = false;
            isImpact = false;
        }

        shootBlurImage.fillAmount = 0;
        shootButton.interactable = true;

        BallTrans.GetComponent<Collider>().isTrigger = false;
        BallRigibody.constraints |= RigidbodyConstraints.FreezePositionX;

        BallRigibody.velocity = Vector3.zero;
        BallRigibody.angularVelocity = Vector3.zero;
        BallTrans.position = Vector3.right * PlayerTrans.position.x + Vector3.up * 1.926f + Vector3.forward * (PlayerTrans.position.z + 5);
    }

    // 플레이어 스핀개인기 시 볼 움직임
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

    // 플레이어 점프 시 볼 띄우기
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
        BallRigibody.angularVelocity = Vector3.zero;

        BallTrans.position = 
            new Vector3(PlayerTrans.position.x, 1.926f, PlayerTrans.position.z + 4.5f);

        BallRigibody.AddForce(Vector3.up * 55 + Vector3.forward * 9, ForceMode.VelocityChange);
        BallRigibody.AddTorque(Vector3.right * 90, ForceMode.VelocityChange);
    }

    // 볼 킥 여러번 트리거 방지 딜레이
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
        // 태클을 당했을 때
        if (!isTackled && collider.gameObject.name == "TackleFoot" && !isShooting && !collider.gameObject.GetComponent<DefenderFootTrigger>().Defender.isHit)
        {
            Debug.LogWarning("공 트리거 활성화");
            Defender defender = collider.GetComponent<DefenderFootTrigger>().Defender;

            // 좌우 슬라이딩 태클 회피
            if ((defender.anomalyUserState == "GetTackled_Right" || defender.anomalyUserState == "GetTackled_Left") && flick)
            {
                return;
            }

            // 정면 스탠드 태클 회피
            if ((defender.currentState.ToString() == "Stand_Tackle_Front" || defender.anomalyUserState == "GetStandTackled_Front") && spin)
            {
                return;
            }

            isTackled = true;
            kick = false;

            // 볼에 태클이 적중했을 시 실행중인 애니메이션 강제종료
            if (BallAni.isActiveAndEnabled == true)
            {
                SpinMoveEnd();
                Debug.LogWarning("볼 애니 강제종료");
            }

            // 볼에 태클이 적중했을 시 플레이어 움직임 막기
            Player.DontMove();

            // 볼에 태클이 적중했을 시 물리 작용 적용
            {
                BallTrans.GetComponent<Collider>().isTrigger = true;
                BallRigibody.constraints &= ~RigidbodyConstraints.FreezePositionX;

                BallRigibody.velocity = Vector3.zero;

                Vector3 direction = new(BallTrans.position.x - collider.transform.position.x, 1, BallTrans.position.z - collider.transform.position.z);

                BallRigibody.AddForce(direction * 100, ForceMode.VelocityChange);
                BallRigibody.AddTorque(direction * 90, ForceMode.VelocityChange);
            }

            Debug.LogWarning("수비 커트");

            // 볼만 태클 당했을 시 플레이어 강제 모션 
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

        if (isImpact && collider.gameObject.name == "PlayerFoot")
        {
            isImpact = false;

            GameManager.Instance.ReSpeedUp();
            GameManager.Instance.InputOn();
        }


        // 플레이어 발 트리거로 인한 볼 이동 혹은 회전
        if (!isImpact && !spin && !flick && !kickDelay && !isTackled && collider.gameObject.name == "PlayerFoot")
        {
            kickDelay = true;

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.angularVelocity = Vector3.zero;

            deceleration = true;

            // 볼 리지바디 이동, 회전 힘 작용
            StartCoroutine(ReceiveKick());


            // 볼 킥 딜레이
            StartCoroutine(KickDelay());

            kick = true;

            return;
        }


        HitDefender(collider);

        // 슛하고 공이 다시올때 변수꺼주기
        if (collider.gameObject.CompareTag("ShootReceive") && isShooting)
        {
            ballReset = true;
            kickDelay = true;

            Debug.LogWarning("pos: " + BallTrans.position + " 슛팅 볼 위치 조정");
            BallTrans.position = new Vector3(BallTrans.position.x, 1.926f, PlayerTrans.position.z + 4.5f);

            BallRigibody.velocity = Vector3.zero;
            BallRigibody.angularVelocity = Vector3.zero;  // 회전 속도도 초기화

            StartCoroutine(ReceiveKick());

            isShooting = false;

            // 볼 킥 딜레이
            StartCoroutine(KickDelay());

            //StartCoroutine(ResetCooltime());
            ballReset = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // 볼이 땅에 닿았을때
        if (collision.gameObject.CompareTag("Floor"))
        {
            // 포물선 특정 높이에서 떨어져야 할때
            if (flickBallDown)
            {
                Player.JumpEnd();

                BallRigibody.velocity = Vector3.right * BallRigibody.velocity.x + Vector3.forward * BallRigibody.velocity.z;

                BallRigibody.AddForce(movement * speed / 2, ForceMode.VelocityChange);
                BallRigibody.AddTorque(Vector3.right * 10, ForceMode.VelocityChange);

                flickBallDown = false;
                flick = false;
                kickDelay = false;

                return;
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("부딫힘");
        }
    }

    public void Shoot()
    {
        if (isShooting) return; // 이미 슛 중이면 무시

        if (LobbyAudioManager.instance != null)
        {
            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.shoot);
        }
        isShooting = true;
        kickDelay = true;
        Player.Instance.isShooting = true;

        startPosition = transform.position;
        BallRigibody.velocity = Vector3.zero;
        BallRigibody.AddForce(Vector3.forward * 250, ForceMode.VelocityChange);
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
            yield return null; // 다음 프레임까지 대기
        }
        shootBlurImage.fillAmount = 0; // 최종적으로 0으로 설정
        shootButton.interactable = true;
    }


    void ShootingUpdate()
    {
        if (transform.position.z - startPosition.z > ballMaxZ && isShooting)
        {
            BallRigibody.velocity = Vector3.zero;
            BallRigibody.AddForce(Vector3.forward * -50, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.left * 90, ForceMode.VelocityChange);

            Debug.Log("나 멈춘다잉");
        }
    }

    // 슛했을때 수비수가 맞고 나서 공의 움직임
    void HitDefender(Collider collider)
    {
        if (collider.CompareTag("DefenderHitBox") && isShooting)
        {
            BallRigibody.velocity = Vector3.zero;
            BallRigibody.AddForce(Vector3.forward * -50, ForceMode.VelocityChange);
            BallRigibody.AddTorque(Vector3.left * 90, ForceMode.VelocityChange);

            Instantiate(HitParticle, collider.ClosestPoint(transform.position += Vector3.up * 2.5f), Quaternion.identity);
        }
    }

    IEnumerator ResetCooltime()
    {
        yield return new WaitForSeconds(0.5f);
        ballReset = false;
    }

    IEnumerator ReceiveKick()
    {
        resetKick = true;

        BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
        BallRigibody.AddTorque(Vector3.right * speed, ForceMode.VelocityChange);

        yield return null;

        resetKick = false;
    }

    public void ImpactSetting(float stopPoint)
    {
        isImpact = true;

        BallRigibody.velocity = Vector3.zero;
        BallRigibody.angularVelocity = Vector3.zero;

        transform.position = transform.position.y * Vector3.up + stopPoint * Vector3.forward;
    }

    public void ImpactEnd(bool isGoal)
    {
        if (isGoal)
            BallTrans.position = Vector3.up * 1.926f + Vector3.forward * BallTrans.position.z;
        else
            BallTrans.position = Vector3.up * 1.926f + Vector3.forward * (PlayerTrans.position.z + 5);

        BallRigibody.velocity = Vector3.zero;
        BallRigibody.angularVelocity = Vector3.zero;
    }
}
