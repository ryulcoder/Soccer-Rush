using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] DefenderPreFabs;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PoolObjects(Transform[] sendObject)
    {
        GameObject Parent;

        foreach (Transform obj in sendObject)
        {
            if (obj.CompareTag("Untagged")) continue;

            foreach (Transform child in transform)
            {

                if (obj.CompareTag(child.name))
                {
                    Parent = child.gameObject;

                    // 자식 설정
                    obj.SetParent(transform);

                    // 선택적으로 위치와 회전을 초기화
                    obj.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    // 애니메이션 초기화
                    break;
                }

            }
        }

        
    }

    public void PopObjects(string objTagName)
    {
        
    }

}
