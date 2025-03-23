using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterGoInter : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;

    void Start()
    {
        inputField.onValueChanged.AddListener(UpdateButtonState);
        UpdateButtonState(inputField.text); // 초기 상태 업데이트
    }

    void UpdateButtonState(string text)
    {
        submitButton.interactable = text.Length >= 3;
    }
}
