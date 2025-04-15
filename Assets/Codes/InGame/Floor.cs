using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [Header("[ Code ]")]
    public GameManager GameManager;
    public PoolManager PoolManager;
    public Player Player;

    [Header("[ Floor ]")]
    public Floor otherFloor;

    public List<Vector3> posList = new();

    static int fpRanInt = 0;
    static int fpRangeInt = 1;
    static float leftGap = 0;
    static float prevX = -2;
    static float fpDistance = 0;
    static bool useLeftGap, prevLineIs2, fixedPattern;

    static string[] defNames = { "StandTackle_Front", "SlidingTackle_Front", "SlidingTackle_Anomaly", "Two_Defenders", "Three_Defenders", "Three_Defenders_Anomaly" };

    [SerializeField] float[] defPer;
    [SerializeField] float minGap, maxGap;

    string[] orderDefNames = { "StandTackle_Front", "SlidingTackle_Front", "SlidingTackle_Anomaly" };

    Vector3 floorScale, settingVec;

    [SerializeField]float[] defXs, ranXs;

    bool onPlayer, inPlayer, coroutine, fpCoolTimeOn, setVar;
    int fpCount, ran, ranDir, fpXIdx, otherXIdx, fpRanNum, defRanIdx;
    float targetPos, ranNum, rNum, MaxPos;
    string defStr;

    List<GameObject> targetObjs = new();

    const int totalDef = 85;

    void Start()
    {
        if (!setVar)
        {
            setVar = true;

            fpRanInt = 0;
            fpRangeInt = 1;
            leftGap = 0;
            prevX = -2;
            fpDistance = 100;
            useLeftGap = false;
            prevLineIs2 = false;
            fixedPattern = true;
            fpCoolTimeOn = true;
        }

        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;

        floorScale = transform.localScale;
        defXs = new float[] { -floorScale.x / 3, 0, floorScale.x / 3 };
        ranXs = new float[] { -floorScale.x / 3, 0, floorScale.x / 3 };

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

        transform.position += transform.localScale.z * GameManager.Tiles.Length * Vector3.forward;

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
        targetObjs.Clear();

        if (GameManager.IsImpact)
        {
            PoolManager.SetPopObject("ImpactZone");

            targetObjs.Add(PoolManager.PopSettingObject());

            targetObjs[0].transform.SetParent(transform, true);
            targetObjs[0].transform.localPosition = Vector3.zero;
            targetObjs[0].SetActive(true);

            targetObjs.Clear();
            yield break;
        }

        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;

        if (!fixedPattern)
        {
            fixedPattern = true;

            FixedPatternSetDefends();

            if (fixedPattern)
                yield break;
        }

        targetPos = transform.TransformPoint(0.5f * Vector3.back).z;
        MaxPos = targetPos + floorScale.z;

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

                posList.Add(Vector3.up * 0.5f + Vector3.forward * targetPos);
            }

        }

        // 세팅한 수비 좌표 리스트를 토대로 수비 옵젝 세팅
        for (int posIdx=0; posIdx < posList.Count; posIdx++)
        {
            ranXs = Shuffle(ranXs);

            defStr = defNames[RanDef()];

            // 3라인 수비 무지개 패턴
            if (defStr == "Three_Defenders_Anomaly")
            {
                orderDefNames = Shuffle(orderDefNames);

                for (int i = 0; i < orderDefNames.Length; i++)
                {
                    PoolManager.SetPopObject(orderDefNames[i]);
                    targetObjs.Add(PoolManager.PopSettingObject());
                }
                    
            }
            // 3라인 수비 패턴
            else if (defStr == "Three_Defenders")
            {
                ran = Random.Range(0, 2);

                defStr = defNames[ran];

                if (ran == 1)
                    posList[posIdx] += Vector3.forward * 10;

                for (int i = 0; i < 3; i++)
                {
                    PoolManager.SetPopObject(defStr);
                    targetObjs.Add(PoolManager.PopSettingObject());
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
                    targetObjs.Add(PoolManager.PopSettingObject());
                }
            }
            // 1라인
            else
            {
                PoolManager.SetPopObject(defStr);
                targetObjs.Add(PoolManager.PopSettingObject());
            }
                

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

                    settingVec = posList[posIdx];
                    settingVec.x = prevX;
                }
                else
                {
                    settingVec = posList[posIdx];
                    settingVec.x = ranXs[i];
                }
                    
                targetObjs[i].transform.position = settingVec;
                targetObjs[i].transform.SetParent(transform);
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

            targetObjs.Clear();
        }

        posList.Clear();
        yield break;
    }

    // 랜덤 수비 인덱스 
    int RanDef()
    {
        rNum = Random.value * 100;

        for (int i = 0; i < defPer.Length; i++)
        {
            if (rNum - defPer[i] <= 0)
                return i;
            else
                rNum -= defPer[i];

        }

        return defPer.Length - 1;
    }


    void FixedPatternSetDefends()
    {
        targetPos = transform.TransformPoint(0.5f * Vector3.back).z;
        float ranNum = Random.Range(minGap, maxGap);

        if (!useLeftGap)
        {
            useLeftGap = true;
            ranNum -= leftGap;
        }

        targetPos += ranNum;

        if (fpCount < 2 && fpRangeInt == 2)
            fpRanNum = Random.Range(1, fpRangeInt);
        else
            fpRanNum = Random.Range(0, fpRangeInt);

        // 45
        switch (fpRanNum)
        {
            // 1라인 블락 좌우 회피 전용 고정 패턴
            case 0:

                ranDir = Random.Range(1, 3);
                if (ranDir == 1) ranDir = 0;

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
                    if (i != 0 && i % 11 == 0)
                    {
                        PoolManager.SetPopObject("SlidingTackle_Anomaly");
                        targetObjs.Add(PoolManager.PopSettingObject());

                        posList.Add(Vector3.right * defXs[fpXIdx] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i));
                        

                        if (fpXIdx == 1)
                            fpXIdx = otherXIdx;
                        else
                            fpXIdx = 1;
                    }

                    if (i != 0 && i % 8 == 0)
                    {
                        PoolManager.SetPopObject("StandTackle_Front");
                        targetObjs.Add(PoolManager.PopSettingObject());

                        posList.Add(Vector3.right * defXs[ranDir] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i));
                    }
                }

                break;

            // 2라인 블락 가운데 스킬 회피 전용 고정 패턴
            case 1:
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
                    if (i != 0 && i % 14 == 0)
                    {
                        defRanIdx = Random.Range(0, 3);

                        // 변
                        if (defRanIdx == 2)
                        {
                            PoolManager.SetPopObject("SlidingTackle_Anomaly");
                            targetObjs.Add(PoolManager.PopSettingObject());

                            posList.Add(Vector3.right * defXs[fpXIdx] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i));

                            PoolManager.SetPopObject("StandTackle_Front");
                            targetObjs.Add(PoolManager.PopSettingObject());

                            posList.Add(Vector3.right * defXs[otherXIdx] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i));

                            (otherXIdx, fpXIdx) = (fpXIdx, otherXIdx);
                        }
                        // 슬
                        else if (defRanIdx == 1)
                        {
                            PoolManager.SetPopObject("SlidingTackle_Front");
                            targetObjs.Add(PoolManager.PopSettingObject());

                            posList.Add(Vector3.right * defXs[1] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i + 40));
                        }
                        // 스
                        else
                        {
                            PoolManager.SetPopObject("StandTackle_Front");
                            targetObjs.Add(PoolManager.PopSettingObject());

                            posList.Add(Vector3.right * defXs[1] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i));
                        }
                        
                    }

                    // 양 사이드 라인 블락
                    if (i != 0 && i % 7 == 0)
                    {
                        if (defRanIdx == 2)
                        {
                            defRanIdx = 0;
                        }
                        else
                        {
                            PoolManager.SetPopObject("StandTackle_Front");
                            targetObjs.Add(PoolManager.PopSettingObject());

                            posList.Add(Vector3.right * defXs[0] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i));

                            
                            PoolManager.SetPopObject("StandTackle_Front");
                            targetObjs.Add(PoolManager.PopSettingObject());

                            posList.Add(Vector3.right * defXs[2] + Vector3.up * 0.5f + Vector3.forward * (targetPos + 5 * i));
                        }
                    }
                }
                break;

            
        }


        for (int i = 0; i < targetObjs.Count; i++)
        {
            targetObjs[i].transform.position = posList[i];
            targetObjs[i].transform.SetParent(transform);
            targetObjs[i].SetActive(true);

        }

        posList.Clear();
        leftGap = 0;

        if (fpRanInt == 1) fpRanInt = 2;

        
        fpCoolTimeOn = true;
        fpCount++;
    }

    void FixedPatternCoolTimeCheck()
    {
        if (fpDistance + 200 <= GameManager.ScoreCal.Distance + 50)
        {

            fpDistance += 200;

            if (fpRangeInt == 1 && fpCount == 1) fpRangeInt = 2;

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
        if (collider.gameObject.name == "PlayerFoot" )
        {
            if (!inPlayer)
                inPlayer = true;

            onPlayer = true;
        }


    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "PlayerFoot") {

            onPlayer = false;
        }

        
    }


    public static T[] Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[j], array[i]) = (array[i], array[j]);
        }

        return array;
    }
}
