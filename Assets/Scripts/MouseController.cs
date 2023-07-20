using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseController : NetworkBehaviour
{
    [SerializeField] private LayerMask mapTileMask;
    [SerializeField] private LayerMask charecterMask;
    [SerializeField] private Grid grid;
    private PlayerUIController uIController;
    private CharecterUIController charecterUIController;
    private MapManager mapManager;
    private TurnManager turnManager;
    
    private GameObject currentSelectedPlayer;
    private Vector3 selectedPlayersOriginalPosition;
    private Quaternion selectedPlayersOriginalRotation;


    private GameObject currentSelectedEnemy;
    [SerializeField] private bool inAttackMode;
    [SerializeField] private bool inWaitMode;

    private GameObject currentHighlightedTile;
    private GameObject previousTileHighlight;
    private GameObject charecterHit;

    private float offset = 1f;

    private List<GameObject> moveTilesInRange;
    private List<GameObject> attackTilesInRange;
    private List<GameObject> enemiesInRange;

    private bool attackHappening;
    private bool youAttacked;

    // Start is called before the first frame update
    void Start()
    {
        uIController = FindAnyObjectByType<PlayerUIController>();
        previousTileHighlight = null;
        moveTilesInRange = new List<GameObject>();
        attackTilesInRange = new List<GameObject>();
        mapManager = grid.GetComponent<MapManager>();
        turnManager = GameObject.Find("Charecters").GetComponent<TurnManager>();
        attackHappening = false;
        youAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool playerControl = turnManager.LocalPlay;

        if (!playerControl) {
            playerControl = turnManager.YourTurn() && turnManager.YourTurnLocal();
        }

        if (!attackHappening && playerControl)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                playerHasBeenDeselected();
            }
            if (inWaitMode && currentHighlightedTile != null)
            {
                chooseRotation();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                bool validAttackCharecter = inAttackMode && charecterHit != null && charecterHit.GetComponent<CharecterStats>().GoodGuy != currentSelectedPlayer.GetComponent<CharecterStats>().GoodGuy && mapManager.isThisCharecterInTiles(attackTilesInRange, charecterHit);
                if (validAttackCharecter)
                {
                    NetworkAttackAndOfficiallyMove();
                }
                else if (charecterHit != null && turnManager.activePlayer(charecterHit) && charecterHit != currentSelectedPlayer)
                {
                    selectPlayer();
                }
                else if (moveTilesInRange.Contains(currentHighlightedTile) && !mapManager.isTileOccupied(currentHighlightedTile))
                {
                    selectTileAndMovePlayer();
                }
            }
        }
        if(!attackHappening)
        {
            uIController.EnablePlayerUI(playerControl);
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

        cacheOriginalTransform();

        Vector3Int tileIndex = mapManager.getTileIndex(currentSelectedPlayer);
        uint move = currentSelectedPlayer.GetComponent<CharecterStats>().Move;
        uint jump = currentSelectedPlayer.GetComponent<CharecterStats>().Jump;
        uint weaponRange = currentSelectedPlayer.GetComponent<WeaponStats>().Range;
        moveTilesInRange.Clear();
        moveTilesInRange = mapManager.getTilesInRange(move, jump, tileIndex, moveTilesInRange, false);
        highlightTiles(moveTilesInRange, "inMoveRangeHighlight");
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
        highlightTiles(moveTilesInRange, "inMoveRangeHighlight");
        attackTilesInRange.Clear();
        uint weaponRange = currentSelectedPlayer.GetComponent<WeaponStats>().Range;

        currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(currentHighlightedTile);
        Vector3Int tileIndex = new Vector3Int((int)(currentSelectedPlayer.transform.position.x - offset), (int)(currentSelectedPlayer.transform.position.y - offset), (int)(currentSelectedPlayer.transform.position.z - offset));
        attackTilesInRange = mapManager.getTilesInRange(weaponRange, 1, tileIndex, attackTilesInRange, true);
        highlightTiles(attackTilesInRange, "inAttackRangeHighlight");

        checkEnemyInRange();

    }

    private void NetworkAttackAndOfficiallyMove()
    {
        attackHappening = true;
        youAttacked = true;
        uIController.DisablePlayerUI();
        currentSelectedEnemy = charecterHit;

        if(IsServer && IsClient)
        {
            attackAndOfficiallyMoveClientRPC(selectedPlayersOriginalPosition, currentSelectedPlayer.transform.position, currentSelectedEnemy.transform.position);
        }
        if(!IsServer && IsClient)
        {
            //need to tell server to check if attack is legit and then it can send clients to attack.
            checkAttackCanHappenServerRpc(selectedPlayersOriginalPosition, currentSelectedPlayer.transform.position, currentSelectedEnemy.transform.position);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void checkAttackCanHappenServerRpc(Vector3 playersOriginalPosition, Vector3 playersNewPosition, Vector3 enemyPosition)
    {

        if (canAttackHappen(playersOriginalPosition, playersNewPosition, enemyPosition))
        {
            attackAndOfficiallyMoveClientRPC(playersOriginalPosition, playersNewPosition, enemyPosition);
        }
        else
        {
            Debug.Log("client is requesting an attack but there was a descrepency between client and server");
        }
    }

    [ClientRpc]
    private void attackAndOfficiallyMoveClientRPC(Vector3 playersOriginalPosition, Vector3 playersNewPosition, Vector3 enemyPosition)
    {
        if (youAttacked)
        {
            currentSelectedPlayer = mapManager.getOccupier(playersNewPosition);
        }
        else
        {
            currentSelectedPlayer = mapManager.getOccupier(playersOriginalPosition);
        }
        currentSelectedEnemy = mapManager.getOccupier(enemyPosition);
        if(currentSelectedPlayer == null)
        {
            Debug.Log("no player found");
        }

        if (currentSelectedEnemy == null)
        {
            Debug.Log("no enemy found");
        }
        attackAndOfficiallyMove(playersNewPosition);
    }

    private void attackAndOfficiallyMove(Vector3 playerPosition)
    {
        if (!youAttacked)
        {
            currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(playerPosition);
        }
        currentSelectedPlayer.GetComponent<PlayerController>().snapRotateCharecter(currentSelectedEnemy.transform.position);
        currentSelectedPlayer.GetComponent<PlayerController>().OfficiallyMoveCharecter(currentSelectedPlayer.transform.position, currentSelectedPlayer.transform.rotation);

        uint damage = currentSelectedPlayer.GetComponent<CharecterStats>().outPutDamage();
        StartCoroutine(enemyHit(0.9f, damage));
    }
    IEnumerator enemyHit(float delayTime, uint damage)
    {
        currentSelectedPlayer.GetComponent<CharecterAnimationController>().PlayAnimation("Attack");
        yield return new WaitForSeconds(delayTime);
        currentSelectedEnemy.GetComponent<CharecterStats>().TakeHit(damage);
        currentSelectedEnemy = null;
        attackHappening = false;
        inAttackMode = false;
        if(youAttacked)
        {
            turnManager.charecterDoneAction(currentSelectedPlayer);
            youAttacked = false;
        }
        playerHasOfficialyMoved();

        yield return null;
    }
    public void playerHasOfficialyMoved()
    {
        currentSelectedPlayer = null;
        clearCharectersHighlights();
        uIController.disableWait();
        uIController.disableAttack();
    }

    public void playerHasBeenDeselected()
    {
        if(currentSelectedPlayer != null)
        {
            currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(selectedPlayersOriginalPosition, selectedPlayersOriginalRotation);
            clearCharectersHighlights();
            setInWaitMode(false);
            setInAttackMode(false);
            uIController.disableWait();
            uIController.disableAttack();
            currentSelectedPlayer = null;
        }  
    }

    private void cacheOriginalTransform()
    {
        selectedPlayersOriginalPosition = currentSelectedPlayer.transform.position;
        selectedPlayersOriginalRotation = currentSelectedPlayer.transform.rotation;
    }
    private void chooseRotation()
    {
        currentSelectedPlayer.GetComponent<PlayerController>().rotateCharecter();
        if (Input.GetMouseButtonDown(0))
        {
            setInWaitMode(false);
            turnManager.charecterDoneAction(currentSelectedPlayer);
            currentSelectedPlayer.GetComponent<PlayerController>().OfficiallyMoveCharecter(currentSelectedPlayer.transform.position, currentSelectedPlayer.transform.rotation);
            playerHasOfficialyMoved();
        }
    }
    public void clearCharectersHighlights()
    {
        highlightTiles(moveTilesInRange, "noHighlight");
        moveTilesInRange.Clear();
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
        currentSelectedPlayer.GetComponent<CharecterUIController>().setDirectionHighlight(inWaitMode);
    }
    private void highlightCurentTile()
    { 
        bool hitEqualsCurrentTile = currentHighlightedTile == previousTileHighlight;
        bool previousTileInTilesInRange = moveTilesInRange.Contains(previousTileHighlight);
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

    private bool canAttackHappen(Vector3 playersOriginalPosition, Vector3 playersNewPosition, Vector3 enemyPosition)
    {
        Debug.Log("Checking if attack can happen");
        bool checkAttackCanHappen = false;

        GameObject player = mapManager.getOccupier(playersOriginalPosition);
        GameObject enemy = mapManager.getOccupier(enemyPosition);

        if (enemy == null)
        {
            Debug.Log("no attacking player found from clients position not doing attack");
            return false;
        }
        if (player == null)
        {
            Debug.Log("no enemy found from clients position not doing attack");
            return false;
        }

        List<GameObject> testTileRange = new List<GameObject>();
        uint move = player.GetComponent<CharecterStats>().Move;
        uint jump = player.GetComponent<CharecterStats>().Jump;
        Vector3Int tileIndex = mapManager.getTileIndex(player);
        testTileRange = mapManager.getTilesInRange(move, jump, tileIndex, testTileRange, false);
        GameObject testTile = mapManager.getTile(playersNewPosition);

        checkAttackCanHappen = testTileRange.Contains(testTile);
        if (!checkAttackCanHappen)
        {
            Debug.Log("movement of player according to server is not possible not doing attack");
        }
        print(checkAttackCanHappen);
        return checkAttackCanHappen;
    }
}
