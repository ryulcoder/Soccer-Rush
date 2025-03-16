using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingHit : MonoBehaviour
{
    public Defender Defender;
    public GameObject DropToFloorParticle;

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

        DropToFloorParticle.SetActive(false);
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
            if (LobbyAudioManager.instance != null)
            {
                LobbyAudioManager.instance.PlaySfx(LobbyAudioManager.Sfx.shootHit);
            }
            animator.SetTrigger("ShootingDeath");
            ExtraScore.instance.CheckStart("HitShoot");

            StartCoroutine(DropToFloorParticleDelay());
        }
    }

    IEnumerator DropToFloorParticleDelay()
    {
        yield return new WaitForSeconds(0.75f);

        if (DropToFloorParticle.activeSelf)
            DropToFloorParticle.SetActive(false);

        DropToFloorParticle.SetActive(true);
    }
}
