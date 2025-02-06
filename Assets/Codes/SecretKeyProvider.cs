using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SecretKeyProvider : MonoBehaviour
{

    void Start()
    {
        Debug.Log($"Secret Key: {ScoreEnc.GetSecretKey()}");
    }
}
