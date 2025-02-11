using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraScoreUI : MonoBehaviour
{

    public string score;

    void Start()
    {
        GetComponent<Text>().text = score;

        StartCoroutine(DestroyExtraScore());
    }

    IEnumerator DestroyExtraScore()
    {
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
