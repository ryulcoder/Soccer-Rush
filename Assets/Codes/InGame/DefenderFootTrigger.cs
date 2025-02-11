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
                Debug.LogWarning("limit Ȯ����..");
            }*/

            StartCoroutine(CheckLimitCoroutine());
            
        }

    }

    // limit Ȯ�� �ڷ�ƾ
    IEnumerator CheckLimitCoroutine()
    {
        // �̹� �÷��̾ ������ ��� ����
        if (DefenderTrans.position.z - PlayerTrans.position.z <= 0)
        {
            checkLimitOn = false;
            //debugMessage = debugMessage2 = false;
            //Debug.Log("limit Ȯ�� ����");
            yield break;
        }

        // �÷��̾ �ٸ����ο� ���� �� ���
        if (!isAnomaly && Mathf.Abs(PlayerTrans.position.x - DefenderTrans.position.x) > Player.distance / 2 - 0.5f)
        {
            /*if (!debugMessage2)
            {
                debugMessage2 = true;
                Debug.Log("limit �ٸ� ���� �����");
            }*/
            
            yield return null;
        }

        // Ư�� �Ÿ� ������ ���� �� limit Ȱ��ȭ
        if (transform.position.z - PlayerTrans.position.z < limitCheckDis)
        {
            checkLimitOn = false;

            // �� ��ų ��� �� limit ��������
            if (Player.Instance.isAct) 
            {
                //debugMessage = debugMessage2 = false;
                //Debug.LogError("��ų �̹� ��� limit ��������");
                yield break;
            }

            //Debug.LogWarning("limit Ȱ��ȭ");
            ExtraScore.instance.LimitStart(this);

            yield return new WaitForSeconds(limitCheckTime);

            ExtraScore.instance.LimitEnd();

            //debugMessage = debugMessage2 = false;
            //Debug.LogWarning("limit Ȱ��ȭ ����");
            yield break;
        }

        yield return null;
    }

    // limit Ȯ�� ����
    public void CheckLimit()
    {
        //Debug.LogWarning("limit Ȯ�� ��û");
        checkLimitOn = true;
    }

    // �÷��̾�� ����� ���� Ȯ��
    public bool LineCheck()
    {
        // Ư�� �Ÿ� �̻� ���� ����x
        if (Mathf.Abs(PlayerTrans.position.x - DefenderTrans.position.x) > Player.distance / 2 - 0.5f)
        {
            return true;
        }

        return false;
    }

}
