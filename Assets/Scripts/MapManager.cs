using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap[] tilemaps;
    void Start()
    {
        tilemaps = GetComponentsInChildren<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        tilemaps[0].GetTile(new Vector3Int(2, 3, 0)).IsDestroyed();
    }
}
