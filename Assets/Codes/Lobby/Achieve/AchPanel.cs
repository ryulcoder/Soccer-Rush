using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchPanel : MonoBehaviour
{
    public AchManager manager;

    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI topDistance;
    public TextMeshProUGUI breakThrough;
    public TextMeshProUGUI topImpactZone;

    public GameObject achTitle;
    public GameObject bSTitle;
    public GameObject tDTitle;
    public GameObject bTTitle;
    public GameObject tITitle;

    public GameObject achSet;
    public GameObject category;
    public GameObject bSAch;
    public GameObject tDAch;
    public GameObject bTAch;
    public GameObject tIAch;

    public GameObject backButton;

    void Start()
    {
        bestScore.text = manager.bestScore.ToString();
        topDistance.text = manager.totalDistance.ToString()+ "m";
        breakThrough.text = manager.breakThrough.ToString();
        topImpactZone.text = manager.topImpactZone.ToString();
    }

    public void BestScoreButton()
    {
        bSTitle.SetActive(true);
        bSAch.SetActive(true);
        backButton.SetActive(true);
        CloseCategory();
    }

    public void TopDistanceButton()
    {
        tDTitle.SetActive(true);
        tDAch.SetActive(true);
        backButton.SetActive(true);

        CloseCategory();
    }

    public void breakThroughButton()
    {
        bTTitle.SetActive(true);
        bTAch.SetActive(true);
        backButton.SetActive(true);

        CloseCategory();
    }

    public void topImpactButton()
    {
        tITitle.SetActive(true); 
        tIAch.SetActive(true);
        backButton.SetActive(true);

        CloseCategory();
    }

    public void BackCategory()
    {
        OpenCategory();
        bSTitle.SetActive(false);
        tDTitle.SetActive(false);
        tITitle.SetActive(false);
        bTTitle.SetActive(false);

        bSAch.SetActive(false);
        tDAch.SetActive(false);
        bTAch.SetActive(false);
        tIAch.SetActive(false);

        backButton.SetActive(false);

    }

    void CloseCategory()
    {
        achTitle.SetActive(false);
        achSet.SetActive(false);
        category.SetActive(false);
    }
    
    void OpenCategory()
    {
        achTitle.SetActive(true);
        achSet.SetActive(true);
        category.SetActive(true);

    }
}
