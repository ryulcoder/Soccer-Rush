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

    float offsetX, targetX, lerpSpeed;

    void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        defaultVec = transposer.m_FollowOffset;
    }

    void FixedUpdate()
    {
        if (targetX != Target.position.x)
        {
            targetX = Target.position.x;

            if (targetX < 0)
            {
                offsetX = targetX * -0.1f + defaultVec.x;
                lerpSpeed = Time.deltaTime * 2;
            }  
            else
            {
                offsetX = targetX * -0.35f + defaultVec.x;
                lerpSpeed = Time.deltaTime * 15;
            }

            targetVec.x = offsetX;
            targetVec.y = transposer.m_FollowOffset.y;
            targetVec.z = transposer.m_FollowOffset.z;

            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetVec, lerpSpeed);
        }
    }

}
