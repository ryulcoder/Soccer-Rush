using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [Header("[ Code ]")]
    public GameManager GameManager;
    public PoolManager PoolManager;
    public Player Player;

    [Header("[ Floor ]")]
    public Floor otherFloor;

    public List<Vector3> posList;

    static float leftGap = 0;
    static float prevX = -2;
    static bool useLeftGap, prevLineIs2;

    Vector3 floorScale;
    float[] defXs;
    [SerializeField] float[] defPer;
    [SerializeField] string[] defNames = { "StandTackle_Front", "SlidingTackle_Front", "SlidingTackle_Anomaly", "Two_Defenders" , "Three_Defenders", "Three_Defenders_Anomaly" };

    [SerializeField] float minGap, maxGap;
    [SerializeField] bool onPlayer, inPlayer, coroutine, firstSet;

    void Awake()
    {
        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;

        floorScale = transform.localScale;
        posList = new List<Vector3>();

        defXs = new float[] { -floorScale.x / 3, 0, floorScale.x / 3 };
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

        yield return new WaitForSeconds(3);

        if (onPlayer || Player.getTackled) 
        {
            coroutine = false;
            yield break; 
        }

        transform.position += new Vector3(0, 0, transform.localScale.z * GameManager.Tiles.Length);

        PoolManager.PoolObjects(GetComponentsInChildren<Transform>());

        inPlayer = false;

        SetDefenders();

        coroutine = false;

        PoolManager.LeftObjectDestroy();

        yield break;
    }

    // ���� ����
    void SetDefenders()
    {
        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;

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
        for (int posIdx=0; posIdx < posList.Count; posIdx++)
        {
            ranXs = defXs.OrderBy(_ => Random.value).ToArray();

            string defStr = defNames[RanDef()];

            // 3���� ���� ������ ����
            if (defStr == "Three_Defenders_Anomaly")
            {
                string[] orderDefNames = defNames[..3].OrderBy(_ => Random.value).ToArray();

                for (int i = 0; i < orderDefNames.Length; i++)
                    targetObj.Add(PoolManager.PopObject(orderDefNames[i]));
            }
            // 3���� ���� ����
            else if (defStr == "Three_Defenders")
            {
                int ran = Random.Range(0, 2);

                defStr = defNames[ran];

                if (ran == 1)
                    posList[posIdx] += Vector3.forward * 10;

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
            {
                targetObj.Add(PoolManager.PopObject(defStr));
            }
                
            

            Vector3 settingVec;

            // ������Ʈpop ���� �������ΰ�� ���� ���� �� ������ ��ġ �̵�
            for (int i= 0;i < targetObj.Count; i++)
            {
                // 2���� ���� ����
                if (prevLineIs2 && targetObj.Count == 2 && i == 1 && ranXs[0] != prevX && ranXs[1] != prevX)
                {
                    prevX = ranXs[1];
                    ranXs[1] = ranXs[2];
                    ranXs[2] = prevX;
                }

                // 1����
                if (i == 0 && targetObj.Count == 1)
                {
                    if (!prevLineIs2 && prevX == ranXs[0])
                    {
                        prevX = ranXs[Random.Range(1, 3)];
                    }
                    else if (!prevLineIs2)
                    {
                        prevX = ranXs[0];
                    }

                    settingVec = new(prevX, posList[posIdx].y, posList[posIdx].z);
                }
                else
                {
                    settingVec = new(ranXs[i], posList[posIdx].y, posList[posIdx].z);
                }
                    
                targetObj[i].transform.position = settingVec;
                targetObj[i].transform.SetParent(transform);

                string[] nameStrs = targetObj[i].name.Split("_");
                targetObj[i].name = "Z(" +(posList[posIdx].z - prev) + ")_" + nameStrs[1].Replace("(clone)", "") + "_" + nameStrs[2].Split("(")[0];

                targetObj[i].SetActive(true);
            }

            if (targetObj.Count == 2)
            {
                prevLineIs2 = true;
                prevX = ranXs[2];
            }
            else
            {
                prevLineIs2 = false;
            }

            prev = posList[posIdx].z;
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
