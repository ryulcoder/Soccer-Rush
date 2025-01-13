using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Player Player;

    [Header("[ Player ]")]
    public Transform PlayerTrans;
    public Animator PlayerAni;

    [Header("[ Ball ]")]
    public Transform BallTrans;
    public Rigidbody BallRigibody;

    [Space]
    public float speed; 
    float dampingFactor = 0.98f;

    bool deceleration, kick, moveKick;
    static bool kickDelay;

    Vector3 movement = Vector3.forward;
    Vector3 torqueDir = Vector3.right;

    Vector3 playerVec, moveTorqueDir;

    void Start()
    {
        BallRigibody.maxAngularVelocity = 20;
    }

    void FixedUpdate()
    {
        if (deceleration)
        {
            // 볼 이동, 회전 감속 로직 적용
            BallRigibody.angularVelocity *= dampingFactor;
            BallRigibody.velocity *= dampingFactor;
            
        }

        if (kick)
        {
            if (BallTrans.position.z < PlayerTrans.position.z + 3)
                BallTrans.position = new Vector3(0, 1.52f, PlayerTrans.position.z + 4);

            playerVec = PlayerTrans.position;

            if (BallTrans.position.x != playerVec.x)
            {
                if (!moveKick)
                {
                    if (BallTrans.position.x < playerVec.x)
                        moveTorqueDir = new Vector3(1, 1, 0);
                    else
                        moveTorqueDir = new Vector3(1, -1, 0);

                    BallRigibody.AddTorque(moveTorqueDir * 100, ForceMode.VelocityChange);

                    moveKick = true;
                }
                    
                BallTrans.position = new(playerVec.x, BallTrans.position.y, BallTrans.position.z);
            }
            else
            {

                moveKick = false;
            }
                

        }

    }


    public void Reset()
    {
        BallTrans.position = new Vector3(0, 1.52f, PlayerTrans.position.z + 4);
    }

    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.2f);

        kickDelay = false;
    }


    
    void OnTriggerStay(Collider collider)
    {
        // 플레이어 발 트리거로 인한 볼 회전
        if (collider.gameObject.name == "PlayerFoot")
        {
            if (kickDelay) return;

            BallRigibody.velocity = Vector3.zero;

            deceleration = true;

            // 볼 리지바디 이동, 회전 힘 작용
            BallRigibody.AddForce(movement * speed, ForceMode.VelocityChange);
            BallRigibody.AddTorque(torqueDir * 100, ForceMode.VelocityChange);

            kickDelay = true;

            StartCoroutine(KickDelay());

            kick = true;

            return;
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Defender") return;

        Vector3 direction = (collision.transform.position - collision.contacts[0].point).normalized;

        BallRigibody.AddForce(direction * speed, ForceMode.Impulse);
        BallRigibody.AddTorque(direction * speed, ForceMode.Impulse);
    }

}
