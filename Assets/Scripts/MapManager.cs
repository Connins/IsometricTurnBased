using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEditor.FilePathAttribute;

public class MapManager : MonoBehaviour
{
    [SerializeField] int baseMapXSize;
    [SerializeField] int baseMapYSize;
    [SerializeField] int baseMapZSize;
    [SerializeField] GameObject baseTile;

    // Start is called before the first frame update
    private Tilemap tilemaps;
    private Grid grid;

    private GameObject[,,] tiles;
    private Dictionary<Vector3Int, GameObject> tileMap;
    
    void Start()
    {
        tiles = new GameObject[100, 100, 100];
        MapTiles();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void SpawnBaseMap(int x, int z)
    {
        //Idea of this function is to create a grid of blocks from serialized fields
    }

    private void MapTiles()
    {

        BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>();

        foreach (var tile in boxColliders)
        {
            //Converts position of tiles into an index we can use. 
            Vector3Int key = new Vector3Int((int)tile.gameObject.transform.position.x, (int)tile.gameObject.transform.parent.transform.position.y , (int)tile.gameObject.transform.position.z);
            tiles[key.x,key.y,key.z] = tile.gameObject;
        }
    }
    private void tileLoop()
    {
        
        for (var x = 0; x < tiles.GetLength(0); x++)
        {
            for (var y = 0; y < tiles.GetLength(1); y++)
            {
                for (var z = 0; z < tiles.GetLength(2); z++)
                {
                    if(tiles[x, y, z] != null)
                    {
                        //tiles[x, y, z].GetComponent<Highlight>().ToggleHighlight(true);
                    }
                }
            }
        }
    }

    public List<GameObject> getTilesInRange(uint move, Vector3Int location)
    {
        List<GameObject> tilesInRange = new List<GameObject>();
        for (var x = -move; x < move; x++)
        {
            for (var z = -move; z < move; z++)
            {
                if((Mathf.Abs(x) + MathF.Abs(z)) < move && (location.x + x) >= 0 && (location.z + z) >= 0)
                {
                    print("currenlty problem when near the edge of the board");
                    //tiles[location.x + x, 0, location.z + z].GetComponent<Highlight>().ToggleHighlight(true);
                    tilesInRange.Add(tiles[location.x + x, 0, location.z + z]);
                }
            }
        }
        return tilesInRange;
    }

    public List<GameObject> getTilesInRangeJump(uint move, uint jump, Vector3Int location)
    {
        List<GameObject> tilesInRange = new List<GameObject>();
        for (var x = -move; x <= move; x++)
        {
            for (var z = -move; z <= move; z++)
            {
                for (var y = -jump; y <= jump; y++)
                {
                    bool isValidTile = (Mathf.Abs(x) + MathF.Abs(z)) <= move && (location.x + x) >= 0 && (location.y + y) >= 0 && (location.z + z) >= 0 && tiles[location.x + x, location.y + y, location.z + z] != null;
                    if (isValidTile)
                    {
                        Debug.LogFormat("x: {0}, y: {1}, z: {2}", x,y,z);   
                        tilesInRange.Add(tiles[location.x + x, location.y + y, location.z + z]);   
                    }
                }
            }
        }
        return tilesInRange;
    }

    //public List<GameObject> getTilesInRangeWalk(uint move, Vector3Int location)
    //{
        
    //    if(move == 0)
    //    {
    //        List<GameObject> tile = new List<GameObject>();
    //        tile.Add(tiles[location.x, location.y, location.z]);
    //        return tile;
    //    }
    //    else
    //    return tilesInRange;
    //}
}
