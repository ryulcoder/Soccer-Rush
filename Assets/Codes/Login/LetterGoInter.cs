using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class LetterGoInter : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;

    public GameObject invalidNamePanel;    // ºÎÀûÀýÇÑ ´Ð³×ÀÓ ¾Ë¸² UI
    public GameObject askAgain;    // ÃÖÁ¾ È®Á¤


    List<string> englishBannedWords = new List<string>
{
    "fuck", "fuxk", "f0ck", "fucc", "f@ck", "phuck",
    "shit", "sh1t", "sh!t", "s.h.i.t",
    "bitch", "b1tch", "b!tch", "biatch",
    "sex", "s3x", "secks", "sx", "sekks",
    "dick", "d1ck", "dyke",
    "ass", "a55", "@ss", "azz",
    "cunt", "kunt", "c*nt",
    "nazi", "n@zi", "n@z!",
    "nigger", "ni99er", "n1gger", "ni**er"
};

    List<string> koreanInitialBannedWords = new List<string>
{
    "¤µ¤²", "¤¶¤²", "¤²¤µ", "¤¸¤©", "¤¡¤µ¤¢", "¤±¤º", "¤±¤º¤¤", "¤¸", "¤·¤¡¤·"
};

    void Start()
    {

        // ¹öÆ°¿¡ ÀÌº¥Æ® ¿¬°á
        submitButton.onClick.AddListener(OnSubmitNickname);

        // ½ÃÀÛ ½Ã ¾Ë¸² UI´Â ²¨Á®ÀÖ°Ô
        invalidNamePanel.SetActive(false);

        inputField.onValueChanged.AddListener(UpdateButtonState);
        UpdateButtonState(inputField.text); // ÃÊ±â »óÅÂ ¾÷µ¥ÀÌÆ®

        inputField.onValueChanged.AddListener((word) => inputField.text = Regex.Replace(word, @"[^a-zA-Z]", ""));


    }

    void UpdateButtonState(string text)
    {
        submitButton.interactable = text.Length >= 3;
    }

    // ´Ð³×ÀÓ À¯È¿¼º °Ë»ç (ÃÖÁ¾ ÁøÀÔÁ¡)
    public bool IsValidNickname(string nickname)
    {
        string sanitized = Sanitize(nickname); // Æ¯¼ö¹®ÀÚ Á¦°Å + ¼Ò¹®ÀÚÈ­

        return !ContainsEnglishBannedWords(sanitized) && !ContainsKoreanInitials(nickname);
    }

    // 1. ¿µ¾î ¿å¼³ °Ë»ç (Á¤±ÔÈ­µÈ ´Ð³×ÀÓÀ¸·Î)
    private bool ContainsEnglishBannedWords(string normalizedNickname)
    {
        foreach (string word in englishBannedWords)
        {
            if (normalizedNickname.Contains(word))
                return true;
        }
        return false;
    }

    // 2. ÇÑ±Û ÃÊ¼º Á¶ÇÕ ÇÊÅÍ
    private bool ContainsKoreanInitials(string originalNickname)
    {
        string initials = ExtractKoreanInitials(originalNickname);
        foreach (string bad in koreanInitialBannedWords)
        {
            if (initials.Contains(bad))
                return true;
        }
        return false;
    }

    // Á¤±ÔÈ­: Æ¯¼ö¹®ÀÚ Á¦°Å + ¼Ò¹®ÀÚÈ­ (¿µ¾î ÇÊÅÍ¿ë)
    private string Sanitize(string input)
    {
        string onlyLetters = Regex.Replace(input, @"[^a-zA-Z0-9°¡-ÆR]", "");
        return onlyLetters.ToLower(); // ¿µ¾î ¼Ò¹®ÀÚ Ã³¸®
    }

    // ÇÑ±Û ÃÊ¼º ÃßÃâ
    private string ExtractKoreanInitials(string input)
    {
        string initials = "";
        foreach (char c in input)
        {
            if (c >= 0xAC00 && c <= 0xD7A3) // ÇÑ±Û À½ÀýÀÎÁö È®ÀÎ
            {
                int unicode = c - 0xAC00;
                int initialIndex = unicode / (21 * 28);
                initials += GetChoseongByIndex(initialIndex);
            }
        }
        return initials;
    }

    // ÃÊ¼º ÀÎµ¦½º ¡æ ¹®ÀÚ ¸ÅÇÎ
    private string GetChoseongByIndex(int index)
    {
        string[] choseong = {
            "¤¡","¤¢","¤¤","¤§","¤¨","¤©","¤±","¤²","¤³","¤µ",
            "¤¶","¤·","¤¸","¤¹","¤º","¤»","¤¼","¤½","¤¾"
        };
        return (index >= 0 && index < choseong.Length) ? choseong[index] : "";
    }


    void OnSubmitNickname()
    {
        string nickname = inputField.text;

        if (IsValidNickname(nickname))
        {
            // À¯È¿ÇÑ ´Ð³×ÀÓ
            Debug.Log("´Ð³×ÀÓ Çã¿ëµÊ: " + nickname);
            askAgain.SetActive(true);

            // ¿©±â¼­ ¼­¹ö¿¡ ´Ð³×ÀÓ Àü¼ÛÇÏ°Å³ª ´ÙÀ½ ´Ü°è ÁøÇà
        }
        else
        {
            // ±ÝÄ¢¾î Æ÷ÇÔµÈ ´Ð³×ÀÓ
            Debug.Log("ºÎÀûÀýÇÑ ´Ð³×ÀÓÀÔ´Ï´Ù: " + nickname);
            invalidNamePanel.SetActive(true);
            StartCoroutine(CloseAlert());
        }
    }

    IEnumerator CloseAlert()
    {
        yield return new WaitForSeconds(3f);
        invalidNamePanel.SetActive(false);

    }
}
