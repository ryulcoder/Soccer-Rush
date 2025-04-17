using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    BallMove BallMove;
    Stamina Stamina;

    Transform PlayerTransform;
    Animator PlayerAni;

    [Header("Particles")]
    public GameObject DustParticle;
    public GameObject ShootDustParticle;

    [Space]
    public Transform TileTransform;

    public float speed, jumpSpeed, distance, totalSpeed;
    [SerializeField] float next_x, returnZero;

    public bool dribbleSlowStart, getTackled, isAct, isImpact, redribble;
    bool start, dontMove, ballReset, isDribble, isJump, isSpin, isAvoid;

    Vector3 direction, defaultVec, movePos;

    public AnimatorStateInfo stateInfo;

    [Header("Shooting")]
    public bool isShooting;
    public Button shootButton;

    public void PlayerStart()
    {
        dontMove = false;
        dribbleSlowStart = true;
        start = true;
    }

    // ���� ���� ��Ŭ�� �ɷ��� �� ������ ����
    public void DontMove()
    {
        Debug.LogWarning("���� ���� �ɸ�");

        dontMove = true;
        dribbleSlowStart = false;
        isJump = false;

    }
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BallMove = BallMove.instance;
        Stamina = Stamina.instance;

        PlayerTransform = transform;
        PlayerAni = GetComponent<Animator>();

        isSpin = true;
        isJump = true;

        distance = TileTransform.localScale.x / 3;

        PlayerAni.SetTrigger("WaitRun");

        isDribble = true;

        PlayerAni.SetTrigger("Dribble");

    }

    void FixedUpdate()
    {
        PlayerGetTackled_Update();

        if (start)
        {
            PlayerMove_Update();
            PlayerDribble_Update();
            PlayerImpactShoot_Update();
        }
    }

    // �÷��̾� ��Ŭ�� �������� Ư�� ����� ���� ������Ʈ ����
    void PlayerGetTackled_Update()
    {
        // �÷��̾� �ִϸ����� ���� �ִϸ��̼� Ȯ��
        stateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0);

        if (getTackled)
        {
            if (!ballReset && stateInfo.IsName("StandUp") || stateInfo.IsName("StandUp_StandTackle"))
            {
                ballReset = true;

                BallMove.Reset();
            }

            if (stateInfo.IsName("Fallen") || stateInfo.IsName("GetStandTackled_Front"))
            {
                start = false;
                isDribble = false;

                totalSpeed = 0;
            }
            else if (stateInfo.IsName("Wait_Run"))
            {
                BallMove.gameObject.SetActive(true);

                getTackled = false;
                dontMove = false;

                dribbleSlowStart = true;
                start = true;

                PlayerAni.SetTrigger("Dribble");
                isDribble = true;
                ballReset = false;
            }
            else
            {
                PlayerTransform.position += Vector3.forward * totalSpeed;
                totalSpeed *= 0.95f;
            }
        }
    }
    // �÷��̾� �¿� ������ ������Ʈ ����
    void PlayerMove_Update()
    {
        if (!getTackled)
        {
            movePos = PlayerTransform.position;

            if (next_x != movePos.x)
            {
                // ������ġ ���� �� ��ž �� �ʱ�ȭ
                if (direction.x > 0 && movePos.x >= next_x - 1 || direction.x < 0 && movePos.x <= next_x + 1)
                {
                    isAct = false;

                    PlayerTransform.position = Vector3.right * next_x + Vector3.up * movePos.y + Vector3.forward * movePos.z;

                    if (stateInfo.IsName("Move_Left") || stateInfo.IsName("Move_Right"))
                    {
                        redribble = true;
                        PlayerAni.ResetTrigger("ReDribble");
                        PlayerAni.SetTrigger("ReDribble");
                    }

                }
                else
                    PlayerTransform.position += 22 * Time.deltaTime * direction;

            }
            else
            {
                if (redribble && (stateInfo.IsName("Move_Left") || stateInfo.IsName("Move_Right")))
                {
                    PlayerAni.ResetTrigger("ReDribble");
                    PlayerAni.SetTrigger("ReDribble");
                    redribble = true;
                }
                else if (redribble)
                {
                    redribble = false;
                    PlayerAni.ResetTrigger("ReDribble");
                }

            }
        }
    }
    // �÷��̾� �帮�� ������Ʈ ����
    void PlayerDribble_Update()
    {
        if (isDribble)
        {
            if (stateInfo.IsName("Start_Run") || stateInfo.IsName("Dribble") || stateInfo.IsName("Jump_Run")
                || stateInfo.IsName("Spin_Left") || stateInfo.IsName("Spin_Right")
                || stateInfo.IsName("Move_Left") || stateInfo.IsName("Move_Right")
                || stateInfo.IsName("ShootingRun"))
            {
                // ó�� �帮�� �� õõ�� �ӵ� �ø���
                if (dribbleSlowStart)
                {
                    totalSpeed += Time.deltaTime;

                    if (totalSpeed >= speed)
                    {
                        dribbleSlowStart = false;
                        isJump = isSpin = isAct = isAvoid = false;
                        totalSpeed = speed;
                    }
                }

                // �帮�� �̵�
                PlayerTransform.position += Vector3.forward * totalSpeed;
            }
        }
    }

    void PlayerImpactShoot_Update()
    {
        if (isImpact && (stateInfo.IsName("Shoot_Right") || stateInfo.IsName("Shoot_Center")))
        {
            transform.position = Vector3.MoveTowards(transform.position, defaultVec + 2 * returnZero * Vector3.left, Time.deltaTime);
        }
    }


    // �÷��̾� �¿� �̵� �Լ�
    public void MoveLeftRight(int moveDirection)
    {
        if ((moveDirection > 0 && PlayerTransform.position.x >= distance) || (moveDirection < 0 && PlayerTransform.position.x <= -distance)) { Debug.LogWarning("Block"); return; }

        if (!start || dontMove || getTackled || isSpin || isJump || dribbleSlowStart || BallMove.isShooting) { Debug.LogWarning("Block"); return; }

        if (LobbyAudioManager.instance != null)
        {
            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.kick);
        }

        direction = Vector3.right * moveDirection;
        next_x += moveDirection * distance;

        if (moveDirection > 0)
            PlayerAni.SetTrigger("MoveRight");
        else
            PlayerAni.SetTrigger("MoveLeft");
        

        if (Mathf.Abs(next_x) >= distance)
        {
            next_x = moveDirection * distance;
        }

        ExtraScore.instance.CheckStart("AvoidMove");
    }

    // �÷��̾� ���α� �Լ�
    public void Spin()
    {
        if (dontMove || getTackled || isSpin || isJump || dribbleSlowStart || BallMove.isShooting) { Debug.LogWarning("Block"); return; }

        if (!Stamina.UseStamina(20)) { Debug.LogWarning("���¹̳� ����"); return; }

        isSpin = true;
        isAct = true;

        PlayerAni.SetTrigger("Spin_Left");
        BallMove.SpinMove("Left");
            

    }
    public void SpinEnd()
    {
        isSpin = false;
        isAct = false;
        isAvoid = false;

    }

    // �÷��̾� ���� �Լ�
    public void Jump()
    {
        if (dontMove || getTackled || isSpin || isJump || dribbleSlowStart || BallMove.isShooting) { Debug.LogWarning("Block"); return; }

        if (!Stamina.UseStamina(20)) { Debug.LogWarning("���¹̳� ����"); return; }

        isJump = true;
        isAct = true;

        PlayerAni.SetTrigger("Jump");

        BallMove.Flick();
    }
    public void JumpEnd()
    {
        isJump = false;
        isAct = false;
        isAvoid = false;
    }

    // �÷��̾� ��Ŭ ���� Ȯ�� �Լ�
    public void GetTackled(string tackleName)
    {
        // �����̵� ��Ŭ ȸ�� ����
        if ((tackleName == "GetTackled_Right" || tackleName == "GetTackled_Left" || tackleName == "GetTackled_Front") && isJump && !dontMove)
        {
            if (isAvoid) return;
            isAvoid = true;

            Debug.LogWarning("���� ����");
            ExtraScore.instance.CheckStart("AvoidJump");
            return;
        }

        // ���ĵ� ��Ŭ ȸ�� ����
        if (tackleName == "GetStandTackled_Front" && isSpin && !dontMove)
        {
            if (isAvoid) return;
            isAvoid = true;

            Debug.LogWarning("�������� ����");
            ExtraScore.instance.CheckStart("AvoidSpin");
            return;
        }


        if (getTackled) return;
        getTackled = true;

        if (tackleName == "GetStandTackled_Front_Anomaly")
            tackleName = "GetStandTackled_Front";
        if (LobbyAudioManager.instance != null)
        {

            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.death);
            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.deathPunch);
        }

        ResetAllTriggers();
        PlayerAni.SetTrigger(tackleName);

    }

    // ���� ��ư ������
    public void ShootingAni()
    {
        if (dontMove || getTackled || isSpin || isJump || dribbleSlowStart || BallMove.isShooting) { Debug.LogWarning("Block"); return; }

        if (!Stamina.UseStamina(20)) { Debug.LogWarning("���¹̳� ����"); return; }

        PlayerAni.SetTrigger("Shooting");
        shootButton.interactable = false;
    }

    public void ShootingBall()
    {
        BallMove.Shoot();
        ShootDustParticle.SetActive(true);
    }

    public void DustOn()
    {
        DustParticle.SetActive(true);
    }

    public void PlayerReset()
    {
        float player_x = transform.position.x;

        if (player_x >= distance / 2)
            player_x = distance;
        else if (player_x >= -distance / 2)
            player_x = 0;
        else
            player_x = -distance;

        gameObject.transform.position = Vector3.right * player_x + Vector3.up * transform.position.y + Vector3.forward * transform.position.z;
    }

    public void ImpactSetting()
    {
        SwipInput.instance.isImpact = true;
        shootButton.gameObject.SetActive(false);

        returnZero = 1;
        isImpact = true;

        StartCoroutine(ImpactMoveCenterLoop());
    }

    IEnumerator ImpactMoveCenterLoop()
    {
        yield return new WaitForSeconds(2);

        if (transform.position.x > 0)
            MoveLeftRight(-1);
        else if (transform.position.x < 0)
            MoveLeftRight(1);

        yield break;
    }

    public void ImpactZoneKickOn()
    {
        returnZero = 0;
        totalSpeed = 0;

        if (LobbyAudioManager.instance != null)
        {

            LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.shoot);

        }

        ImpactZone.Instance.ImpactKickOn();
    }

    public void GoalCheck(bool isGoal)
    {
        if (isGoal)
            PlayerAni.SetTrigger("Goal");
        else
            PlayerAni.SetTrigger("NoGoal");
    }

    

    public void Spinballtouch()
    {
        if (LobbyAudioManager.instance == null)
            return;

        LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.kick);
    }

    //public void StartWhistle()
    //{
    //    if (LobbyAudioManager.instance == null)
    //        return;

    //    LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.startWhistle);
    //}

    public void StartKick()
    {
        if (LobbyAudioManager.instance == null)
            return;
        LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.kick);
    }

    void ResetAllTriggers()
    {
        for (int i = 0; i < PlayerAni.parameterCount; i++)
        {
            AnimatorControllerParameter param = PlayerAni.GetParameter(i);
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                PlayerAni.ResetTrigger(param.name);
            }
        }
    }

}