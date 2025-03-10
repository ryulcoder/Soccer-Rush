using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine;

public class InAppUpdateManager : MonoBehaviour
{
    private AppUpdateManager appUpdateManager;

    private void Start()
    {
        appUpdateManager = new AppUpdateManager();
        StartCoroutine(CheckForUpdate());
    }

    private IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            appUpdateManager.GetAppUpdateInfo();

        yield return appUpdateInfoOperation; // �񵿱� ��� ���

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfo = appUpdateInfoOperation.GetResult();

            // ������Ʈ ���� ���� Ȯ��
            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                // ��� ������Ʈ Ȥ�� ������ ������Ʈ ����
                StartCoroutine(StartUpdate(appUpdateInfo));
            }
        }
    }

    private IEnumerator StartUpdate(AppUpdateInfo appUpdateInfo)
    {
        var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(); // ��� ������Ʈ

        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);

        yield return startUpdateRequest;

        if (startUpdateRequest.IsDone)
        {
            Debug.Log("������Ʈ �Ϸ��.");
        }
    }
}
