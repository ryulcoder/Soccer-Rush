using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LobbyPlayAni : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject StageFont;
    public GameObject StageArrow;
    public void PlayTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play();
            StageFont.SetActive(false);
            StageArrow.SetActive(false);
        }
    }
}
