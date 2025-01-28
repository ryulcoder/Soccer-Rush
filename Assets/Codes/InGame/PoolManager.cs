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

                    // 오브젝트 비활성화
                    obj.GetComponent<Defender>().Reset();
                    obj.gameObject.SetActive(false);

                    // 자식 설정
                    obj.SetParent(Parent);

                    // 선택적으로 위치와 회전을 초기화
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
                Debug.LogWarning("삭제! 삭제!!");
                Destroy(chilchild.gameObject);
            }

        }
    }

}
