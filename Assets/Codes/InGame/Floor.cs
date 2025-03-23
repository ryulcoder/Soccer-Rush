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
    bool onPlayer, inPlayer, coroutine, fpCoolTimeOn, setVar;
    int fpCount;


    void Start()
    {
        if (!setVar)
        {
            setVar = true;

            fpRanInt = 0;
            fpRangeInt = 1;
            leftGap = 0;
            prevX = -2;
            fpDistance = 0;
            useLeftGap = false;
            prevLineIs2 = false;
            fixedPattern = false;
        }

        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;

        floorScale = transform.localScale;
        posList = new List<Vector3>();

        defXs = new float[] { -floorScale.x / 3, 0, floorScale.x / 3 };

        // 시작시 첫 floor 제외 순차적 수비 세팅
        if (otherFloor)
        {
            StartCoroutine(FirstSettingCoroutine());
        }
    }

    void Update()
    {
        if (!Player.getTackled && fixedPattern && fpCoolTimeOn)
        {
            FixedPatternCoolTimeCheck();
        }

        // 플레이어가 해당 floor 다 지나갈시
        if (inPlayer && !onPlayer)
        {
            if (!Player.getTackled && !coroutine)
                StartCoroutine(DelayAndMoveTile());
        }
    }

    // 타일 이동 후 세팅준비
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

        PoolManager.PoolObjects(transform);

        yield return new WaitUntil(() => PoolManager.poolEnd);

        inPlayer = false;

        yield return StartCoroutine(SetDefenders());

        coroutine = false;

        yield break;
    }

    // 수비 세팅
    IEnumerator SetDefenders()
    {
        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;

        if (!fixedPattern)
        {
            fixedPattern = true;

            if (fpRanInt != 1 && Random.Range(0, fpRanInt) == 0)
            {
                fixedPattern = false;
                fpCoolTimeOn = false;
            }
            else
            {
                yield return StartCoroutine(FixedPatternSetDefends());

                if (fixedPattern)
                    yield break;
            }
        }

        List<GameObject> targetObjs = new();
        float[] ranXs;

        float targetPos = transform.TransformPoint(new(0, 0, -0.5f)).z;
        float MaxPos = targetPos + floorScale.z;

        float ranNum;

        // 새로운 수비 리스트 좌표 세팅
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

        // 세팅한 수비 좌표 리스트를 토대로 수비 옵젝 세팅
        for (int posIdx=0; posIdx < posList.Count; posIdx++)
        {
            ranXs = defXs.OrderBy(_ => Random.value).ToArray();

            string defStr = defNames[RanDef()];

            // 3라인 수비 무지개 패턴
            if (defStr == "Three_Defenders_Anomaly")
            {
                string[] orderDefNames = defNames[..3].OrderBy(_ => Random.value).ToArray();

                for (int i = 0; i < orderDefNames.Length; i++)
                {
                    PoolManager.SetPopObject(orderDefNames[i]);

                    yield return new WaitUntil(() => PoolManager.popEnd);

                    targetObjs.Add(PoolManager.PopObjcet);
                }
                    
            }
            // 3라인 수비 패턴
            else if (defStr == "Three_Defenders")
            {
                int ran = Random.Range(0, 2);

                defStr = defNames[ran];

                if (ran == 1)
                    posList[posIdx] += Vector3.forward * 10;

                for (int i = 0; i < 3; i++)
                {
                    PoolManager.SetPopObject(defStr);

                    yield return new WaitUntil(() => PoolManager.popEnd);

                    targetObjs.Add(PoolManager.PopObjcet);

                    
                }
                    
            }
            // 2라인 수비 패턴
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

                    PoolManager.SetPopObject(defStr);

                    yield return new WaitUntil(() => PoolManager.popEnd);

                    targetObjs.Add(PoolManager.PopObjcet);

                    
                }
            }
            // 1라인
            else
            {
                PoolManager.SetPopObject(defStr);

                yield return new WaitUntil(() => PoolManager.popEnd);

                targetObjs.Add(PoolManager.PopObjcet);

                
            }
                
            

            Vector3 settingVec;

            // 오브젝트pop 만약 프리팹인경우 복제 생성 후 정해진 위치 이동
            for (int i= 0;i < targetObjs.Count; i++)
            {
                // 2라인 수비 조정
                if (prevLineIs2 && targetObjs.Count == 2 && i == 1 && ranXs[0] != prevX && ranXs[1] != prevX)
                {
                    prevX = ranXs[1];
                    ranXs[1] = ranXs[2];
                    ranXs[2] = prevX;
                }

                // 1라인
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

    // 랜덤 수비 인덱스 
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
        int ranDir = Random.Range(1, 3);
        if (ranDir == 1) ranDir = 0;

        const int totalDef = 85;

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


        int fpRanNum;

        if (fpCount > 2 && fpRangeInt == 2)
            fpRanNum = Random.Range(1, fpRangeInt);
        else
            fpRanNum = Random.Range(0, fpRangeInt);

        // 45
        switch (fpRanNum)
        {
            // 1라인 블락 좌우 회피 전용 고정 패턴
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
                    if (i == 0 || i % 11 == 0)
                    {
                        PoolManager.SetPopObject("SlidingTackle_Anomaly");

                        yield return new WaitUntil(() => PoolManager.popEnd);

                        fpDefList.Add(PoolManager.PopObjcet);
                        posList.Add(new(defXs[fpXIdx], 0.5f, targetPos + 5 * i));
                        

                        if (fpXIdx == 1)
                            fpXIdx = otherXIdx;
                        else
                            fpXIdx = 1;

                        
                        
                    }

                    if (i == 0 || i % 8 == 0)
                    {
                        PoolManager.SetPopObject("StandTackle_Front");

                        yield return new WaitUntil(() => PoolManager.popEnd);

                        fpDefList.Add(PoolManager.PopObjcet);
                        posList.Add(new(defXs[ranDir], 0.5f, targetPos + 5 * i));
                        
                        
                    }
                }

                break;

            // 2라인 블락 가운데 스킬 회피 전용 고정 패턴
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
                    if (i == 0 || i % 14 == 0)
                    {
                        defRanIdx = Random.Range(0, 3);

                        // 변
                        if (defRanIdx == 2)
                        {
                            PoolManager.SetPopObject("SlidingTackle_Anomaly");

                            yield return new WaitUntil(() => PoolManager.popEnd);

                            fpDefList.Add(PoolManager.PopObjcet);
                            posList.Add(new(defXs[fpXIdx], 0.5f, targetPos + 5 * i));

                            PoolManager.SetPopObject("StandTackle_Front");

                            yield return new WaitUntil(() => PoolManager.popEnd);

                            fpDefList.Add(PoolManager.PopObjcet);
                            posList.Add(new(defXs[otherXIdx], 0.5f, targetPos + 5 * i));

                            (otherXIdx, fpXIdx) = (fpXIdx, otherXIdx);
                        }
                        // 슬
                        else if (defRanIdx == 1)
                        {
                            PoolManager.SetPopObject("SlidingTackle_Front");

                            yield return new WaitUntil(() => PoolManager.popEnd);

                            fpDefList.Add(PoolManager.PopObjcet);
                            posList.Add(new(defXs[1], 0.5f, targetPos + 5 * i + 40));
                        }
                        // 스
                        else
                        {
                            PoolManager.SetPopObject("StandTackle_Front");

                            yield return new WaitUntil(() => PoolManager.popEnd);

                            fpDefList.Add(PoolManager.PopObjcet);
                            posList.Add(new(defXs[1], 0.5f, targetPos + 5 * i));
                        }
                        
                    }

                    // 양 사이드 라인 블락
                    if (i == 0 || i % 7 == 0)
                    {
                        if (defRanIdx == 2)
                        {
                            defRanIdx = 0;
                        }
                        else
                        {
                            PoolManager.SetPopObject("StandTackle_Front");

                            yield return new WaitUntil(() => PoolManager.popEnd);

                            fpDefList.Add(PoolManager.PopObjcet);
                            posList.Add(new(defXs[0], 0.5f, targetPos + 5 * i));

                            
                            PoolManager.SetPopObject("StandTackle_Front");

                            yield return new WaitUntil(() => PoolManager.popEnd);

                            fpDefList.Add(PoolManager.PopObjcet);
                            posList.Add(new(defXs[2], 0.5f, targetPos + 5 * i));
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
        leftGap = 0;

        if (fpRanInt == 1) fpRanInt = 2;

        fpDistance = GameManager.ScoreCal.Distance;
        fpCoolTimeOn = true;
        fpCount++;
    }

    void FixedPatternCoolTimeCheck()
    {
        if (fpDistance + 100 < GameManager.ScoreCal.Distance)
        {
            if (fpRangeInt == 1) fpRangeInt = 2;

            fixedPattern = false;
            fpCoolTimeOn = false;
        }
    }

    

    // 시작시 첫 floor 제외 순차적 수비 세팅함수
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
