using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;

    public Vector3 defaultVec;
    public Vector3 targetVec;

    float targetdis;

    void Awake()
    {
        defaultVec = transform.position;
        targetdis = Target.position.x - transform.position.x;
    }

    void FixedUpdate()
    {
        targetVec = new(Target.position.x + defaultVec.x, transform.position.y, Target.position.z + defaultVec.z);

        transform.position = Vector3.Lerp(transform.position, targetVec, Time.deltaTime * 5);
        transform.position = new Vector3(transform.position.x, transform.position.y, Target.position.z + defaultVec.z);

    }

}
