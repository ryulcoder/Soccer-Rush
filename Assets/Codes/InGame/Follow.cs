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

    float targetdis;

    void Awake()
    {
        defaultVec = transform.position;
        targetdis = Target.position.x - transform.position.x;
    }

    void FixedUpdate()
    {
        targetVec = new(Target.position.x < 0 ? Target.position.x * left + defaultVec.x : Target.position.x * right + defaultVec.x, transform.position.y, Target.position.z + defaultVec.z);

        transform.position = Vector3.Lerp(transform.position, targetVec, Target.position.x < 0 ? Time.deltaTime * 7 : Time.deltaTime * 5);
        transform.position = new Vector3(transform.position.x, transform.position.y, Target.position.z + defaultVec.z);

    }

}
