using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraScoreParticle : MonoBehaviour
{
    private void Update()
    {
        StartCoroutine(DestroyExtraScore());
    }

    IEnumerator DestroyExtraScore()
    {
        yield return new WaitForSeconds(0.8f);

        if (gameObject.name.Contains("SkillExtraParticle"))
            transform.GetChild(3).gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}
