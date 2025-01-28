using System.Collections;
using System.Collections.Generic;
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
    float[] defPer;

    [SerializeField] float minGap, maxGap; 
    bool onPlayer, inPlayer, coroutine, firstSet;

    private void Awake()
    {
        (minGap, maxGap) = GameManager.DefGap;
        defPer = GameManager.DefPer;
        floorScale = transform.localScale;
        posList = new List<Vector3>();
    }
    void Start()
    {
        if (otherFloor)
        {
            SetDefenders();
            otherFloor.FirstSetting();
        }
    }

    void Update()
    {
        if (inPlayer && !onPlayer)
        {
            if (!Player.getTackled && !coroutine)
                StartCoroutine(DelayAndMoveTile());
        }
    }

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

    void SetDefenders()
    {
        GameObject targetObj;

        float targetPos = transform.TransformPoint(new(0, 0, -0.5f)).z;
        float MaxPos = targetPos + floorScale.z;

        float ranNum;

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

                int ranX = 0;

                while (prevX == ranX)
                {
                    ranX = Random.Range(-1, 2);
                }

                prevX = ranX;

                posList.Add(new(ranX * floorScale.x / 3, 0.75f, targetPos));
            }


        }

        float prev = transform.TransformPoint(new(0, 0, -0.5f)).z;

        foreach (var pos in posList)
        {
            targetObj = PoolManager.PopObject(RanDef());

            if (PrefabUtility.GetPrefabAssetType(targetObj) != PrefabAssetType.NotAPrefab)
                targetObj = Instantiate(targetObj, pos, Quaternion.identity);
            else
                targetObj.transform.position = pos;

            targetObj.transform.SetParent(transform);
            targetObj.name = gameObject.name+" "+ (pos.z - prev);
            prev = pos.z;
            targetObj.SetActive(true);
        }

        posList.Clear();

    }


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
