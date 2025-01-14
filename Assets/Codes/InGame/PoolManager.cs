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

                    // �ڽ� ����
                    obj.SetParent(transform);

                    // ���������� ��ġ�� ȸ���� �ʱ�ȭ
                    obj.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    // �ִϸ��̼� �ʱ�ȭ
                    break;
                }

            }
        }

        
    }

    public void PopObjects(string objTagName)
    {
        
    }

}
