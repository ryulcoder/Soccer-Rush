using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;// [Conditional]�� ����ϱ� ���� �ʿ�
using Debug = UnityEngine.Debug; 

public class SetGraphic : MonoBehaviour
{
    private static SetGraphic instance;

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
        if (QualitySettings.vSyncCount != 0)
            QualitySettings.vSyncCount = 0;     // ���� ����ȭ ����

        if (Application.targetFrameRate != 60)
            Application.targetFrameRate = 60;   // ������ 60���� ����

        Debug.LogWarning("��������ȭ ���� && ������ 60 ����");
    }
}
