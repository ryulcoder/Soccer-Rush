using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurvedWorld : MonoBehaviour {

    public Vector3 Curvature = new Vector3(0, 0.0005f, 0);

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
