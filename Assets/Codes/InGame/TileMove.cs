using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMove : MonoBehaviour
{
    public GameManager GameManager;

    public Transform TileTrans;

    bool onPlayer, inPlayer, coroutine;

    void Update()
    {
        if (inPlayer && !onPlayer)
        {
            if (!coroutine)
                StartCoroutine(DelayAndMoveTile());
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == "PlayerFoot")
        {
            if (!inPlayer)
                inPlayer = true;

            onPlayer = true;

            return;
        }


    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name != "PlayerFoot") return;

        onPlayer = false;
    }

    IEnumerator DelayAndMoveTile()
    {
        coroutine = true;

        yield return new WaitForSeconds(1);

        if (onPlayer) yield break;

        TileTrans.position += new Vector3(0, 0, TileTrans.localScale.z * GameManager.Tiles.Length);

        inPlayer = false;
        coroutine = false;
    }

}
