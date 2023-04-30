using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private LayerMask mapTileMask;
    [SerializeField] private LayerMask charecterMask;
    [SerializeField] private Grid grid;
    private UIController uIController;

    private MapManager mapManager;
    private TurnManager turnManager;
    
    private GameObject currentSelectedPlayer;
    private Vector3 selectedPlayersPosition;

    private GameObject currentSelectedEnemy;
    [SerializeField] private bool inAttackMode;
    [SerializeField] private bool inWaitMode;

    private GameObject currentHighlightedTile;
    private GameObject previousTileHighlight;
    private GameObject charecterHit;

    private float offset = 1f;

    private List<GameObject> tilesInRange;
    private List<GameObject> attackTilesInRange;

    private bool coroutineActive;

    // Start is called before the first frame update
    void Start()
    {
        uIController = FindAnyObjectByType<UIController>();
        previousTileHighlight = null;
        tilesInRange = new List<GameObject>();
        attackTilesInRange = new List<GameObject>();
        mapManager = grid.GetComponent<MapManager>();
        turnManager = GameObject.Find("Charecters").GetComponent<TurnManager>();
        coroutineActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!coroutineActive)
        {
            if (inWaitMode && currentHighlightedTile != null)
            {
                chooseRotation();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (inAttackMode && charecterHit != null && charecterHit.GetComponent<CharecterStats>().GoodGuy != currentSelectedPlayer.GetComponent<CharecterStats>().GoodGuy)
                {
                    attackAndOfficiallyMove();
                }
                else if (charecterHit != null && turnManager.activePlayer(charecterHit) && charecterHit != currentSelectedPlayer)
                {
                    selectPlayer();
                }
                else if (tilesInRange.Contains(currentHighlightedTile) && !mapManager.isTileOccupied(currentHighlightedTile))
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
        clearCharectersHighlights();

        if (currentSelectedPlayer != null)
        {
            playerHasBeenDeselected();
        }

        currentSelectedPlayer = charecterHit;
        
        selectedPlayersPosition = new Vector3(currentSelectedPlayer.transform.position.x, currentSelectedPlayer.transform.position.y, currentSelectedPlayer.transform.position.z);

        Vector3Int tileIndex = mapManager.getTileIndex(currentSelectedPlayer);
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
        checkEnemyInRange();
    }

    private void selectTileAndMovePlayer()
    {
        inAttackMode = false;
        highlightTiles(attackTilesInRange, "noHighlight");
        highlightTiles(tilesInRange, "inMoveRangeHighlight");
        attackTilesInRange.Clear();
        uint weaponRange = currentSelectedPlayer.GetComponent<WeaponStats>().Range;

        currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(currentHighlightedTile);
        Vector3Int tileIndex = new Vector3Int((int)(currentSelectedPlayer.transform.position.x - offset), (int)(currentSelectedPlayer.transform.position.y - offset), (int)(currentSelectedPlayer.transform.position.z - offset));
        attackTilesInRange = mapManager.getTilesInRange(weaponRange, 1, tileIndex, attackTilesInRange, true);
        highlightTiles(attackTilesInRange, "inAttackRangeHighlight");
        checkEnemyInRange();

    }

    private void attackAndOfficiallyMove()
    {
        currentSelectedEnemy = charecterHit;
        currentSelectedPlayer.GetComponent<PlayerController>().rotateCharecter(charecterHit.transform.position);
        currentSelectedPlayer.GetComponent<Animator>().Play("Attack");
        uint damage = currentSelectedPlayer.GetComponent<CharecterStats>().outPutDamage();
        StartCoroutine(enemyHit(0.9f, damage));
    }

    IEnumerator enemyHit(float delayTime, uint damage)
    {
        coroutineActive = true;
        yield return new WaitForSeconds(delayTime);
        currentSelectedEnemy.GetComponent<CharecterStats>().takeHit(damage);
        currentSelectedEnemy = null;
        coroutineActive = false;
        inAttackMode = false;
        playerHasOfficialyMoved();
        yield return null;
        
    }
    public void playerHasOfficialyMoved()
    {
        turnManager.charecterDoneAction(currentSelectedPlayer);
        currentSelectedPlayer = null;
        clearCharectersHighlights();
        uIController.disableWait();
        uIController.disableAttack();
    }

    public void playerHasBeenDeselected()
    {
        if(currentSelectedPlayer != null)
        {
            currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(selectedPlayersPosition);
            currentSelectedPlayer = null;
            clearCharectersHighlights();
            inAttackMode = false;
            uIController.disableWait();
            uIController.disableAttack();
        }  
    }

    private void chooseRotation()
    {
        currentSelectedPlayer.GetComponent<PlayerController>().rotateCharecter();
        if (Input.GetMouseButtonDown(0))
        {
            inWaitMode = false;
            playerHasOfficialyMoved();
        }
    }
    public void clearCharectersHighlights()
    {
        highlightTiles(tilesInRange, "noHighlight");
        tilesInRange.Clear();
        highlightTiles(attackTilesInRange, "noHighlight");
        attackTilesInRange.Clear();
    }

    public void setInAttackMode(bool isInAttackMode)
    {
        inAttackMode = isInAttackMode;
    }
    public void setInWaitMode(bool isInWaitMode)
    {
        inWaitMode = isInWaitMode;
        if (inWaitMode)
        {
            clearCharectersHighlights();
        }
    }
    private void highlightCurentTile()
    { 
        bool hitEqualsCurrentTile = currentHighlightedTile == previousTileHighlight;
        bool previousTileInTilesInRange = tilesInRange.Contains(previousTileHighlight);
        bool currentTileInAttackTilesInRange = attackTilesInRange.Contains(currentHighlightedTile);
        bool noTileHighlighted = currentHighlightedTile == null && previousTileHighlight != null;
        bool newTileHigghlighted = !hitEqualsCurrentTile && previousTileHighlight != null;

        if ((noTileHighlighted || newTileHigghlighted) && !previousTileInTilesInRange)
        {
            previousTileHighlight.GetComponent<Highlight>().ToggleHighlight("noHighlight");
        }

        if (currentHighlightedTile && !currentTileInAttackTilesInRange)
        {
            currentHighlightedTile.GetComponent<Highlight>().ToggleHighlight("inMoveRangeHighlight");
            previousTileHighlight = currentHighlightedTile;
        }
    }

    private void checkEnemyInRange()
    {
        bool goodGuy = currentSelectedPlayer.GetComponent<CharecterStats>().GoodGuy;

        bool enemyInRange = mapManager.isEnemyInTiles(attackTilesInRange, goodGuy);

        if (enemyInRange)
        {
            uIController.enableAttack();
        }
        else
        {
            uIController.disableAttack();
        }
    }
}
