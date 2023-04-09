using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject tileCharecterOn;
    [SerializeField] private uint move;
    [SerializeField] private uint jump;

    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
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
        tileCharecterOn = hit;
    }

    public uint Move
    {
        get { return move; }
    }
    public uint Jump   
    {
        get { return jump; }
    }
}
