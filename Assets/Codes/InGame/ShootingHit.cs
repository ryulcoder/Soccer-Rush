using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingHit : MonoBehaviour
{
    CapsuleCollider capsuleCollider;
    Animator animator;
    BallMove ballMove;

    GameObject ball;
    bool isHit;

    void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInParent<Animator>();
        ball = GameObject.FindWithTag("Ball");
        ballMove = ball.GetComponent<BallMove>();
    }

    void OnEnable()
    {
        isHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !isHit && ballMove.isShooting)
        {
            isHit = true;
            animator.SetTrigger("ShootingDeath");   
        }
    }
}
