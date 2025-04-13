using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goalkeeper_Hands : MonoBehaviour
{
    Transform BallTrans;
    Collider BallCollider;

    static bool isCatch;

    private void Start()
    {
        isCatch = false;

        BallTrans = null;
        BallCollider = null;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Ball") && !isCatch)
        {
            isCatch = true;

            BallCollider = collider;
            BallTrans = BallCollider.transform;

            BallCollider.isTrigger = true;
            BallCollider.attachedRigidbody.useGravity = false;

            BallCollider.attachedRigidbody.velocity = Vector3.zero;
            BallCollider.attachedRigidbody.angularVelocity = Vector3.zero;

            BallTrans.SetParent(transform);

            ImpactZone.Instance.NoGoal();

            Debug.LogWarning("NoGoal");
        }
    }
}
