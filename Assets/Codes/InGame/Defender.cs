using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Defender : MonoBehaviour
{
    // ���� Ÿ��
    public enum States
    {
        Stand_Tackle_Front,
        Sliding_Tackle_Front,
        Sliding_Tackle_Anomaly
    }

    // enum Ÿ���� ������ ����
    public States currentState;

    public string anomalyUserState;
    public bool isTackle;
    public bool isHit;

    [Space]

    public DefenderFootTrigger[] FootTriggers;

    Animator DefenderAni;
    BoxCollider boxCollider;
    SkinnedMeshRenderer skinRenderer;

    float fadeDuration = 2f; // ���̵� ���� �ð�
    float totalSpeed;
    string anomalyStr;

    AnimatorStateInfo stateInfo;

    void Awake()
    {
        totalSpeed = 0;
        DefenderAni = gameObject.GetComponent<Animator>();
        skinRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    void FixedUpdate()
    {
        SlidingTackleFront_Update();
    }

    // �޸��� �� �����̵� ��Ŭ�� �ϴ� ����� ������Ʈ ����
    void SlidingTackleFront_Update()
    {
        if (isTackle && currentState.ToString() == "Sliding_Tackle_Front")
        {
            stateInfo = DefenderAni.GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName("Wait") && !stateInfo.IsName("Tackle_Front"))
            {
                // ���� ��Ŭ �� õõ�� �ӵ� �ø���
                if (totalSpeed >= 0.7f)
                {
                    totalSpeed = 0.7f;
                }
                else
                    totalSpeed += 0.1f;

                // �帮�� �̵�
                transform.position += Vector3.back * totalSpeed;
            }

        }
    }

    // ��Ŭ ����
    void Tackle()
    {
        DefenderAni.SetBool(currentState.ToString(), true);
    }
    // Ư�� ����� ���� ��Ŭ �Լ�
    void Tackle(Transform player)
    {
        float playerdis = player.position.x - transform.position.x;

        if (playerdis < -0.5f)
        {
            anomalyStr = "Sliding_Tackle_Right";
            anomalyUserState = "GetTackled_Right";
        }
        else if (playerdis > 0.5f)
        {
            anomalyStr = "Sliding_Tackle_Left";
            anomalyUserState = "GetTackled_Left";
        }
        else
        {
            anomalyStr = "Stand_Tackle_Front";
            anomalyUserState = "GetStandTackled_Front";
        }

        DefenderAni.SetBool(anomalyStr, true);

    }

    // ����
    public void Reset()
    {
        isTackle = false;
        totalSpeed = 0; 
        DefenderAni.speed = 1;

        if (currentState.ToString() == "Sliding_Tackle_Anomaly")
            DefenderAni.SetBool(anomalyStr, false);
        else
            DefenderAni.SetBool(currentState.ToString(), false);

        anomalyStr = "";
        anomalyUserState = "";

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Ư�� �Ÿ� �÷��̾ Ʈ���ſ� ���� �� ��Ŭ ����
        if (collider.gameObject.name == "PlayerTriggerBox")
        {
            if (isTackle) return;

            isTackle = true;

            FootTriggers[0].CheckLimit();
            FootTriggers[1].CheckLimit();

            if (currentState.ToString() == "Sliding_Tackle_Anomaly")
                Tackle(collider.transform);
            else
                Tackle();
        }

        if (collider.CompareTag("Ball"))
        {
            if (isHit) return;

            isHit = true;
            boxCollider.enabled = false;
            if (skinRenderer != null)
            {
                Debug.Log("������ ����");
                Material material = skinRenderer.material;
                Color startColor = material.color;

                material.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    gameObject.SetActive(false); // ���̵� �Ϸ� �� ������Ʈ ��Ȱ��ȭ
                });
            }
            //transform.gameObject.SetActive(false);
        }
    }
}
