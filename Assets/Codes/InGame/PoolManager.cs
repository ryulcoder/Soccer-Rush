using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] DefenderPrefabs;

    public void PoolObjects(Transform[] sendObject)
    {
        Transform Parent;

        foreach (Transform obj in sendObject)
        {
            if (obj.CompareTag("Untagged") || obj.CompareTag("Floor")) continue;

            foreach (Transform child in transform)
            {

                if (obj.CompareTag(child.name))
                {
                    Parent = child.gameObject.transform;

                    // ������Ʈ ��Ȱ��ȭ
                    obj.GetComponent<Defender>().Reset();
                    obj.gameObject.SetActive(false);

                    // �ڽ� ����
                    obj.SetParent(Parent);

                    // ���������� ��ġ�� ȸ���� �ʱ�ȭ
                    obj.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    break;
                }

            }
        }

        
    }

    public GameObject PopObject(int defIdx)
    {
        Transform targetParent = transform.GetChild(defIdx);

        if (targetParent.childCount != 0)
        {
            return targetParent.GetChild(0).gameObject;
        }

        return DefenderPrefabs[defIdx];
    }

    public void LeftObjectDestroy()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount == 0) continue;

            foreach (Transform chilchild in child)
            {
                Debug.LogWarning("����! ����!!");
                Destroy(chilchild.gameObject);
            }

        }
    }

}
