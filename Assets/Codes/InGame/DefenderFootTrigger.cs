using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderFootTrigger : MonoBehaviour
{
    public Defender Defender;

    static Player Player;
    static Transform PlayerTrans;

    Transform DefenderTrans;

    bool checkLimitOn, isAnomaly, debugMessage, debugMessage2;
    float limitCheckDis, limitCheckTime;

    void Start()
    {
        if (Player == null)
            Player = Player.Instance;

        if (PlayerTrans == null)
            PlayerTrans = Player.gameObject.transform;

        DefenderTrans = Defender.gameObject.transform;

        checkLimitOn = isAnomaly = debugMessage = debugMessage2 = false;

        limitCheckDis = ExtraScore.instance.limitDis;
        limitCheckTime = ExtraScore.instance.limitTime;

        isAnomaly = Defender.currentState.ToString() == "Sliding_Tackle_Anomaly";
    }
    void Update()
    {
        if (Player && checkLimitOn)
        {
            /*if (!debugMessage) 
            {
                debugMessage = true;
                Debug.LogWarning("limit 확인중..");
            }*/

            StartCoroutine(CheckLimitCoroutine());
            
        }

    }

    // limit 확인 코루틴
    IEnumerator CheckLimitCoroutine()
    {
        // 이미 플레이어가 지나간 경우 종료
        if (DefenderTrans.position.z - PlayerTrans.position.z <= 0)
        {
            checkLimitOn = false;
            //debugMessage = debugMessage2 = false;
            //Debug.Log("limit 확인 종료");
            yield break;
        }

        // 플레이어가 다른라인에 있을 시 대기
        if (!isAnomaly && Mathf.Abs(PlayerTrans.position.x - DefenderTrans.position.x) > Player.distance / 2 - 0.5f)
        {
            /*if (!debugMessage2)
            {
                debugMessage2 = true;
                Debug.Log("limit 다른 라인 대기중");
            }*/
            
            yield return null;
        }

        // 특정 거리 안으로 들어올 시 limit 활성화
        if (transform.position.z - PlayerTrans.position.z < limitCheckDis)
        {
            checkLimitOn = false;

            // 선 스킬 사용 시 limit 강제종료
            if (Player.Instance.isAct) 
            {
                //debugMessage = debugMessage2 = false;
                //Debug.LogError("스킬 이미 사용 limit 강제종료");
                yield break;
            }

            //Debug.LogWarning("limit 활성화");
            ExtraScore.instance.LimitStart(this);

            yield return new WaitForSeconds(limitCheckTime);

            ExtraScore.instance.LimitEnd();

            //debugMessage = debugMessage2 = false;
            //Debug.LogWarning("limit 활성화 종료");
            yield break;
        }

        yield return null;
    }

    // limit 확인 시작
    public void CheckLimit()
    {
        //Debug.LogWarning("limit 확인 요청");
        checkLimitOn = true;
    }

    // 플레이어와 수비수 라인 확인
    public bool LineCheck()
    {
        // 특정 거리 이상 점수 인정x
        if (Mathf.Abs(PlayerTrans.position.x - DefenderTrans.position.x) > Player.distance / 2 - 0.5f)
        {
            return true;
        }

        return false;
    }

}
