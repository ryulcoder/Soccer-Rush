using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageControl : MonoBehaviour
{
    public StageSlide stageSlide;
    public int currentStage;

    void Start()
    {
        currentStage = PlayerPrefs.GetInt("currentStage");
        stageSlide.currentStage = currentStage;
        stageSlide.StageSet();
    }
}
