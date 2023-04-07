using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject tileCharecterOn;

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

    private GameObject GetMouseGameObject(Ray ray)
    {
        // Builds a ray from camera point of view to the mouse position 
        RaycastHit hit;
        // Casts the ray and get the first game object hit 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //Instantiate(clickMarker, hit.point, Quaternion.identity); //places clickMarker at hit.point. This isn't needed, just there for visualisation. 
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
            
    }

    private void MoveCharecter(GameObject hit)
    {
        Transform target = hit.transform;
        float horOffset = 0.5f;
        float verOffset = 0.5f;
        Vector3 targetPosition = new Vector3(target.position.x + horOffset, target.parent.transform.position.y + target.localScale.y - verOffset, target.position.z + horOffset);
        playerTransform.SetPositionAndRotation(targetPosition, playerTransform.rotation);
        tileCharecterOn = hit;
    }

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject Tile = GetMouseGameObject(ray);
        if (Input.GetMouseButton(0))
        {
            MoveCharecter(Tile);
        }

    }
}
