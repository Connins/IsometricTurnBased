using System.Collections;
using System.Collections.Generic;
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

        MoveCharecter(targetPosition, playerTransform.rotation);
    }

    public void MoveCharecter(Vector3 target)
    {
        MoveCharecter(target, playerTransform.rotation);
    }
    public void MoveCharecter(Vector3 targetPosition, Quaternion targetRotation)
    {
        mapManager.removeFromOccupied(transform.position);
        playerTransform.SetPositionAndRotation(targetPosition, targetRotation);
        mapManager.addToOccupied(transform.gameObject, transform.position);

    }
    public void rotateCharecter()
    {  
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
            transform.rotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90f) * 90f, 0);
        }
    }
    public void rotateCharecter(Vector3 targetPosition)
    {        
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        transform.rotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90f) * 90f, 0);   
    }

    
}
