using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] int baseMapXSize;
    [SerializeField] int baseMapYSize;
    [SerializeField] int baseMapZSize;
    [SerializeField] GameObject baseTile;

    // Start is called before the first frame update
    private Tilemap tilemaps;
    private Grid grid;
    void Start()
    {
        tilemaps = GetComponentInChildren<Tilemap>();
        tileLoop(tilemaps);
        //SpawnBaseMap(baseMapXSize, baseMapZSize);
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    tileLoop(tilemaps[0]);
        //}
       
    }

    private void SpawnBaseMap(int x, int z)
    {
        //foreach(int i in Enumerable.Range(1, x))
        //{
        //    foreach (int j in Enumerable.Range(1, z))
        //    {
        //        Vector3 temp = tilemaps[0].GetCellCenterLocal(new Vector3Int(i, 0, j));

        //        Instantiate(baseTile, temp, Quaternion.identity);
        //    }
        //}
        //tilemaps[0].GetTile(new Vector3Int(x, 0, z)).
        //tilemaps[0].FloodFill(new Vector3Int(x, 0, z),temp.GameObject(baseTile));

    }
    private void tileLoop(Tilemap tilemap)
    {
        print(tilemap.cellBounds.min.x);
        print(tilemap.cellBounds.max.x);
        for (var x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for (var y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                for (var z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++)
                {

                    Debug.LogFormat("x: {0}, y {1}, z: {2}", x, y, z);
                    tilemap.GetTile(new Vector3Int(x, y, z)).GameObject().GetComponent<Highlight>().ToggleHighlight(true);
                }
            }

        }
    }
}
