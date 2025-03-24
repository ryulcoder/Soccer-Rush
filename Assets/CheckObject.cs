using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObject : MonoBehaviour
{
    public List<string> list = new ();
    
    
    void Start()
    {
        Canvas[] canvas = FindObjectsOfType<Canvas>();

        for (int i = 0; i < canvas.Length; i++)
        {
            list.Add(canvas[i].name);   
        }
    }


    public void CheckStart()
    {
        Debug.LogWarning("CheckStart");
        StartCoroutine(CheckCoroutine());
    }


    IEnumerator CheckCoroutine()
    {
        bool isCheck = true;

        while (isCheck)
        {
            isCheck = Check();

            Debug.LogWarning("Checking...");
            yield return null;
        }

        Canvas[] canvas = FindObjectsOfType<Canvas>();

        for (int i = 0; i < canvas.Length; i++)
        {
            if (!list.Contains(canvas[i].name))
            {
                DeepCheck(canvas[i].transform);
            }
        }

    }

    bool Check()
    {
        Canvas[] canvas = FindObjectsOfType<Canvas>();

        for (int i = 0; i < canvas.Length; i++)
        {
            if (!list.Contains(canvas[i].name))
            {
                return false;
            }
        }

        return true;
    }


    void DeepCheck(Transform obj)
    {
        Debug.LogWarning(obj.name);

        if (obj.transform.childCount != 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                DeepCheck(obj.transform.GetChild(i));
            }
        }
    }
}
