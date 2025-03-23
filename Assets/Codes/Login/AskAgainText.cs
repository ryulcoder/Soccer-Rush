using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AskAgainText : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public Text nicknameAgain; // ´Ù½Ã ¹¯±â

    private void OnEnable()
    {
        nicknameAgain.text = nickname.text + "?";
    }

}
