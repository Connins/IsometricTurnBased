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

    private List<GameObject> tilesInRange;
    private List<GameObject> attackTilesInRange;
    private List<GameObject> enemiesInRange;

    private bool attackHappening;
    private bool youAttacked;

    // Start is called before the first frame update
    void Start()
    {
        uIController = FindAnyObjectByType<PlayerUIController>();
        previousTileHighlight = null;
        tilesInRange = new List<GameObject>();
        attackTilesInRange = new List<GameObject>();
        mapManager = grid.GetComponent<MapManager>();
        turnManager = GameObject.Find("Charecters").GetComponent<TurnManager>();
        attackHappening = false;
        youAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!attackHappening && turnManager.YourTurn() && turnManager.YourTurnLocal())
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
                else if (tilesInRange.Contains(currentHighlightedTile) && !mapManager.isTileOccupied(currentHighlightedTile))
                {
                    selectTileAndMovePlayer();
                }
            }
        }
        if(!attackHappening)
        {
            uIController.EnablePlayerUI(turnManager.YourTurnLocal());
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

    private void NetworkAttackAndOfficiallyMove()
    {
        attackHappening = true;
        youAttacked = true;
        uIController.DisablePlayerUI();
        currentSelectedEnemy = charecterHit;

        if (IsServer && !IsClient)
        {
            //Not sure the below comment is true as this section of code should never be hit. Also we do update position and health in server so that is always tracked
            //Needs to do attack itself or a basic version of it with no animation, currently we do not have a server that is not a client
        }
        if(IsServer && IsClient)
        {
            attackAndOfficiallyMoveClientRPC(currentSelectedPlayer.transform.position, currentSelectedEnemy.transform.position);
        }
        if(!IsServer && IsClient)
        {
            checkAttackCanHappenServerRpc(currentSelectedPlayer.transform.position, currentSelectedEnemy.transform.position);
            //need to tell server to check if attack is legit and then it can send clients to attack.
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void checkAttackCanHappenServerRpc(Vector3 playerPosition, Vector3 enemyPosition)
    {
        Debug.Log("This check simply checks tiles are occupied not that the tiles can attack one another this should be also checked");
        if(mapManager.isTileOccupied(playerPosition) && mapManager.isTileOccupied(enemyPosition))
        {
            attackAndOfficiallyMoveClientRPC(playerPosition, enemyPosition);
        }
        else
        {
            Debug.Log("client is requesting an attack where tiles are not currently occupied");
        }
    }

    [ClientRpc]
    private void attackAndOfficiallyMoveClientRPC(Vector3 playerPosition, Vector3 enemyPosition)
    {
        currentSelectedPlayer = mapManager.getOccupier(playerPosition);
        currentSelectedEnemy = mapManager.getOccupier(enemyPosition);
        if(currentSelectedPlayer == null)
        {
            Debug.Log("no player found");
        }

        if (currentSelectedEnemy == null)
        {
            Debug.Log("no enemy found");
        }
        attackAndOfficiallyMove();
    }

    private void attackAndOfficiallyMove()
    {
        currentSelectedPlayer.GetComponent<PlayerController>().rotateCharecter(currentSelectedEnemy.transform.position);
        
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
        currentSelectedPlayer.GetComponent<CharecterUIController>().setDirectionHighlight(inWaitMode);
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
