using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GuestLogin : MonoBehaviour
{
    public void GuestLog()
    {
        PlayerPrefs.SetInt("FirstIn", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("InGame");
    }
}
