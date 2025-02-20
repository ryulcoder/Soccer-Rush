using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;

    public Vector3 defaultVec;
    public Vector3 targetVec;

    public float left = 0.9f;
    public float right = 0.6f;

    public float followSpeed = 50f; // �ӵ� ���� ����

    void Awake()
    {
        defaultVec = transform.position;
    }

    void LateUpdate()
    {
        // ��ǥ ��ġ ���
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
        );
    }
}
