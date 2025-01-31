using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartStage : MonoBehaviour
{
    public StageSlide stageSlide;
    private int stageIndex;

    // Start is called before the first frame update
    void Start()
    {
        stageIndex = stageSlide.currentStage;
        Debug.Log(stageIndex);
        SceneManager.LoadScene("InGame");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
