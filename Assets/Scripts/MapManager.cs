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

    [SerializeField] private int xBound;
    [SerializeField] private int yBound;
    [SerializeField] private int zBound;

    private GameObject[,,] tiles;
    private Dictionary<Vector3Int, GameObject> tileMap;
    

    // Start is called before the first frame update
    void Start()
    {
        tiles = new GameObject[xBound, yBound, zBound];
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
                    if (tiles[x, y, z] != null)
                    {
                        //tiles[x, y, z].GetComponent<Highlight>().ToggleHighlight(true);
                    }
                }
            }
        }
    }
    public List<GameObject> getTilesInRange(uint move, uint jump, Vector3Int location, List<GameObject> tilesInRange)
    {
        GameObject tile = tiles[location.x, location.y, location.z];
        //Checks if we have already stepped on tile if so just return current held tilesInRange
        if (!tilesInRange.Contains(tile))
        {
            tilesInRange.Add(tile);
        }

        if (move > 0)
        {
            List<Vector3Int> validTilesLocation = GetValidTilesNextToThisTile(location, jump);
            if (validTilesLocation.Count != 0)
            {
                foreach (var nextLocation in validTilesLocation)
                {
                    tilesInRange = getTilesInRange(move - 1, jump, nextLocation, tilesInRange);
                }
            }
        }

        return tilesInRange;
    }

    private List<Vector3Int> GetValidTilesNextToThisTile(Vector3Int location, uint jump)
    {
        List<Vector3Int> validTilesLocation = new List<Vector3Int>();
        Vector3Int xForward = new Vector3Int(location.x + 1, location.y, location.z);
        Vector3Int xBackward = new Vector3Int(location.x - 1, location.y, location.z);
        Vector3Int zForward = new Vector3Int(location.x, location.y, location.z + 1);
        Vector3Int zBackward = new Vector3Int(location.x, location.y, location.z - 1);

        validTilesLocation.AddRange(GetValidTilesInCollunm(xForward, jump));
        validTilesLocation.AddRange(GetValidTilesInCollunm(xBackward, jump));
        validTilesLocation.AddRange(GetValidTilesInCollunm(zForward, jump));
        validTilesLocation.AddRange(GetValidTilesInCollunm(zBackward, jump));

        return validTilesLocation;
    }
    private List<Vector3Int> GetValidTilesInCollunm(Vector3Int location, uint jump)
    {
        List <Vector3Int> validTilesLocation = new List <Vector3Int>();

        if(IsLocationInBounds(location))
        {
            for (var y = -jump; y <= jump; y++)
            {
                Vector3Int nextLocation = new Vector3Int(location.x, location.y + (int)y, location.z);
                if (IsLocationInBounds(nextLocation) && IsTileStandable(nextLocation))
                {
                    validTilesLocation.Add(nextLocation);
                }
            }
        }
        
        return validTilesLocation;
    }

    private bool IsLocationInBounds(Vector3Int location)
    {
        return location.x >= 0 && location.y >= 0 && location.z >= 0 && location.x < xBound && location.y < yBound && location.z < zBound;
    }
    private bool IsTileStandable(Vector3Int location)
    {
        return tiles[location.x, location.y, location.z] != null && tiles[location.x, location.y + 1, location.z] == null && tiles[location.x, location.y + 2, location.z] == null;
    }
}
