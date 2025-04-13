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

    public float followSpeed = 50f; // 속도 조절 변수

    float offsetX, targetX, lerpSpeed;

    void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        defaultVec = transposer.m_FollowOffset;
    }

    /*void FixedUpdate()
    {
        float offsetX = Target.position.x < 0 ? Target.position.x * left + defaultVec.x : -Target.position.x * right + defaultVec.x;

        targetVec = new Vector3(offsetX, transposer.m_FollowOffset.y, transposer.m_FollowOffset.z);
        
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetVec, Target.position.x < 0 ? Time.deltaTime * 2 : Time.deltaTime * 15);

    }*/

    void FixedUpdate()
    {
        targetX = Target.position.x;

        offsetX = targetX < 0 ? targetX * left + defaultVec.x: -targetX * right + defaultVec.x;

        targetVec.x = offsetX;
        targetVec.y = transposer.m_FollowOffset.y;
        targetVec.z = transposer.m_FollowOffset.z;

        lerpSpeed = targetX < 0 ? Time.deltaTime * 2 : Time.deltaTime * 15;

        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetVec, lerpSpeed);
    }

}
