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
        Debug.Log("�� �����");
    }
    IEnumerator CheckForUpdate()
    {
        //�� �ʱ�ȭ
        appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
        appUpdateManager.GetAppUpdateInfo();
        // �� ������ Ȯ�� �� ������ ��ٸ�
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            //������Ʈ�� ������ ����
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                //������ ������Ʈ ó�� ���
                var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                //��� ������Ʈ ó�� ���
                /*var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult,appUpdateOptions);
                yield return startUpdateRequest;*/

                //������Ʈ�� �Ϸ�� �� ���� ��ٸ�
                while (!startUpdateRequest.IsDone)
                {
                    if (startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        Debug.Log("������Ʈ �ٿ�ε尡 ���� ���Դϴ�.");
                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        Debug.Log("������Ʈ�� ������ �ٿ�ε�Ǿ����ϴ�.");
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
            //������Ʈ�� ���� ���
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("������Ʈ ����");
                loadingLogin.CheckNickName();
            }
            else
            {
                Debug.Log("���� ����");
                //yield return (int)UpdateAvailability.Unknown;
                loadingLogin.CheckNickName();

            }

        }
        else
        {
            // appUpdateInfoOperation.Error�� ����Ѵ�
            Debug.Log("Error��");
            loadingLogin.CheckNickName();

        }
    }
}
