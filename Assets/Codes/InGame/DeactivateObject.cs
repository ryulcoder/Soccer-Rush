using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObject : MonoBehaviour
{
    public float time = 0.8f;


    private void Update()
    {
        StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
