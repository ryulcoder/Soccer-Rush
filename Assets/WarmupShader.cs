using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmupShader : MonoBehaviour
{
    
    void Start()
    {
        Shader.WarmupAllShaders();
    }

}
