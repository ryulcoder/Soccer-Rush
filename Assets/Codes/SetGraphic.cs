using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;// [Conditional]을 사용하기 위해 필요
using Debug = UnityEngine.Debug; 

public class SetGraphic : MonoBehaviour
{
    private static SetGraphic instance;

    static bool isWarmupShader;

    string prevScene;

    void Awake()
    {
        SkipInEditor();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 발생 시 제거
        }

        prevScene = SceneManager.GetActiveScene().name;

        if (!isWarmupShader)
        {
            isWarmupShader = true;
            Shader.WarmupAllShaders();
        }
            
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != prevScene)
        {
            prevScene = SceneManager.GetActiveScene().name;
            ApplyGraphicsSettings();
        }
            
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ApplyGraphicsSettings();
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        if (!isPaused) // 다시 복귀할 때
        {
            ApplyGraphicsSettings();
        }
    }

    // 에디터에서 실행되지 않도록 설정
    [Conditional("UNITY_EDITOR")]
    private void SkipInEditor()
    {
        Destroy(gameObject);
    }

    private void ApplyGraphicsSettings()
    {
        QualitySettings.vSyncCount = 0;     // 수직 동기화 해제
        Application.targetFrameRate = 60;   // 프레임 60으로 고정

        Debug.LogWarning("수직동기화 해제 && 프레임 60 고정");
    }
}
