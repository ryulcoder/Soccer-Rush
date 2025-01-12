using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform PlayerTransform;
    public Rigidbody PlayerRigibody;
    public Animator PlayerAni;

    public Transform TileTransform;
    
    public float speed, jumpSpeed;
    float distance, totalSpeed, prev_x, next_x;

    bool start, dribbleSlowStart;
    public bool isDribble, isJump, isMove;
    bool ballKick, MoveButtonDelay;

    Vector3 direction;

    AnimatorStateInfo stateInfo;

    public void PlayerStart()
    {
        dribbleSlowStart = true;
        start = true;
    }

    public void Start()
    {
        distance = TileTransform.localScale.y / 4;

        prev_x = PlayerRigibody.position.x;

        PlayerAni.SetTrigger("WaitRun");

    }

    void Update()
    {
        if (start)
        {
            if (!isDribble) 
            {
                isDribble = true;
                PlayerAni.SetTrigger("Dribble"); 
            }

        }

    }


    void FixedUpdate()
    {
        if (start)
        {
            stateInfo = PlayerAni.GetCurrentAnimatorStateInfo(0);

            if (isDribble)
            {
                if (stateInfo.IsName("Start_Run") || stateInfo.IsName("Dribble_Tree") || stateInfo.IsName("Jump_Run"))
                {
                    if (dribbleSlowStart)
                    {
                        totalSpeed += Time.deltaTime;

                        if (totalSpeed >= speed)
                        {
                            dribbleSlowStart = false;
                            totalSpeed = speed;
                        }
                    }

                    PlayerTransform.position += Vector3.forward * totalSpeed;
                }


                if (isMove)
                {
                    PlayerTransform.position += direction * Time.deltaTime * 13;

                    Vector3 position = PlayerTransform.position;

                    if (direction.x > 0)
                        PlayerAni.SetFloat("Horizontal", next_x - position.x < distance / 2 ? next_x - position.x : distance - (next_x - position.x));
                    else
                        PlayerAni.SetFloat("Horizontal", position.x - next_x < distance / 2 ? next_x - position.x : - distance + position.x - next_x);     


                    if (direction.x > 0 && position.x >= next_x || direction.x < 0 && position.x <= next_x + 0.05f)
                    {
                        Debug.Log("도착");

                        PlayerTransform.position = new(next_x, position.y, position.z);

                        PlayerAni.SetFloat("Horizontal", 0);

                        prev_x = next_x;

                        isMove = false;
                        MoveButtonDelay = false;
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        if (start)
        {
            if (isJump && !stateInfo.IsName("Jump_Run"))
            {
                isJump = false;
            }
        }
    }


    public void MoveLeftRight(int moveDirection)
    {
        if (start) 
        { 
            if (isMove || isJump) return;

            if (moveDirection == -1)
            {
                if (PlayerRigibody.position.x <= -distance) return;

                direction = Vector3.left;
            }
            else
            {
                if (PlayerRigibody.position.x >= distance) return;

                direction = Vector3.right;
            }

            next_x = prev_x + direction.x * distance;
            
            PlayerAni.SetFloat("Horizontal", direction.x);

            isMove = true;
        }
    }

    public void Jump()
    {
        if (isJump || isMove) return;

        isJump = true;

        PlayerAni.SetTrigger("Jump");
    }



    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "Ball")
        {
            ballKick = true;

        }

        if (collider.gameObject.name == "Takle_Defender")
        {
            if (isJump)
            {
                Debug.Log("점프중 무적");
                return;
            }

            PlayerAni.SetTrigger("GetTackled_Front");
            totalSpeed = 0;

        }

        if (collider.gameObject.name == "SlidingTakle_Defender")
        {

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "Ball")
        {
            ballKick = false;

        }
    }


}
