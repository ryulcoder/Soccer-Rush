using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine;

public class InAppUpdateManager : MonoBehaviour
{
    AppUpdateManager appUpdateManager;
    public LoadingLogin loadingLogin;

    private void Start()
    {

        StartCoroutine(CheckForUpdate());
        Debug.Log("나 실행됨");
    }
    IEnumerator CheckForUpdate()
    {
        //앱 초기화
        appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
        appUpdateManager.GetAppUpdateInfo();
        // 앱 정보가 확인 될 때까지 기다림
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            //업데이트가 가능한 상태
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                //유연한 업데이트 처리 방법
                var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                //즉시 업데이트 처리 방법
                /*var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult,appUpdateOptions);
                yield return startUpdateRequest;*/

                //업데이트가 완료될 때 까지 기다림
                while (!startUpdateRequest.IsDone)
                {
                    if (startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        Debug.Log("업데이트 다운로드가 진행 중입니다.");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        Debug.Log("업데이트가 완전히 다운로드되었습니다.");
                    }
                    yield return null;
                }
                var result = appUpdateManager.CompleteUpdate();
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }
                //yield return (int)startUpdateRequest.Status;
                loadingLogin.CheckNickName();
            }
            //업데이트가 없는 경우
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("업데이트 없음");
                loadingLogin.CheckNickName();
            }
            else
            {
                Debug.Log("정보 없음");
                //yield return (int)UpdateAvailability.Unknown;
                loadingLogin.CheckNickName();

            }

        }
        else
        {
            // appUpdateInfoOperation.Error를 기록한다
            Debug.Log("Error를");
            loadingLogin.CheckNickName();

        }
    }
}
