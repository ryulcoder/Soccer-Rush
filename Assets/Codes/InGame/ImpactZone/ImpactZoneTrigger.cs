using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactZoneTrigger : MonoBehaviour
{
    public Transform ImpactZoneCenter;

    Player Player;
    BallMove Ball;

    

    [SerializeField] float stopPoint, gameSpeed;

    [SerializeField] bool impactZoneIn, speedDown, isBallStop, isPlayerStop, ballStopLoop;

    private void Start()
    {
        if (Player == null)
            Player = Player.Instance;

        if (Ball == null)
            Ball = BallMove.instance;
    }

    private void LateUpdate()
    {
        if (impactZoneIn)
        {
            if (!isBallStop && Ball.transform.position.z >= stopPoint)
            {
                isBallStop = true;

                Ball.ImpactSetting(stopPoint + 5);
                BallMove.instance.deceleration = false;
            }

            if (!isPlayerStop && Player.transform.position.z >= stopPoint - 10.5f)
            {
                isPlayerStop = true;
                Player.Instance.GetComponent<Animator>().SetTrigger("Impact");
            }

            if (isBallStop && isPlayerStop)
            {
                impactZoneIn = false;
                isBallStop = false;
                isPlayerStop = false;
                ballStopLoop = true;

                ImpactKick();
            }
                
        }

        if (speedDown)
        {
            Time.timeScale -= (gameSpeed - 1) * 0.1f;

            if (Time.timeScale <= 1)
            {
                Time.timeScale = 1;
                speedDown = false;
            }
        }

        if (ballStopLoop)
        {
            Ball.ImpactSetting(stopPoint + 5);
        }
    }

    void ImpactKick()
    {
        StartCoroutine(KickDelay());
    }

    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.4f);

        ballStopLoop = false;

        if (ImpactZone.Instance.right)
            Player.Instance.GetComponent<Animator>().SetTrigger("Impact_Kick_Right");
        else
            Player.Instance.GetComponent<Animator>().SetTrigger("Impact_Kick_Left");
    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !impactZoneIn)
        {
            impactZoneIn = true;

            stopPoint = ImpactZoneCenter.position.z;

            gameSpeed = Time.timeScale;
            speedDown = true;

            Player.Instance.ImpactSetting();

            ImpactZone.Instance.right = Random.Range(0, 2) != 0;
        }
    }
}
