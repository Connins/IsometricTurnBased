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

    [SerializeField] private int xBound;
    [SerializeField] private int yBound;
    [SerializeField] private int zBound;

    private List<OccupiedTile> occupiedTiles = new List<OccupiedTile>();

    private GameObject[,,] tiles;

    //accessor functions
    public GameObject getTile(Vector3 position)
    {
        return tiles[(int)position.x - 1, (int)position.y - 1, (int)position.z - 1];
    }

    private void Awake()
    {
        tiles = new GameObject[xBound, yBound, zBound];
        MapTiles();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addToOccupied(GameObject occupier, Vector3 position)
    {
        GameObject tile = getTile(position);
        Vector3Int tileIndex = new Vector3Int((int)position.x - 1, (int)position.y - 1, (int)position.z - 1);
        OccupiedTile occupiedTile = new OccupiedTile(tile, occupier, tileIndex);
        occupiedTiles.Add(occupiedTile);
    }
    public void removeFromOccupied(Vector3 position)
    {
        GameObject tile = getTile(position);
        occupiedTiles.RemoveAll(x => x.GetTile() == tile); ;
    }

    public bool isTileOccupied(GameObject tile)
    {
        return occupiedTiles.Any(x => x.GetTile() == tile);
    }
    public List<GameObject> getTilesInRange(uint move, uint jump, Vector3Int location, List<GameObject> tilesInRange, bool passible)
    {
        GameObject tile = tiles[location.x, location.y, location.z];
        //Checks if we have already stepped on tile if so just return current held tilesInRange
        if (!tilesInRange.Contains(tile))
        {
            tilesInRange.Add(tile);
        }

        if (move > 0)
        {
            List<Vector3Int> validTilesLocation = GetValidTilesNextToThisTile(location, jump, passible);
            if (validTilesLocation.Count != 0)
            {
                foreach (var nextLocation in validTilesLocation)
                {
                    tilesInRange = getTilesInRange(move - 1, jump, nextLocation, tilesInRange, passible);
                }
            }
        }

        return tilesInRange;
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
            Vector3Int key = new Vector3Int((int)tile.gameObject.transform.position.x, (int)tile.gameObject.transform.parent.transform.position.y, (int)tile.gameObject.transform.position.z);
            tiles[key.x, key.y, key.z] = tile.gameObject;
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
    private List<Vector3Int> GetValidTilesNextToThisTile(Vector3Int location, uint jump, bool passible)
    {
        List<Vector3Int> validTilesLocation = new List<Vector3Int>();
        Vector3Int xForward = new Vector3Int(location.x + 1, location.y, location.z);
        Vector3Int xBackward = new Vector3Int(location.x - 1, location.y, location.z);
        Vector3Int zForward = new Vector3Int(location.x, location.y, location.z + 1);
        Vector3Int zBackward = new Vector3Int(location.x, location.y, location.z - 1);

        validTilesLocation.AddRange(GetValidTilesInCollunm(xForward, jump, passible));
        validTilesLocation.AddRange(GetValidTilesInCollunm(xBackward, jump, passible));
        validTilesLocation.AddRange(GetValidTilesInCollunm(zForward, jump, passible));
        validTilesLocation.AddRange(GetValidTilesInCollunm(zBackward, jump, passible));

        return validTilesLocation;
    }
    private List<Vector3Int> GetValidTilesInCollunm(Vector3Int location, uint jump, bool passible)
    {
        List <Vector3Int> validTilesLocation = new List <Vector3Int>();

        if(IsLocationInBounds(location))
        {
            for (var y = -jump; y <= jump; y++)
            {
                Vector3Int nextLocation = new Vector3Int(location.x, location.y + (int)y, location.z);
                if (IsLocationInBounds(nextLocation) && IsTileStandable(nextLocation, passible))
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
    private bool IsTileStandable(Vector3Int location, bool passible)
    {
        bool tileIsThere = tiles[location.x, location.y, location.z] != null;
        bool noTilesAbove = tiles[location.x, location.y + 1, location.z] == null && tiles[location.x, location.y + 2, location.z] == null;

        bool output = tileIsThere && noTilesAbove;
        if (!passible)
        {
            output = output && !isTileOccupied(tiles[location.x, location.y, location.z]);
        }
        return output;
    }

}
