using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] DefenderPrefabs;
    public GameObject ImpactZone;

    GameObject PopObjcet;

    public bool popEnd, poolEnd;

    int poolChildCount;

    void Awake()
    {
        poolChildCount = transform.childCount;

        for (int i = 0; i < poolChildCount; i++)
        {
            if (transform.GetChild(i).name == "ImpactZone") continue;

            GameObject def;

            for (int j = 0; j < 15; j++)
            {
                def = Instantiate(DefenderPrefabs[i], transform.GetChild(i));
                def.SetActive(false);
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
            Transform def = FloorTransform.GetChild(FloorTransform.childCount - 1);

            if (def.name == "ImpactZone")
            {
                def.GetComponent<ImpactZone>().Off();
                def.SetParent(transform);

                break;
            }

            if (def.CompareTag("Untagged") || def.CompareTag("Floor")) break;

            for (int j = 0; j < poolChildCount; j++)
            {
                if (def.CompareTag(transform.GetChild(j).name))
                {
                    // ������Ʈ ��Ȱ��ȭ
                    def.GetComponent<Defender>().Reset();

                    // �ڽ� ����
                    def.SetParent(transform.GetChild(j));

                    // ���������� ��ġ�� ȸ���� �ʱ�ȭ
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
        Transform PopTransform = null;

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

        //StartCoroutine (PopCoroutine(tag));
    }

    public GameObject PopSettingObject()
    {
        return PopObjcet;
    }






    /*IEnumerator PopCoroutine(string tag) 
    {
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
                    yield return null;
                }

            }
        }

        PopObjcet = PopTransform.GetChild(0).gameObject;
        PopObjcet.transform.SetParent(transform);
        yield return null;

        popEnd = true;
    }*/

   /* public void LeftObjectDestroy()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount == 0) continue;

            foreach (Transform chilchild in child)
            {
                Destroy(chilchild.gameObject);
            }

        }
    }*/

}
