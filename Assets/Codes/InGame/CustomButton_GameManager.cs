using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class CustomButton_GameManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("[ Custom Button ]", EditorStyles.boldLabel);

        GameManager generator = (GameManager)target;

        if (GUILayout.Button("GameStart"))
        {
            generator.GameStart();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Speed Up"))
        {
            generator.GameSpeedUp();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Ball Trasform Reset"))
        {
            generator.BallReset();
        }

    }
}
