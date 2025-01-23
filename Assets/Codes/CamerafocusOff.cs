using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamerafocusOff : MonoBehaviour
{
    public CinemachineVirtualCamera vcam1; // Look At Ȱ��ȭ
    public CinemachineVirtualCamera vcam2; // Look At ��Ȱ��ȭ
    public float switchPosition = 4f;      // ��ȯ�� Dolly Path Index
    private CinemachineTrackedDolly dolly;

    void Start()
    {
        dolly = vcam1.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
        if (dolly.m_PathPosition >= switchPosition)
        {
            vcam1.Priority = 0; // ��Ȱ��ȭ
            vcam2.Priority = 1; // Ȱ��ȭ
        }
        else
        {
            vcam1.Priority = 1; // Ȱ��ȭ
            vcam2.Priority = 0; // ��Ȱ��ȭ
        }
    }

}
