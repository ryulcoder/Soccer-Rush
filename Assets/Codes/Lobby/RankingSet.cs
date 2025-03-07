using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingSet : MonoBehaviour
{
    void Start()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log(Social.localUser.id);
                Debug.Log("¿¬°áµÊ");
            }
            else
            {
            }
        });
    }
}
