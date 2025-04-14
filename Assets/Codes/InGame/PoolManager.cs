using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] DefenderPrefabs;
    public GameObject ImpactZone;

    GameObject PopObjcet;
    Transform def, PopTransform;

    public bool popEnd, poolEnd;

    int poolChildCount;

    void Awake()
    {
        poolChildCount = transform.childCount;

        for (int i = 0; i < poolChildCount; i++)
        {
            if (transform.GetChild(i).name == "ImpactZone") continue;

            GameObject defObj;

            for (int j = 0; j < 15; j++)
            {
                defObj = Instantiate(DefenderPrefabs[i], transform.GetChild(i));
                defObj.SetActive(false);
            }
        }

    }

    public void PoolObjects(Transform FloorTransform)
    {
        poolEnd = false;

        StartCoroutine(PoolCoroutine(FloorTransform));
    }

    IEnumerator PoolCoroutine(Transform FloorTransform)
    {
        while (FloorTransform.childCount > 1)
        {
            def = FloorTransform.GetChild(FloorTransform.childCount - 1);

            if (def.name == "ImpactZone")
            {
                def.SetParent(transform);

                break;
            }

            if (def.CompareTag("Untagged") || def.CompareTag("Floor")) break;

            for (int j = 0; j < poolChildCount; j++)
            {
                if (def.CompareTag(transform.GetChild(j).name))
                {
                    // 오브젝트 비활성화
                    def.GetComponent<Defender>().Reset();

                    // 자식 설정
                    def.SetParent(transform.GetChild(j));

                    // 선택적으로 위치와 회전을 초기화
                    //def.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    
                    yield return null;
                    break;
                }
            }
        }

        poolEnd = true;
        yield break;
    }

    

    public void SetPopObject(string tag)
    {
        popEnd = false;

        PopObjcet = null;
        PopTransform = null;

        if (tag == "ImpactZone")
        {
            PopObjcet = ImpactZone;
            popEnd = true;

            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == tag)
            {
                PopTransform = transform.GetChild(i);
                break;
            }
        }

        if (PopTransform && PopTransform.childCount == 0)
        {
            for (int i = 0; i < DefenderPrefabs.Length; i++)
            {
                if (DefenderPrefabs[i].CompareTag(tag))
                {
                    PopObjcet = Instantiate(DefenderPrefabs[i], PopTransform);
                }

            }
        }

        PopObjcet = PopTransform.GetChild(0).gameObject;
        PopObjcet.transform.SetParent(transform);

        popEnd = true;
    }

    public GameObject PopSettingObject()
    {
        return PopObjcet;
    }


}
