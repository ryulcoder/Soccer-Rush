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

    public GameObject invalidNamePanel;    // �������� �г��� �˸� UI
    public GameObject askAgain;    // ���� Ȯ��


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
    "����", "����", "����", "����", "������", "����", "������", "��", "������"
};

    void Start()
    {

        // ��ư�� �̺�Ʈ ����
        submitButton.onClick.AddListener(OnSubmitNickname);

        // ���� �� �˸� UI�� �����ְ�
        invalidNamePanel.SetActive(false);

        inputField.onValueChanged.AddListener(UpdateButtonState);
        UpdateButtonState(inputField.text); // �ʱ� ���� ������Ʈ

        inputField.onValueChanged.AddListener((word) => inputField.text = Regex.Replace(word, @"[^a-zA-Z]", ""));


    }

    void UpdateButtonState(string text)
    {
        submitButton.interactable = text.Length >= 3;
    }

    // �г��� ��ȿ�� �˻� (���� ������)
    public bool IsValidNickname(string nickname)
    {
        string sanitized = Sanitize(nickname); // Ư������ ���� + �ҹ���ȭ

        return !ContainsEnglishBannedWords(sanitized) && !ContainsKoreanInitials(nickname);
    }

    // 1. ���� �弳 �˻� (����ȭ�� �г�������)
    private bool ContainsEnglishBannedWords(string normalizedNickname)
    {
        foreach (string word in englishBannedWords)
        {
            if (normalizedNickname.Contains(word))
                return true;
        }
        return false;
    }

    // 2. �ѱ� �ʼ� ���� ����
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

    // ����ȭ: Ư������ ���� + �ҹ���ȭ (���� ���Ϳ�)
    private string Sanitize(string input)
    {
        string onlyLetters = Regex.Replace(input, @"[^a-zA-Z0-9��-�R]", "");
        return onlyLetters.ToLower(); // ���� �ҹ��� ó��
    }

    // �ѱ� �ʼ� ����
    private string ExtractKoreanInitials(string input)
    {
        string initials = "";
        foreach (char c in input)
        {
            if (c >= 0xAC00 && c <= 0xD7A3) // �ѱ� �������� Ȯ��
            {
                int unicode = c - 0xAC00;
                int initialIndex = unicode / (21 * 28);
                initials += GetChoseongByIndex(initialIndex);
            }
        }
        return initials;
    }

    // �ʼ� �ε��� �� ���� ����
    private string GetChoseongByIndex(int index)
    {
        string[] choseong = {
            "��","��","��","��","��","��","��","��","��","��",
            "��","��","��","��","��","��","��","��","��"
        };
        return (index >= 0 && index < choseong.Length) ? choseong[index] : "";
    }


    void OnSubmitNickname()
    {
        string nickname = inputField.text;

        if (IsValidNickname(nickname))
        {
            // ��ȿ�� �г���
            Debug.Log("�г��� ����: " + nickname);
            askAgain.SetActive(true);

            // ���⼭ ������ �г��� �����ϰų� ���� �ܰ� ����
        }
        else
        {
            // ��Ģ�� ���Ե� �г���
            Debug.Log("�������� �г����Դϴ�: " + nickname);
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
