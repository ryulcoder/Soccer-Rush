using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingHit : MonoBehaviour
{
    public Defender Defender;

    CapsuleCollider capsuleCollider;
    Animator animator;
    BallMove ballMove;

    GameObject ball;

    void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInParent<Animator>();
        ball = GameObject.FindWithTag("Ball");
        ballMove = ball.GetComponent<BallMove>();
    }

    void OnEnable()
    {
        Defender.isHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !Defender.isHit && ballMove.isShooting)
        {
            Defender.isHit = true;
            animator.SetTrigger("ShootingDeath");   
        }
    }
}
