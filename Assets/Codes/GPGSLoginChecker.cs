using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSLoginChecker : MonoBehaviour
{
    public GameObject objectA; // �α��� ���¿��� Ȱ��ȭ�� ������Ʈ
    public GameObject objectB; // �α׾ƿ� ���¿��� Ȱ��ȭ�� ������Ʈ

    void Start()
    {
        CheckLoginStatus();
    }

    void CheckLoginStatus()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            objectA.SetActive(true);
            objectB.SetActive(false);
        }
        else
        {
            objectA.SetActive(false);
            objectB.SetActive(true);
        }
    }

}
