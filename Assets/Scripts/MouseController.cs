using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseController : NetworkBehaviour
{
    [SerializeField] private LayerMask mapTileMask;
    [SerializeField] private LayerMask charecterMask;
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject uIManager;
    [SerializeField] private GameObject yourStatsUI;
    [SerializeField] private GameObject enemyStatsUI;
    private StatsUIController yourStatsUIController;
    private StatsUIController enemyStatsUIController;
    private PlayerUIController playerUIController;
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

    //StatsUI
    private GameObject yourCharecter;
    private GameObject enemyCharecter;

    private float offset = 1f;

    private List<GameObject> moveTilesInRange;
    private List<GameObject> attackTilesInRange;
    private List<GameObject> enemiesInRange;

    private bool attackHappening;
    private bool youAttacked;

    // Start is called before the first frame update
    void Start()
    {
        playerUIController = uIManager.GetComponentInChildren<PlayerUIController>();
        yourStatsUIController = yourStatsUI.GetComponent<StatsUIController>();
        enemyStatsUIController = enemyStatsUI.GetComponent<StatsUIController>();

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
        if(turnManager.MatchHappening)
        {
            charecterControlUpdateLoop();
            statsUIControllerUpdateLoop();
            highlightCurentTile();
        }
    }
    void FixedUpdate()
    {
        currentHighlightedTile = GetObject(mapTileMask);
        charecterHit = GetObject(charecterMask);
    }
    private void charecterControlUpdateLoop()
    {
        bool playerControl = turnManager.LocalPlay;

        if (!playerControl)
        {
            playerControl = turnManager.YourTurn() && turnManager.YourTurnLocal();
        }

        if (!attackHappening && playerControl)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
        if (!attackHappening)
        {
            playerUIController.EnablePlayerUI(playerControl);
        }
    }
    private void statsUIControllerUpdateLoop()
    {
        if (Input.GetMouseButtonDown(0) && charecterHit != null)
        {
            if (charecterHit.GetComponent<CharecterStats>().GoodGuy == turnManager.YouAreGoodGuys)
            {
                yourCharecter = charecterHit;
            }
            else
            {
                enemyCharecter = charecterHit;
            }
        }

        if (yourCharecter != null)
        {
            yourStatsUIController.Charecter = yourCharecter;
        }
        else
        {
            if (charecterHit != null && charecterHit.GetComponent<CharecterStats>().GoodGuy == turnManager.YouAreGoodGuys)
            {
                yourStatsUIController.Charecter = charecterHit;
            }
            else
            {
                yourStatsUIController.Charecter = null;
            }

        }

        if (enemyCharecter != null)
        {
            enemyStatsUIController.Charecter = enemyCharecter;
        }
        else
        {
            if (charecterHit != null && charecterHit.GetComponent<CharecterStats>().GoodGuy != turnManager.YouAreGoodGuys)
            {
                enemyStatsUIController.Charecter = charecterHit;
            }
            else
            {
                enemyStatsUIController.Charecter = null;
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            killStatsUI();
        }
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
            tile.GetComponent<Highlight>().highlightMaterial(highlight);
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
        uint heightBonus = currentSelectedPlayer.GetComponent<WeaponStats>().HeightBonus;
        moveTilesInRange.Clear();
        moveTilesInRange = mapManager.getMovementTilesInRange(move, jump, tileIndex, moveTilesInRange, false);
        highlightTiles(moveTilesInRange, "inMoveRangeHighlight");
        attackTilesInRange.Clear();
        
        attackTilesInRange = mapManager.getAttackTilesInRange(weaponRange, tileIndex, heightBonus);
        highlightTiles(attackTilesInRange, "inAttackRangeHighlight");
        playerUIController.enableButton(true, "Wait");
        checkEnemyInRange();
        checkOnCapturePoint();
    }

    private void selectTileAndMovePlayer()
    {
        inAttackMode = false;
        highlightTiles(attackTilesInRange, "noHighlight");
        highlightTiles(moveTilesInRange, "inMoveRangeHighlight");
        attackTilesInRange.Clear();
        uint weaponRange = currentSelectedPlayer.GetComponent<WeaponStats>().Range;
        uint heightBonus = currentSelectedPlayer.GetComponent<WeaponStats>().HeightBonus;
        
        currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(currentHighlightedTile);
        Vector3Int tileIndex = new Vector3Int((int)(currentSelectedPlayer.transform.position.x - offset), (int)(currentSelectedPlayer.transform.position.y - offset), (int)(currentSelectedPlayer.transform.position.z - offset));
        attackTilesInRange = mapManager.getAttackTilesInRange(weaponRange, tileIndex, heightBonus);
        highlightTiles(attackTilesInRange, "inAttackRangeHighlight");

        checkEnemyInRange();
        checkOnCapturePoint();

    }

    private void NetworkAttackAndOfficiallyMove()
    {
        attackHappening = true;
        youAttacked = true;
        playerUIController.DisablePlayerUI();
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
        currentSelectedPlayer.GetComponent<PlayerController>().RotateCharecter(currentSelectedEnemy.transform.position);

        uint damage = currentSelectedPlayer.GetComponent<CharecterStats>().outPutDamage();
        float attackAnimationTime = currentSelectedPlayer.GetComponent<CharecterStats>().AttackAnimationTime;
        float angle = Quaternion.Angle(Quaternion.identity, Quaternion.Inverse(currentSelectedPlayer.transform.rotation) * currentSelectedEnemy.transform.rotation);
        StartCoroutine(enemyHit(attackAnimationTime, damage, angle));    

    }

    IEnumerator enemyHit(float delayTime, uint damage, float angle)
    {
        currentSelectedPlayer.GetComponent<CharecterAnimationController>().PlayAnimation("Attack");
        yield return new WaitForSeconds(delayTime);
        currentSelectedEnemy.GetComponent<CharecterStats>().TakeHit(damage, angle);
        attackHappening = false;
        inAttackMode = false;
        if (youAttacked)
        {
            turnManager.charecterDoneAction(currentSelectedPlayer);
            youAttacked = false;
        }
        currentSelectedPlayer.GetComponent<PlayerController>().snapRotateCharecter(currentSelectedEnemy.transform.position);
        currentSelectedEnemy = null;

        if (!IsHost)
        {
            Debug.Log("client who is not the host is updating server transform to ensure they are not snapped into position due to server change variable causing animation to change befopre this couroutine finishes");
            currentSelectedPlayer.GetComponent<PlayerController>().OfficiallyMoveCharecter(currentSelectedPlayer.transform.position, currentSelectedPlayer.transform.rotation);
        }

        ResetHighlightsAndSelectedCharecters();

        yield return null;
    }
    public void NetworkCaptureAndOfficiallyMove()
    {
        attackHappening = true;
        youAttacked = true;
        playerUIController.DisablePlayerUI();

        if (IsServer && IsClient)
        {
            CaptureAndOfficiallyMoveClientRPC(selectedPlayersOriginalPosition, currentSelectedPlayer.transform.position);
        }
        if (!IsServer && IsClient)
        {
            //need to tell server to check if capture is legit and then it can send clients to attack.
            CheckCaptureCanHappenServerRpc(selectedPlayersOriginalPosition, currentSelectedPlayer.transform.position);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckCaptureCanHappenServerRpc(Vector3 playersOriginalPosition, Vector3 playersNewPosition)
    {

        if (CanCaptureHappen(playersOriginalPosition, playersNewPosition))
        {
            CaptureAndOfficiallyMoveClientRPC(playersOriginalPosition, playersNewPosition);
        }
        else
        {
            Debug.Log("client is requesting a capture but there was a descrepency between client and server");
        }
    }

    [ClientRpc]
    private void CaptureAndOfficiallyMoveClientRPC(Vector3 playersOriginalPosition, Vector3 playersNewPosition)
    {
        if (youAttacked)
        {
            currentSelectedPlayer = mapManager.getOccupier(playersNewPosition);
        }
        else
        {
            currentSelectedPlayer = mapManager.getOccupier(playersOriginalPosition);
        }

        if (currentSelectedPlayer == null)
        {
            Debug.Log("no player found");
        }

        CaptureAndOfficiallyMove(playersNewPosition);
    }

    private void CaptureAndOfficiallyMove(Vector3 playerPosition)
    {


        if (!youAttacked)
        {
            currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(playerPosition);
        }
        //currentSelectedPlayer.GetComponent<PlayerController>().RotateCharecter(currentSelectedEnemy.transform.position);

        int captrurePoints = currentSelectedPlayer.GetComponent<CharecterStats>().Health;
        bool goodGuy = currentSelectedPlayer.GetComponent<CharecterStats>().GoodGuy;
        float captureAnimationTime = currentSelectedPlayer.GetComponent<CharecterStats>().AttackAnimationTime;

        GameObject capturePoint = mapManager.getTile(currentSelectedPlayer);
        capturePoint.GetComponent<CaptureMechanics>().beingCaptured(captrurePoints, goodGuy);
        
        attackHappening = false;
        
        if (youAttacked)
        {
            turnManager.charecterDoneAction(currentSelectedPlayer);
            youAttacked = false;
        }

        if (!IsHost)
        {
            currentSelectedPlayer.GetComponent<PlayerController>().OfficiallyMoveCharecter(currentSelectedPlayer.transform.position, currentSelectedPlayer.transform.rotation);
        }

        ResetHighlightsAndSelectedCharecters();


    }
    public void ResetHighlightsAndSelectedCharecters()
    {
        currentSelectedPlayer = null;
        yourCharecter = null;
        enemyCharecter = null;
        clearCharectersHighlights();
        playerUIController.DefaultPlayerUI();
    }

    public void playerHasBeenDeselected()
    {
        if(currentSelectedPlayer != null)
        {
            currentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(selectedPlayersOriginalPosition, selectedPlayersOriginalRotation);
            clearCharectersHighlights();
            setInWaitMode(false);
            setInAttackMode(false);
            playerUIController.DefaultPlayerUI();
            currentSelectedPlayer = null;
            killStatsUI();
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
            ResetHighlightsAndSelectedCharecters();
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
            previousTileHighlight.GetComponent<Highlight>().highlightMaterial("noHighlight");
        }

        if (currentHighlightedTile && !currentTileInAttackTilesInRange)
        {
            currentHighlightedTile.GetComponent<Highlight>().highlightMaterial("inMoveRangeHighlight");
            previousTileHighlight = currentHighlightedTile;
        }
    }

    private void checkEnemyInRange()
    {
        bool goodGuy = currentSelectedPlayer.GetComponent<CharecterStats>().GoodGuy;

        bool enemyInRange = mapManager.isEnemyInTiles(attackTilesInRange, goodGuy);

        if (enemyInRange)
        {
            playerUIController.enableButton(true, "Attack");
        }
        else
        {
            playerUIController.enableButton(false, "Attack");
        }
    }

    private void checkOnCapturePoint()
    {
        GameObject tile = mapManager.getTile(currentSelectedPlayer.transform.position);
        if (mapManager.isTileACapturePoint(tile))
        {
            playerUIController.enableButton(true, "Capture");
        }
        else
        {
            playerUIController.enableButton(false, "Capture");
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
            Debug.Log("no enemy found from clients position not doing attack");
            return false;
        }
        if (player == null)
        {
            Debug.Log("no attacking player found from clients position not doing attack");
            return false;
        }

        bool isMovementAllowed = player.GetComponent<PlayerController>().canMovementHappen(playersNewPosition);

        if (!isMovementAllowed)
        {
            Debug.Log("movement of player according to server is not possible not doing attack");
        }
        bool isAttackAllowed = isAttackInRange(playersOriginalPosition, playersNewPosition, enemyPosition);
        
        if (!isAttackAllowed)
        {
            Debug.Log("Enemy is not in range of player on server not doing attack");
        }
        checkAttackCanHappen = isMovementAllowed && isAttackAllowed;

        return checkAttackCanHappen;
    }

    private bool CanCaptureHappen(Vector3 playersOriginalPosition, Vector3 playersNewPosition)
    {
        Debug.Log("Checking if capture can happen");
        bool checkCaptureCanHappen = false;

        GameObject player = mapManager.getOccupier(playersOriginalPosition);

        if (player == null)
        {
            Debug.Log("no player found from clients position not doing attack");
            return false;
        }

        bool isMovementAllowed = player.GetComponent<PlayerController>().canMovementHappen(playersNewPosition);

        if (!isMovementAllowed)
        {
            Debug.Log("movement of player according to server is not possible not doing capture");
        }

        GameObject tile = mapManager.getTile(playersNewPosition);
        bool isCapturePoint = mapManager.isTileACapturePoint(tile);

        if (!isCapturePoint)
        {
            Debug.Log("Position of player is not a capture point not doing capture");
        }
        
        checkCaptureCanHappen = isMovementAllowed && isCapturePoint;

        return checkCaptureCanHappen;
    }
    private bool isAttackInRange(Vector3 playersOriginalPosition, Vector3 playersNewPosition, Vector3 enemyPosition)
    {
        GameObject player = mapManager.getOccupier(playersOriginalPosition);
        uint weaponRange = player.GetComponent<WeaponStats>().Range;
        uint heightBonus = player.GetComponent<WeaponStats>().HeightBonus;
        List<GameObject> testAttackTileRange = mapManager.getAttackTilesInRange(weaponRange, new Vector3Int((int)playersNewPosition.x - 1, (int)playersNewPosition.y - 1, (int)playersNewPosition.z - 1), heightBonus);
        GameObject enemyTile = mapManager.getTile(enemyPosition);
        bool isAttackAllowed = testAttackTileRange.Contains(enemyTile);
        if (!isAttackAllowed)
        {
            Debug.Log("Enemy is not in range of player on server");
        }
        return isAttackAllowed;
    }

    public void killStatsUI()
    {
        yourCharecter = null;
        yourStatsUIController.Charecter = null;
        enemyCharecter = null;
        enemyStatsUIController.Charecter = null;
    }
}
