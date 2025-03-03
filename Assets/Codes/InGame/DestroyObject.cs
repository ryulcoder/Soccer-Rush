using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float time = 0.8f;

    private void Update()
    {
        StartCoroutine(DestroyObj());
    }

    IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(time);
        
        gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
