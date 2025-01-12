using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataConnectGP : MonoBehaviour
{
    public Text showText;

    public void SaveData(int stageNeedSave) // 외부에서 클리어 스테이지 넘겨줌
    {
        OpenSaveGame(stageNeedSave); 
    }

    //저장 메서드
    private void OpenSaveGame(int dataToSave)
    {
        ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;

        saveGameClient.OpenWithAutomaticConflictResolution("currentStage",
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood,
            (status, game) => onsavedGameOpend(status, game, dataToSave)); // 저장할 데이터도 전달
    }

    private void onsavedGameOpend(SavedGameRequestStatus status, ISavedGameMetadata game, int dataToSave)
    {
        ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            var update = new SavedGameMetadataUpdate.Builder().Build();

            // int 데이터를 JSON으로 변환
            var json = JsonUtility.ToJson(new NeedSaveData { myIntValue = dataToSave });
            byte[] data = Encoding.UTF8.GetBytes(json);

            saveGameClient.CommitUpdate(game, update, data, OnSavedGameWritten);
        }
        else
        {
            Debug.Log("Save No.....");
        }
    }

    // 저장 완료 확인
    private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Save End");
        }
        else
        {
            Debug.Log("Save nonononononono...");
        }
    }


    // 스테이지 데이터 로드
    public void LoadData()
    {
        ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;

        saveGameClient.OpenWithAutomaticConflictResolution("currentStage",
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood,
            (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    saveGameClient.ReadBinaryData(game, (readStatus, data) =>
                    {
                        if (readStatus == SavedGameRequestStatus.Success)
                        {
                            string json = Encoding.UTF8.GetString(data);
                            NeedSaveData loadedData = JsonUtility.FromJson<NeedSaveData>(json);
                            Debug.Log("Loaded int value: " + loadedData.myIntValue);
                            PlayerPrefs.SetInt("currentStage", loadedData.myIntValue);
                            PlayerPrefs.Save();


                            if (showText != null) 
                                showText.text = loadedData.myIntValue.ToString();

                            SceneManager.LoadScene("Lobby");

                        }
                        else
                        {
                            Debug.Log("Failed to load data.");
                            if (showText != null)
                                showText.text = "Failed to load data.";
                            SceneManager.LoadScene("Lobby");

                        }
                    });
                }
            });
    }

    // JSON 변환용 클래스 정의
    [System.Serializable]
    public class NeedSaveData
    {
        public int myIntValue;
    }
}
