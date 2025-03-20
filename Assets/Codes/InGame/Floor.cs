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

    static int fpRanInt = 0;
    static int fpRangeInt = 1;
    static float leftGap = 0;
    static float prevX = -2;
    static float fpDistance = 0;
    static bool useLeftGap, prevLineIs2, fixedPattern;

    Vector3 floorScale;
    float[] defXs;
    [SerializeField] float[] defPer;
    [SerializeField] string[] defNames = { "StandTackle_Front", "SlidingTackle_Front", "SlidingTackle_Anomaly", "Two_Defenders" , "Three_Defenders", "Three_Defenders_Anomaly" };

    [SerializeField] float minGap, maxGap;
    [SerializeField] bool onPlayer, inPlayer, coroutine, fpCoolTimeOn;
    

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
            StartCoroutine(FirstSettingCoroutine());
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

        if (fixedPattern && !fpCoolTimeOn)
        {
            fpCoolTimeOn = true;
            fpDistance = GameManager.ScoreCal.Distance;

            StartCoroutine(FixedPatternCoolTime());
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

        yield return StartCoroutine(SetDefenders());

        coroutine = false;

        //PoolManager.LeftObjectDestroy();

        yield break;
    }

    // ���� ����
    IEnumerator SetDefenders()
    {
        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;

        if (!fixedPattern)
        {
            yield return StartCoroutine(FixedPatternSetDefends());

            if (fixedPattern)
                yield break;
        }

        List<GameObject> targetObjs = new();
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

                posList.Add(new(0, 0.5f, targetPos));
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
                {
                    targetObjs.Add(PoolManager.PopObject(orderDefNames[i]));
                    yield return null;
                }
                    
            }
            // 3���� ���� ����
            else if (defStr == "Three_Defenders")
            {
                int ran = Random.Range(0, 2);

                defStr = defNames[ran];

                if (ran == 1)
                    posList[posIdx] += Vector3.forward * 10;

                for (int i = 0; i < 3; i++)
                {
                    targetObjs.Add(PoolManager.PopObject(defStr));
                    yield return null;
                }
                    
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

                    targetObjs.Add(PoolManager.PopObject(defStr));
                    yield return null;
                }
            }
            // 1����
            else
            {
                targetObjs.Add(PoolManager.PopObject(defStr));
                yield return null;
            }
                
            

            Vector3 settingVec;

            // ������Ʈpop ���� �������ΰ�� ���� ���� �� ������ ��ġ �̵�
            for (int i= 0;i < targetObjs.Count; i++)
            {
                // 2���� ���� ����
                if (prevLineIs2 && targetObjs.Count == 2 && i == 1 && ranXs[0] != prevX && ranXs[1] != prevX)
                {
                    prevX = ranXs[1];
                    ranXs[1] = ranXs[2];
                    ranXs[2] = prevX;
                }

                // 1����
                if (i == 0 && targetObjs.Count == 1)
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
                    
                targetObjs[i].transform.position = settingVec;
                targetObjs[i].transform.SetParent(transform);

                string[] nameStrs = targetObjs[i].name.Split("_");
                targetObjs[i].name = "Z(" +(posList[posIdx].z - prev) + ")_" + nameStrs[1].Replace("(clone)", "") + "_" + nameStrs[2].Split("(")[0];

                targetObjs[i].SetActive(true);
            }

            if (targetObjs.Count == 2)
            {
                prevLineIs2 = true;
                prevX = ranXs[2];
            }
            else
            {
                prevLineIs2 = false;
            }

            prev = posList[posIdx].z;
            targetObjs.Clear();
        }

        posList.Clear();
        yield break;
    }

    // ���� ���� �ε��� 
    int RanDef()
    {
        float ranNum = Random.value * 100;

        for (int i = 0; i < defPer.Length; i++)
        {
            if (ranNum - defPer[i] <= 0)
                return i;
            else
                ranNum -= defPer[i];

        }

        return defPer.Length - 1;
    }


    IEnumerator FixedPatternSetDefends()
    {
        fpCoolTimeOn = true;
        fixedPattern = true;

        if (fpRanInt != 1 && Random.Range(0, fpRanInt) == 0)
        {
            fixedPattern = false;
            fpCoolTimeOn = false;
            yield break;
        }

        fpCoolTimeOn = false;


        int ranDir = Random.Range(1, 3);
        if (ranDir == 1) ranDir = 0;

        const int totalDef = 45;

        List<GameObject> fpDefList = new();

        float targetPos = transform.TransformPoint(new(0, 0, -0.5f)).z;
        float ranNum = Random.Range(minGap, maxGap);
        int fpXIdx, otherXIdx = 0;

        if (!useLeftGap)
        {
            useLeftGap = true;
            ranNum -= leftGap;
        }

        targetPos += ranNum;

        // 45
        switch (Random.Range(0, fpRangeInt))
        {
            // 1���� ��� �¿� ȸ�� ���� ���� ����
            case 0:

                if (ranDir == 0)
                {
                    fpXIdx = Random.Range(1, 3);
                    otherXIdx = 2;
                }
                else
                {
                    fpXIdx = Random.Range(0, 2);
                    otherXIdx = 0;
                }

                for (int i = 0; i < totalDef; i++)
                {
                    if (i == 0 || i % 5 == 0)
                    {
                        fpDefList.Add(PoolManager.PopObject("SlidingTackle_Anomaly"));
                        posList.Add(new(defXs[fpXIdx], 0.5f, targetPos + 10 * i));

                        if (fpXIdx == 1)
                            fpXIdx = otherXIdx;
                        else
                            fpXIdx = 1;
                        yield return null;
                    }

                    if (i == 0 || i % 2 == 0)
                    {
                        fpDefList.Add(PoolManager.PopObject("StandTackle_Front"));
                        posList.Add(new(defXs[ranDir], 0.5f, targetPos + 10 * i));
                        yield return null;
                    }
                }

                break;

            // 2���� ��� ��� ��ų ȸ�� ���� ���� ����
            case 1:
                int defRanIdx = 0;

                fpXIdx = Random.Range(0, 2);
                if (fpXIdx == 1)
                {
                    fpXIdx = 2;
                    otherXIdx = 0;
                }
                else
                    otherXIdx = 2;


                for (int i = 0; i < totalDef; i++)
                {
                    if (i == 0 || i % 6 == 0)
                    {
                        defRanIdx = Random.Range(0, 3);

                        // ��
                        if (defRanIdx == 2)
                        {
                            fpDefList.Add(PoolManager.PopObject("SlidingTackle_Anomaly"));
                            posList.Add(new(defXs[fpXIdx], 0.5f, targetPos + 10 * i));
                            yield return null;
                        }
                        // ��
                        else if (defRanIdx == 1)
                        {
                            fpDefList.Add(PoolManager.PopObject("SlidingTackle_Front"));
                            posList.Add(new(defXs[1], 0.5f, targetPos + 10 * i + 40));
                            yield return null;
                        }
                        // ��
                        else
                        {
                            fpDefList.Add(PoolManager.PopObject("StandTackle_Front"));
                            posList.Add(new(defXs[1], 0.5f, targetPos + 10 * i));
                            yield return null;
                        }
                        
                    }

                    // �� ���̵� ���� ���
                    if (i == 0 || i % 2 == 0)
                    {
                        if (defRanIdx == 2)
                        {
                            defRanIdx = 0;

                            fpDefList.Add(PoolManager.PopObject("StandTackle_Front"));
                            posList.Add(new(defXs[otherXIdx], 0.5f, targetPos + 10 * i));
                            yield return null;

                            (otherXIdx, fpXIdx) = (fpXIdx, otherXIdx);
                        }
                        else
                        {
                            fpDefList.Add(PoolManager.PopObject("StandTackle_Front"));
                            posList.Add(new(defXs[0], 0.5f, targetPos + 10 * i));
                            yield return null;

                            fpDefList.Add(PoolManager.PopObject("StandTackle_Front"));
                            posList.Add(new(defXs[2], 0.5f, targetPos + 10 * i));
                            yield return null;
                        }
                    }
                }
                break;

            
        }


        for (int i = 0; i < fpDefList.Count; i++)
        {
            fpDefList[i].transform.position = posList[i];
            fpDefList[i].transform.SetParent(transform);

            string[] nameStrs = fpDefList[i].name.Split("_");
            fpDefList[i].name = "Z(" + posList[i].z + ")_" + nameStrs[1].Replace("(clone)", "") + "_" + nameStrs[2].Split("(")[0];

            fpDefList[i].SetActive(true);

        }

        posList.Clear();
        prevX = 0;
    }

    IEnumerator FixedPatternCoolTime()
    {
        yield return new WaitUntil(() => fpDistance + 200 < GameManager.ScoreCal.Distance);

        if (fpRangeInt == 1) fpRangeInt = 2;

        fpCoolTimeOn = false;
        fixedPattern = false;
    }

    

    // ���۽� ù floor ���� ������ ���� �����Լ�
    void FirstSetting()
    {
        StartCoroutine(SetDefenders());
    }

    IEnumerator FirstSettingCoroutine()
    {
        yield return StartCoroutine(SetDefenders());

        fpRanInt = 1;

        otherFloor.FirstSetting();
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
