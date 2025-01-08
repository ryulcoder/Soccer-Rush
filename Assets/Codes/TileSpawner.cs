using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private int tileStartCount = 10;
    [SerializeField]
    private int minimumStratightTiles = 3;
    [SerializeField]
    private int maximumStratightTiles = 15;
    [SerializeField]
    private GameObject startingTile;
    [SerializeField]
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;
    private GameObject prevTile;

    private List<GameObject> currentTiles;
    private List<GameObject> currentObstacles;

    private void Start()
    {
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < tileStartCount; i++)
        {
            SpawnTile(startingTile.GetComponent<Tile>(), false);
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    private void SpawnTile(Tile tile, bool spawnObstacle = false)
    {
        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, Quaternion.identity);
        currentTiles.Add(prevTile);
        currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
    {
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
