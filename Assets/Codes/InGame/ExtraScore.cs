using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraScore : MonoBehaviour
{
    static float extraScore;

    public Player Player;
    public BallMove BallMove;

    bool checkScore;

    void Start()
    {

    }


    void Update()
    {
        if (checkScore)
            StartCoroutine(CheckingExtraScore());
    }



    IEnumerator CheckingExtraScore()
    {

        

        yield return null;
    }



}
