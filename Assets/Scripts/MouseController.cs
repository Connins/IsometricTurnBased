using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private LayerMask mapTileMask;
    [SerializeField] private LayerMask charecterMask;

    private GameObject CurrentGameObject;
    private GameObject CurrentSelectedPlayer;
    // Start is called before the first frame update
    void Start()
    {
        CurrentGameObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        GameObject hit = GetObject(mapTileMask);
        if (hit != null)
        {
            if (CurrentGameObject != null)
            {
                CurrentGameObject.GetComponent<Highlight>().ToggleHighlight(false);
            }
            CurrentGameObject = hit;
            CurrentGameObject.GetComponent<Highlight>().ToggleHighlight(true);
        }

        if (Input.GetMouseButton(0))
        {
            GameObject charecterHit = GetObject(charecterMask);
            if (charecterHit != null)
            {
                CurrentSelectedPlayer = charecterHit;
            }
            else
            {
                CurrentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(hit);
            }
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
}
