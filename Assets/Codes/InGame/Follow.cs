using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;

    public CinemachineVirtualCamera virtualCamera;
    CinemachineTransposer transposer;

    public Vector3 defaultVec;
    public Vector3 targetVec;

    float left = -0.1f;
    float right = 0.35f;

    public float followSpeed = 50f; // �ӵ� ���� ����

    void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        defaultVec = transposer.m_FollowOffset;
    }

    void FixedUpdate()
    {
        float offsetX = Target.position.x < 0 ? Target.position.x * left + defaultVec.x : -Target.position.x * right + defaultVec.x;

        targetVec = new Vector3(offsetX, transposer.m_FollowOffset.y, transposer.m_FollowOffset.z);
        
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetVec, Target.position.x < 0 ? Time.deltaTime * 2 : Time.deltaTime * 15);

        /*// ��ǥ ��ġ ���
        targetVec = new Vector3(
            Target.position.x < 0 ? Target.position.x * left + defaultVec.x : Target.position.x * right + defaultVec.x,
            transform.position.y,
            Target.position.z + defaultVec.z
        );

        // ���� �̵� (Lerp ����)
        float step = followSpeed * Time.deltaTime;
        transform.position = new Vector3(
            Mathf.MoveTowards(transform.position.x, targetVec.x, step),
            transform.position.y,
            targetVec.z
        );*/
    }
}
