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
    // Start is called before the first frame update
    void Start()
    {
        grid = FindAnyObjectByType<Grid>();
        mapManager = grid.GetComponent<MapManager>();
        playerTransform = GetComponent<Transform>();
        mapManager.addToOccupied(transform.gameObject, transform.position);
        turnManager = gameObject.GetComponentInParent<TurnManager>();
    }

    public override void OnNetworkSpawn()
    {
        //This needs to occur as when client connects it changes transform of game object to host automatically 
        //I cannot seem to stop this occurring
        //so we need to sort out occupied tiles with this.
        mapManager.removeFromOccupied(transform.gameObject);
        mapManager.addToOccupied(transform.gameObject, transform.position);
    }
    private void OnEnable()
    {
        serverTransformState.OnValueChanged += OnServerStateChanged;
    }

    private void OnServerStateChanged(TransformState previousValue, TransformState serverState)
    {
        if (IsServer)
        {
            return;
        }
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
        mapManager.removeFromOccupied(transform.position);
        playerTransform.SetPositionAndRotation(targetPosition, targetRotation);
        mapManager.addToOccupied(transform.gameObject, transform.position);
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
        if (IsServer)
        {
            UpdateServerTransform(targetPosition, targetRotation); 
        }
        else
        {
            Moving(targetPosition, targetRotation);
            UpdateServerTransformServerRpc(targetPosition, targetRotation);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateServerTransformServerRpc(Vector3 targetPosition, Quaternion targetRotation)
    {
        Debug.Log("Should do a check to see if move is possible to avoid cheaters");

        if (IsServer && !IsClient)
        {
            Moving(targetPosition, targetRotation);
        }
        UpdateServerTransform(targetPosition, targetRotation); 
    }

    public void UpdateServerTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        //MoveCharecterClientRPC(targetPosition, targetRotation);
        Moving(targetPosition, targetRotation);

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
            rotateCharecter(targetPosition);
        }
    }
    public void rotateCharecter(Vector3 targetPosition)
    {        
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        Quaternion targetRotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90f) * 90f, 0);
        MoveCharecter(transform.position, targetRotation);
    }

    //this only works if server is also a client as ServerRPC command just calls wanted function
    //it does not call a clientRPC command
    public void NetworkOfficiallyMoved()
    {
        if (IsServer)
        {
            OfficiallyMovedClientRpc();
        }
        else
        {
            OfficiallyMovedServerRpc();
            turnManager.charecterDoneAction(gameObject);
        }
    }

    [ClientRpc]
    private void OfficiallyMovedClientRpc()
    {
        turnManager.charecterDoneAction(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OfficiallyMovedServerRpc()
    {
        turnManager.charecterDoneAction(gameObject);
    }
}
