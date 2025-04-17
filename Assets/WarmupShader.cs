using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmupShader : MonoBehaviour
{

    static bool isWarmup;

    void Start()
    {

#if !UNITY_EDITOR
        if (!isWarmup)
        {
            isWarmup = true;

            Shader.WarmupAllShaders();
        }
#endif
            
    }

}
