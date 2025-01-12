using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;

    public Vector3 Offset;
    public Vector3 targetZ;

    void Awake()
    {
        Offset = transform.position;
    }

    void FixedUpdate()
    {
        targetZ = new(0, 0, Target.position.z);
        transform.position = Offset + targetZ;
    }
}
