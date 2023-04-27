using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    
    private Grid grid;
    private MapManager mapManager;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindAnyObjectByType<Grid>();
        mapManager = grid.GetComponent<MapManager>();
        playerTransform = GetComponent<Transform>();
        mapManager.addToOccupied(transform.gameObject, transform.position);
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

        mapManager.removeFromOccupied(transform.position);
        playerTransform.SetPositionAndRotation(targetPosition, playerTransform.rotation);
        mapManager.addToOccupied(transform.gameObject, transform.position);
    }

    public void MoveCharecter(Vector3 target)
    {
        mapManager.removeFromOccupied(transform.position);
        playerTransform.SetPositionAndRotation(target, playerTransform.rotation);
        mapManager.addToOccupied(transform.gameObject, transform.position);
    }
}
