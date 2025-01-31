using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public GameManager GameManager;
    public PoolManager PoolManager;
    public Player Player;

    public Floor otherFloor;

    public List<Vector3> posList;

    static float leftGap = 0;
    static float prevX = 0;
    static bool useLeftGap;

    Vector3 floorScale;
    float[] defXs;
    [SerializeField] float[] defPer;
    [SerializeField] string[] defNames = { "StandTackle_Front", "SlidingTackle_Front", "SlidingTackle_Anomaly", "Two_Defenders" , "Three_Defenders" };

    [SerializeField] float minGap, maxGap; 
    bool onPlayer, inPlayer, coroutine, firstSet;

    private void Awake()
    {
        floorScale = transform.localScale;
        posList = new List<Vector3>();

        defXs = new float[] { -floorScale.x / 3, 0, floorScale.x / 3 };

        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;
    }
    void Start()
    {
        // ���۽� ù floor ���� ������ ���� ����
        if (otherFloor)
        {
            SetDefenders();
            otherFloor.FirstSetting();
        }
    }

    void Update()
    {
        // �÷��̾ �ش� floor �� ��������
        if (inPlayer && !onPlayer)
        {
            if (!Player.getTackled && !coroutine)
                StartCoroutine(DelayAndMoveTile());
        }
    }

    // Ÿ�� �̵� �� �����غ�
    IEnumerator DelayAndMoveTile()
    {
        coroutine = true;

        PoolManager.PoolObjects(GetComponentsInChildren<Transform>());

        yield return new WaitForSeconds(3);

        if (onPlayer || Player.getTackled) yield break;

        transform.position += new Vector3(0, 0, transform.localScale.z * GameManager.Tiles.Length);

        inPlayer = false;

        SetDefenders();

        coroutine = false;

        PoolManager.LeftObjectDestroy();
    }

    // ���� ����
    void SetDefenders()
    {
        List<GameObject> targetObj = new();
        float[] ranXs;

        float targetPos = transform.TransformPoint(new(0, 0, -0.5f)).z;
        float MaxPos = targetPos + floorScale.z;

        float ranNum;

        // ���ο� ���� ����Ʈ ��ǥ ����
        while (true)
        {
            if (!useLeftGap)
            {
                useLeftGap = true;
                ranNum = Random.Range(minGap, maxGap) - leftGap;
            }
            else
                ranNum = Random.Range(minGap, maxGap);


            if (targetPos + ranNum > MaxPos)
            {
                leftGap = MaxPos - targetPos;
                useLeftGap = false;
                break;
            }
            else
            {
                targetPos += ranNum;

                posList.Add(new(0, 0.75f, targetPos));
            }

        }

        float prev = transform.TransformPoint(new(0, 0, -0.5f)).z;

        // ������ ���� ��ǥ ����Ʈ�� ���� ���� ���� ����
        foreach (Vector3 pos in posList)
        {
            ranXs = defXs.OrderBy(_ => Random.value).ToArray();
            string defStr = defNames[RanDef()];
            
            // 3���� ���� ����
            if (defStr == "Three_Defenders")
            {
                defStr = defNames[1];//defNames[Random.Range(0, 2)];

                for (int i = 0; i < 3; i++)
                    targetObj.Add(PoolManager.PopObject(defStr));

            }
            // 2���� ���� ����
            else if (defStr == "Two_Defenders")
            {
                for (int i = 0; i < 2; i++)
                {
                    if (ranXs[0] != 0 && ranXs[1] != 0)
                    {
                        if (defStr == "SlidingTackle_Anomaly")
                            defStr = defNames[Random.Range(0, 2)];
                        else
                            defStr = defNames[Random.Range(0, 3)];
                    }
                    else
                        defStr = defNames[Random.Range(0, 2)];

                    targetObj.Add(PoolManager.PopObject(defStr));
                }
            }
            // 1����
            else
                targetObj.Add(PoolManager.PopObject(defStr));

            // ������Ʈpop Ȥ�� �������ΰ�� ���� ���� �� ������ ��ġ �̵�
            for (int i= 0;i < targetObj.Count; i++)
            {
                Vector3 vec = new(targetObj.Count == 1 ? ranXs[i+1] : ranXs[i], pos.y, pos.z);

                targetObj[i].transform.position = vec;

                targetObj[i].transform.SetParent(transform);
                targetObj[i].name = gameObject.name + " " + (pos.z - prev);

                targetObj[i].SetActive(true);
            }

            prev = pos.z;
            targetObj.Clear();
        }

        posList.Clear();

    }


    // ���� ���� �ε��� 
    int RanDef()
    {
        float ranNum = Random.value * 100;

        for (int i = 0;i < defPer.Length; i++) 
        {
            if (ranNum - defPer[i] <= 0)
                return i;
            else
                ranNum -= defPer[i];

        }

        return defPer.Length - 1;

    }

    // ���۽� ù floor ���� ������ ���� �����Լ�
    public void FirstSetting()
    {
        if (!firstSet)
        {
            firstSet = true;
            SetDefenders();
        }
            
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == "PlayerFoot")
        {
            if (!inPlayer)
                inPlayer = true;

            onPlayer = true;

            return;
        }


    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name != "PlayerFoot") return;

        onPlayer = false;
    }

}
