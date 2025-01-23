using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamerafocusOff : MonoBehaviour
{
    public CinemachineVirtualCamera vcam1; // Look At 활성화
    public CinemachineVirtualCamera vcam2; // Look At 비활성화
    public float switchPosition = 4f;      // 전환할 Dolly Path Index
    private CinemachineTrackedDolly dolly;

    void Start()
    {
        dolly = vcam1.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
        if (dolly.m_PathPosition >= switchPosition)
        {
            vcam1.Priority = 0; // 비활성화
            vcam2.Priority = 1; // 활성화
        }
        else
        {
            vcam1.Priority = 1; // 활성화
            vcam2.Priority = 0; // 비활성화
        }
    }

}
