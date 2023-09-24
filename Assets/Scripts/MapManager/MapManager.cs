using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class MapManager : MonoBehaviour
{

    [SerializeField] private int xBound;
    [SerializeField] private int yBound;
    [SerializeField] private int zBound;

    [SerializeField] private LayerMask mapTileMask;

    private List<OccupiedTile> occupiedTiles = new List<OccupiedTile>();
    public List<GameObject> capturePoints;
    private GameObject[,,] tiles;

    private void Awake()
    {
        tiles = new GameObject[xBound, yBound, zBound];
        MapTiles();
        FillCapturePoints();
    }

    private void FillCapturePoints()
    {
        GameObject[] capturePointsFound = GameObject.FindGameObjectsWithTag("CapturePoint");

        foreach (var capturePoint in capturePointsFound)
        {
            capturePoints.Add(capturePoint);      
        }
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
        occupiedTiles.RemoveAll(x => x.GetTile() == tile);
    }
    public void removeFromOccupied(GameObject charecter)
    {
        occupiedTiles.RemoveAll(x => x.GetOccupier() == charecter);
    }

    //accessor functions
    public GameObject getTile(Vector3 position)
    {
        return tiles[(int)position.x - 1, (int)position.y - 1, (int)position.z - 1];
    }

    public GameObject getTile(GameObject charecter)
    {
        return getTile(charecter.transform.position);
    }
    public GameObject getOccupier(Vector3 position)
    {
        GameObject tile = getTile(position);
        return getOccupier(tile);
    }

    public GameObject getOccupier(GameObject tile)
    {
        GameObject output = null;
        if (isTileOccupied(tile))
        {
            output = occupiedTiles.First(x => x.GetTile() == tile).GetOccupier();
        }
        return output;
    }
    public List<GameObject> CapturePoints
    {
        get { return capturePoints; }
    }
    public bool isTileOccupied(GameObject tile)
    {
        return occupiedTiles.Any(x => x.GetTile() == tile);
    }

    public bool isTileOccupied(Vector3 location)
    {
        return isTileOccupied(getTile(location));
    }

    public bool isTileACapturePoint(GameObject tile)
    {
        return capturePoints.Any(x => x == tile);
    }

    public bool isCharecterOnCapturePoint(GameObject charecter)
    {
        GameObject tile = getTile(charecter);
        return capturePoints.Any(x => x == tile);
    }
    public bool isEnemyInTiles(List<GameObject> tiles, bool goodGuy)
    {
        bool output = false;

        output = occupiedTiles.Any(x => tiles.Contains(x.GetTile()) && x.GetOccupier().GetComponent<CharecterStats>() != null && x.GetOccupier().GetComponent<CharecterStats>().GoodGuy != goodGuy);

        return output;
    }

    public bool isThisCharecterInTiles(List<GameObject> tiles, GameObject charecter)
    {
        bool output = false;

        output = occupiedTiles.Any(x => tiles.Contains(x.GetTile()) && x.GetOccupier().GetComponent<CharecterStats>() != null && x.GetOccupier() == charecter);

        return output;
    }

    public Vector3Int getTileIndex(GameObject occupier)
    {
        return occupiedTiles.Find(x => x.GetOccupier() == occupier).GetTileIndex();
    }

    public List<GameObject> getMovementTilesInRange(uint move, uint jump, Vector3Int location, List<GameObject> tilesInRange, bool passible, bool goodGuy)
    {
        GameObject tile = tiles[location.x, location.y, location.z];
        //Checks if we have already stepped on tile if so just return current held tilesInRange
        if (!tilesInRange.Contains(tile))
        {
            tilesInRange.Add(tile);
        }

        if (move > 0)
        {
            List<Vector3Int> validTilesLocation = GetValidTilesNextToThisTile(location, jump, passible, goodGuy);
            if (validTilesLocation.Count != 0)
            {
                foreach (var nextLocation in validTilesLocation)
                {
                    tilesInRange = getMovementTilesInRange(move - 1, jump, nextLocation, tilesInRange, passible, goodGuy);
                }
            }
        }

        return tilesInRange;
    }

    public List<GameObject> getAttackTilesInRange(uint range, Vector3Int tileIndex, uint heightBonus)
    {
        //melee units
        if (range == 1)
        {
            return getMeleeTilesInRange(tileIndex);
        }
        //archer units        
        if (range > 1) 
        {
            return getRangedTilesInRange(range, tileIndex, heightBonus);
        }

        return new List<GameObject>();
        
    }

    public List<GameObject> getMeleeTilesInRange(Vector3Int tileIndex)
    {
        List<GameObject> tilesInRange = new List<GameObject>();

        List<Vector3Int> validTilesLocation = GetValidTilesNextToThisTile(tileIndex, 1, true);

        if (validTilesLocation.Count != 0)
        {
            foreach (var tileLocation in validTilesLocation)
            {
                tilesInRange.Add(tiles[tileLocation.x, tileLocation.y, tileLocation.z]);
            }
        }
        return tilesInRange;
    }
    public List<GameObject> getRangedTilesInRange(uint range, Vector3Int tileIndex, uint heightBonus)
    {
        List<GameObject> tilesInRange = new List<GameObject>();
        List<GameObject> tilesToRemoveFromInRange = new List<GameObject>();
        int lowerLimit = 0;
        
        for (var x = 0; x < tiles.GetLength(0); x++)
        {
            for (var z = 0; z < tiles.GetLength(2); z++)
            {
                int height = heightCalc(tileIndex, new Vector3Int(x, 0, z), range, heightBonus);
               
                if (height >= 0)
                {
                    tilesInRange.AddRange(GetRangedValidTilesInCollunm(new Vector3Int(x, height, z), lowerLimit));
                }
            }
        }

        //Raycast test to see if unit could see unit on all tiles
        float tileYOffset = 2;
        Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);
        GameObject startingTile = tiles[tileIndex.x,tileIndex.y,tileIndex.z];
        Vector3 startPoint = startingTile.transform.position;
        startPoint.y = startPoint.y + tileYOffset;
        tilesToRemoveFromInRange.Add(startingTile);
        foreach (GameObject tile in tilesInRange)
        {
            Vector3 endPoint = tile.transform.position;
            endPoint.y = endPoint.y + tileYOffset;
            
            Vector3 rayDirection = endPoint - startPoint;

            RaycastHit hitInfo;

            if (Physics.Raycast(startPoint + offset, rayDirection, out hitInfo, rayDirection.magnitude, mapTileMask))
            {
                tilesToRemoveFromInRange.Add(tile);
            }
            
        }

        foreach (GameObject tile in tilesToRemoveFromInRange)
        {
            tilesInRange.Remove(tile);
        }

        return tilesInRange;
    }

    private int heightCalc(Vector3Int startLocation, Vector3Int currentLocation, uint range, uint heightBonus)
    {
        int distance = Mathf.Abs(startLocation.x - currentLocation.x) + Mathf.Abs(startLocation.z - currentLocation.z);

        int height = startLocation.y + ((int)range - distance) * (int)heightBonus;
         
        return height;
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
    private List<Vector3Int> GetValidTilesNextToThisTile(Vector3Int location, uint jump, bool passible, bool? goodGuy = null)
    {
        List<Vector3Int> validTilesLocation = new List<Vector3Int>();
        Vector3Int xForward = new Vector3Int(location.x + 1, location.y, location.z);
        Vector3Int xBackward = new Vector3Int(location.x - 1, location.y, location.z);
        Vector3Int zForward = new Vector3Int(location.x, location.y, location.z + 1);
        Vector3Int zBackward = new Vector3Int(location.x, location.y, location.z - 1);

        validTilesLocation.AddRange(GetValidTilesInCollunm(xForward, jump, passible, goodGuy));
        validTilesLocation.AddRange(GetValidTilesInCollunm(xBackward, jump, passible, goodGuy));
        validTilesLocation.AddRange(GetValidTilesInCollunm(zForward, jump, passible, goodGuy));
        validTilesLocation.AddRange(GetValidTilesInCollunm(zBackward, jump, passible, goodGuy));

        return validTilesLocation;
    }
    private List<Vector3Int> GetValidTilesInCollunm(Vector3Int location, uint jump, bool passible, bool? goodGuy = null)
    {
        List <Vector3Int> validTilesLocation = new List <Vector3Int>();

        if(IsLocationInBounds(location))
        { 
            for (var y = -jump; y <= jump; y++)
            {
                Vector3Int nextLocation = new Vector3Int(location.x, location.y + (int)y, location.z);
                if (IsLocationInBounds(nextLocation) && IsTileStandable(nextLocation, passible, goodGuy))
                {
                    validTilesLocation.Add(nextLocation);
                }
            }
        }
        
        return validTilesLocation;
    }

    private List<GameObject> GetRangedValidTilesInCollunm(Vector3Int tileIndex, int lowerLimit)
    {
        List<GameObject> validTiles = new List<GameObject>();

        if (IsLocationInBounds(tileIndex))
        {
            for (var y = tileIndex.y; y >= lowerLimit; y--)
            {
                Vector3Int nextLocation = new Vector3Int(tileIndex.x, y, tileIndex.z);
                if (IsLocationInBounds(nextLocation) && IsTileStandable(nextLocation, true))
                {
                    validTiles.Add(tiles[nextLocation.x, nextLocation.y, nextLocation.z]);
                }
            }
        }

        return validTiles;
    }

    private bool IsLocationInBounds(Vector3Int location)
    {
        return location.x >= 0 && location.y >= 0 && location.z >= 0 && location.x < xBound && location.y < yBound && location.z < zBound;
    }

    private bool IsTileStandable(Vector3Int location, bool passible, bool? goodGuy = null)
    {
        bool tileIsThere = tiles[location.x, location.y, location.z] != null;
        bool noTilesAbove = tiles[location.x, location.y + 1, location.z] == null && tiles[location.x, location.y + 2, location.z] == null;

        bool output = tileIsThere && noTilesAbove;
        if (!passible && output)
        {
            if (isTileOccupied(tiles[location.x, location.y, location.z]))
            {
                if (goodGuy.HasValue)
                {
                    GameObject occupier = getOccupier(tiles[location.x, location.y, location.z]);
                    if (occupier.GetComponent<CharecterStats>() != null && occupier.GetComponent<CharecterStats>().GoodGuy == goodGuy.Value)
                    {
                        output = true;
                    }
                    else
                    {
                        output = false;
                    }
                }
                else
                {
                    output = false;
                }
            }

        }
        return output;
    }

}
