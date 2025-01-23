using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMove : MonoBehaviour
{
    public GameManager GameManager;
    public PoolManager PoolManager;
    public GameObject grass;

    public Transform FloorTrans;

    bool onPlayer, inPlayer, coroutine;

    void Update()
    {
        if (inPlayer && !onPlayer)
        {
            if (!coroutine)
                StartCoroutine(DelayAndMoveTile());
        }
    }

    IEnumerator DelayAndMoveTile()
    {
        coroutine = true;

        //PoolManager.PoolObjects(GetComponentsInChildren<Transform>());

        yield return new WaitForSeconds(3);

        if (onPlayer) yield break;

        FloorTrans.position += new Vector3(0, 0, FloorTrans.localScale.z * GameManager.Tiles.Length);
        //Instantiate(grass, FloorTrans.position, Quaternion.identity);

        inPlayer = false;
        coroutine = false;

        SettingDefender();
    }

    void SettingDefender()
    {
        

        //PoolManager.DefenderPreFabs.Length
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

}
