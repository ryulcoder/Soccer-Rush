using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurvedWorld : MonoBehaviour {

    public Vector3 Curvature = Vector3.right * 0.0005f + Vector3.up * 0.002f;

    int CurvatureID;

    private void OnEnable()
    {
        CurvatureID = Shader.PropertyToID("_Curvature");
    }

    void Update()
    {
        Shader.SetGlobalVector(CurvatureID, Curvature);
    }
}
