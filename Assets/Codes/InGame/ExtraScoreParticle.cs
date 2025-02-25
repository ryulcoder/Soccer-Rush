using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraScoreParticle : MonoBehaviour
{
    public enum ScoreType
    {
        AvoidMove,
        AvoidSkill
    }

    public ScoreType scoreType;

    private void Update()
    {
        StartCoroutine(DestroyExtraScore());
    }

    IEnumerator DestroyExtraScore()
    {
        yield return new WaitForSeconds(0.8f);

        gameObject.SetActive(false);
    }
}
