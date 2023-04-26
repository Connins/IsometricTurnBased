using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private LayerMask mapTileMask;
    [SerializeField] private LayerMask charecterMask;

    [SerializeField] private Grid grid;
    private MapManager mapManager;
    private TurnManager turnManager;
    private GameObject previousTileHighlight;
    
    private GameObject currentSelectedPlayer;
    private GameObject currentSelectedEnemy;



    private Vector3 selectedPlayersPosition;

    private GameObject currentHighlightedTile;
    private GameObject charecterHit;

    private float offset = 1f;

    private List<GameObject> tilesInRange;
    private List<GameObject> attackTilesInRange;

    private UIController uIController;

    // Start is called before the first frame update
    void Start()
    {
        uIController = FindAnyObjectByType<Canvas>().GetComponent<UIController>();
        previousTileHighlight = null;
        tilesInRange = new List<GameObject>();
        attackTilesInRange = new List<GameObject>();
        mapManager = grid.GetComponent<MapManager>();
        turnManager = GameObject.Find("Charecters").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (charecterHit != null && turnManager.activePlayer(charecterHit) && charecterHit != currentSelectedPlayer)
            {
                selectPlayer();
            }
            else
            {
                if (tilesInRange.Contains(currentHighlightedTile) && !mapManager.isTileOccupied(currentHighlightedTile))
                {
                    selectTileAndMovePlayer();
                }
            }

        }
    }

    void FixedUpdate()
    {
        currentHighlightedTile = GetObject(mapTileMask);
        charecterHit = GetObject(charecterMask);

        highlightCurentTile();
    }

    private GameObject GetObject(LayerMask mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Builds a ray from camera point of view to the mouse position 
        RaycastHit hit;
        // Casts the ray and get the first game object hit 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            //Instantiate(clickMarker, hit.point, Quaternion.identity); //places clickMarker at hit.point. This isn't needed, just there for visualisation. 
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    private void highlightTiles(List<GameObject> tiles, string highlight)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Highlight>().ToggleHighlight(highlight);
        }
    }


    private void selectPlayer()
    {
        highlightTiles(tilesInRange, "noHighlight");
        highlightTiles(attackTilesInRange, "noHighlight");

        if (currentSelectedPlayer != null)
        {
            playerHasBeenDeselected();
        }

        currentSelectedPlayer = charecterHit;
        
        selectedPlayersPosition = new Vector3(currentSelectedPlayer.transform.position.x, currentSelectedPlayer.transform.position.y, currentSelectedPlayer.transform.position.z);


        Vector3Int tileIndex = new Vector3Int((int)(currentSelectedPlayer.transform.position.x - offset), (int)(currentSelectedPlayer.transform.position.y - offset), (int)(currentSelectedPlayer.transform.position.z - offset));
        uint move = currentSelectedPlayer.GetComponent<CharecterStats>().Move;
        uint jump = currentSelectedPlayer.GetComponent<CharecterStats>().Jump;
        uint weaponRange = currentSelectedPlayer.GetComponent<WeaponStats>().Range;
        tilesInRange.Clear();
        tilesInRange = mapManager.getTilesInRange(move, jump, tileIndex, tilesInRange, false);
        highlightTiles(tilesInRange, "inMoveRangeHighlight");
        attackTilesInRange.Clear();
        attackTilesInRange = mapManager.getTilesInRange(weaponRange, 1, tileIndex, attackTilesInRange, true);
        highlightTiles(attackTilesInRange, "inAttackRangeHighlight");
        uIController.enableWait();
    }

    private void selectTileAndMovePlayer()
    {
        highlightTiles(attackTilesInRange, "noHighlight");
        highlightTiles(tilesInRange, "inMoveRangeHighlight");
        attackTilesInRange.Clear();
        uint weaponRange = currentSelectedPlayer.GetComponent<WeaponStats>().Range;

        currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(currentHighlightedTile);
        Vector3Int tileIndex = new Vector3Int((int)(currentSelectedPlayer.transform.position.x - offset), (int)(currentSelectedPlayer.transform.position.y - offset), (int)(currentSelectedPlayer.transform.position.z - offset));
        attackTilesInRange = mapManager.getTilesInRange(weaponRange, 1, tileIndex, attackTilesInRange, true);
        highlightTiles(attackTilesInRange, "inAttackRangeHighlight");

    }

    public void playerHasOfficialyMoved()
    {
        turnManager.charecterDoneAction(currentSelectedPlayer);
        clearCharectersHighlights();
        uIController.disableWait();
    }

    public void playerHasBeenDeselected()
    {
        currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(selectedPlayersPosition);
        clearCharectersHighlights();
        uIController.disableWait();
    }

    public void clearCharectersHighlights()
    {
        currentSelectedPlayer = null;
        highlightTiles(tilesInRange, "noHighlight");
        tilesInRange.Clear();
        highlightTiles(attackTilesInRange, "noHighlight");
        attackTilesInRange.Clear();
    }

    private void highlightCurentTile()
    { 
        bool hitEqualsCurrentTile = currentHighlightedTile == previousTileHighlight;
        bool previousTileInTilesInRange = tilesInRange.Contains(previousTileHighlight);
        bool currentTileInAttackTilesInRange = attackTilesInRange.Contains(currentHighlightedTile);
        bool noTileHighlighted = currentHighlightedTile == null && previousTileHighlight != null;
        bool newTileHigghlighted = !hitEqualsCurrentTile && previousTileHighlight != null && !previousTileInTilesInRange;

        if (noTileHighlighted || newTileHigghlighted)
        {
            previousTileHighlight.GetComponent<Highlight>().ToggleHighlight("noHighlight");
        }

        if (currentHighlightedTile && !currentTileInAttackTilesInRange)
        {
            currentHighlightedTile.GetComponent<Highlight>().ToggleHighlight("inMoveRangeHighlight");
            previousTileHighlight = currentHighlightedTile;
        }
    }
}
