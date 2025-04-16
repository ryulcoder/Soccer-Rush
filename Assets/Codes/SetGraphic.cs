using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;// [Conditional]�� ����ϱ� ���� �ʿ�
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
            Destroy(gameObject); // �ߺ� �߻� �� ����
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
        if (!isPaused) // �ٽ� ������ ��
        {
            ApplyGraphicsSettings();
        }
    }

    // �����Ϳ��� ������� �ʵ��� ����
    [Conditional("UNITY_EDITOR")]
    private void SkipInEditor()
    {
        Destroy(gameObject);
    }

    private void ApplyGraphicsSettings()
    {
        QualitySettings.vSyncCount = 0;     // ���� ����ȭ ����
        Application.targetFrameRate = 60;   // ������ 60���� ����

        Debug.LogWarning("��������ȭ ���� && ������ 60 ����");
    }
}
