using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSLoginChecker : MonoBehaviour
{
    public GameObject objectA; // 로그인 상태에서 활성화할 오브젝트
    public GameObject objectB; // 로그아웃 상태에서 활성화할 오브젝트

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
