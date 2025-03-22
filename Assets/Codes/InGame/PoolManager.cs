using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] DefenderPrefabs;

    public GameObject PopObjcet;

    public bool popEnd, poolEnd;

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject def;

            for (int j = 0; j < 10; j++)
            {
                def = Instantiate(DefenderPrefabs[i], transform.GetChild(i));
                def.SetActive(false);
            }
        }

    }


    public void PoolObjects(Transform FloorTransform)
    {
        poolEnd = false;

        while (FloorTransform.childCount > 1)
        {
            Transform def = FloorTransform.GetChild(FloorTransform.childCount - 1);

            if (def.CompareTag("Untagged") || def.CompareTag("Floor")) break;

            for (int j = 0; j < transform.childCount; j++)
            {
                if (def.CompareTag(transform.GetChild(j).name))
                {
                    // 오브젝트 비활성화
                    def.GetComponent<Defender>().Reset();

                    // 자식 설정
                    def.SetParent(transform.GetChild(j));

                    // 선택적으로 위치와 회전을 초기화
                    //def.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    break;
                }
            }
        }


        for (int i = 0; i < FloorTransform.childCount; i++)
        {
            if (FloorTransform.GetChild(i).CompareTag("Untagged") || FloorTransform.GetChild(i).CompareTag("Floor")) continue;

            Transform def = FloorTransform.GetChild(i);

            for (int j = 0; j < transform.childCount; j++)
            {
                if (def.CompareTag(transform.GetChild(j).name))
                {
                    Debug.LogWarning(transform.GetChild(j).name);

                    // 오브젝트 비활성화
                    def.GetComponent<Defender>().Reset();

                    // 자식 설정
                    def.SetParent(transform.GetChild(j));

                    // 선택적으로 위치와 회전을 초기화
                    //def.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    break;
                }
            }
        }

        poolEnd = true;
    }

    public void SetPopObject(string tag)
    {
        popEnd = false;

        PopObjcet = null;
        Transform PopTransform = null;

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

    public bool WaitNextFrame()
    {
        return PopObjcet != null;
    }


    public void LeftObjectDestroy()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount == 0) continue;

            foreach (Transform chilchild in child)
            {
                Destroy(chilchild.gameObject);
            }

        }
    }

}
