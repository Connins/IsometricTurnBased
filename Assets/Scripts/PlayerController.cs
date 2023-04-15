using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    
    private Grid grid;
    private MapManager mapManager;

    private GameObject tileCharecterOn;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindAnyObjectByType<Grid>();
        mapManager = grid.GetComponent<MapManager>();
        playerTransform = GetComponent<Transform>();
        tileCharecterOn = mapManager.getTile(playerTransform.position);
        mapManager.addTileToOccupied(tileCharecterOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCharecter(GameObject hit)
    {
        Transform target = hit.transform;
        float horOffset = 0.5f;
        float verOffset = 0f;
        Vector3 targetPosition = new Vector3(target.position.x + horOffset, target.parent.transform.position.y + target.localScale.y - verOffset, target.position.z + horOffset);
        playerTransform.SetPositionAndRotation(targetPosition, playerTransform.rotation);

        mapManager.removeTileFromOccupied(tileCharecterOn);
        tileCharecterOn = hit;
        mapManager.addTileToOccupied(tileCharecterOn);
    }

}
