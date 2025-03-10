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

        yield return appUpdateInfoOperation; // 비동기 결과 대기

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfo = appUpdateInfoOperation.GetResult();

            // 업데이트 가능 여부 확인
            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                // 즉시 업데이트 혹은 유연한 업데이트 실행
                StartCoroutine(StartUpdate(appUpdateInfo));
            }
        }
    }

    private IEnumerator StartUpdate(AppUpdateInfo appUpdateInfo)
    {
        var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(); // 즉시 업데이트

        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);

        yield return startUpdateRequest;

        if (startUpdateRequest.IsDone)
        {
            Debug.Log("업데이트 완료됨.");
        }
    }
}
