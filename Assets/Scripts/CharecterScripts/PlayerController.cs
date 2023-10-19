using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.CharecterScripts;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private Grid grid;
    private MapManager mapManager;
    private Transform playerTransform;
    private TurnManager turnManager;

    NetworkVariable<TransformState> serverTransformState = new NetworkVariable<TransformState>();

    private void Awake()
    {
        grid = FindAnyObjectByType<Grid>();
        mapManager = grid.GetComponent<MapManager>();
        playerTransform = GetComponent<Transform>();
        turnManager = gameObject.GetComponentInParent<TurnManager>();
    }

    public override void OnNetworkSpawn()
    {
        //This needs to occur as when client connects it changes transform of game object to host automatically 
        //I cannot seem to stop this occurring
        //so we need to sort out occupied tiles with this.
        mapManager.removeFromOccupied(gameObject);
        mapManager.addToOccupied(gameObject, transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        mapManager.addToOccupied(transform.gameObject, transform.position);
    }
    private void OnEnable()
    {
        serverTransformState.OnValueChanged += OnServerStateChanged;
    }

    private void OnServerStateChanged(TransformState previousValue, TransformState serverState)
    {
        TransformState clientTransformState =  new TransformState()
        {
            tick = 0,
            position = transform.position,
            rotation = transform.rotation,
        };

        if (clientTransformState.position != serverState.position || clientTransformState.rotation != serverState.rotation)
        {

            Moving(serverState.position, serverState.rotation);
            Debug.Log("client position being updated according to the server popsition, This update is not based on ticks which it should be");
        }
    }
    public void Moving(Vector3 targetPosition, Quaternion targetRotation)
    {
        mapManager.removeFromOccupied(gameObject);
        playerTransform.SetPositionAndRotation(targetPosition, targetRotation);
        mapManager.addToOccupied(gameObject, transform.position);
    }
    public void MoveCharecter(GameObject hit)
    {
        Transform target = hit.transform;
        float horOffset = 0.5f;
        float verOffset = 0f;
        Vector3 targetPosition = new Vector3(target.position.x + horOffset, target.parent.transform.position.y + target.localScale.y - verOffset, target.position.z + horOffset);

        MoveCharecter(targetPosition, playerTransform.rotation);
    }

    public void MoveCharecter(Vector3 target)
    {
        MoveCharecter(target, playerTransform.rotation);
    }

    public void MoveCharecter(Vector3 targetPosition, Quaternion targetRotation)
    {
        Moving(targetPosition, targetRotation);
    }

    public void OfficiallyMoveCharecter(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (IsServer)
        {
            UpdateServerTransform(targetPosition, targetRotation);
        }
        else
        {
            UpdateServerTransformServerRpc(targetPosition, targetRotation);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdateServerTransformServerRpc(Vector3 targetPosition, Quaternion targetRotation)
    {
        Debug.Log("Checking if movement can happen before updating");
        if (canMovementHappen(targetPosition))
        {
            UpdateServerTransform(targetPosition, targetRotation);
        }
        else
        {
            Debug.Log("movement of player according to server is not possible not updating server transform state");
        }
    }


    public bool canMovementHappen(Vector3 targetPosition)
    {
        bool isMovementAllowed = false;

        uint move = GetComponent<CharecterStats>().Move;
        uint jump = GetComponent<CharecterStats>().Jump;
        bool goodGuy = GetComponent<CharecterStats>().GoodGuy;
        Vector3Int tileIndex = mapManager.getTileIndex(transform.gameObject);
        List<GameObject> testMovementTileRange = new List<GameObject>();
        testMovementTileRange = mapManager.getMovementTilesInRange(move, jump, tileIndex, testMovementTileRange, false, goodGuy);
        GameObject testTile = mapManager.getTile(targetPosition);
        isMovementAllowed = testMovementTileRange.Contains(testTile);

        if (!isMovementAllowed)
        {
            Debug.Log("movement of player according to server is not possible");
        }

        return isMovementAllowed;
    }
    public void UpdateServerTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        TransformState state = new TransformState()
        {
            tick = 0,
            position = targetPosition,
            rotation = targetRotation,
        };

        serverTransformState.Value = state;
    }
    public void rotateCharecter()
    {  
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            snapRotateCharecter(targetPosition);
        }
    }

    public void RotateCharecter(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
    }
    public void snapRotateCharecter(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        float offset = 0.001f;
        Quaternion targetRotation = Quaternion.Euler(0, Mathf.Round((transform.rotation.eulerAngles.y + offset) / 90f) * 90f, 0);
        MoveCharecter(transform.position, targetRotation);
    }

}
