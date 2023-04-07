using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private LayerMask mapTileMask;
    [SerializeField] private LayerMask charecterMask;

    [SerializeField] private Grid grid;


    private GameObject CurrentGameObject;
    private GameObject CurrentSelectedPlayer;

    private GameObject hit;
    private GameObject charecterHit;

    private List<GameObject> tilesInRange;
    // Start is called before the first frame update
    void Start()
    {
        CurrentGameObject = null;
        tilesInRange = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (charecterHit != null)
            {
                CurrentSelectedPlayer = charecterHit;
                Vector3Int location = new Vector3Int((int)CurrentSelectedPlayer.transform.position.x, (int)CurrentSelectedPlayer.transform.position.y, (int)CurrentSelectedPlayer.transform.position.z);
                tilesInRange = grid.GetComponent<MapManager>().getTilesInRange(CurrentSelectedPlayer.GetComponent<PlayerController>().Move, location);
                highlightTiles(tilesInRange, true);
            }
            else
            {
                if (tilesInRange.Contains(hit))
                {
                    CurrentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(hit);
                    CurrentSelectedPlayer = null;
                    highlightTiles(tilesInRange, false);
                    tilesInRange.Clear();
                }
            }

        }
    }

    void FixedUpdate()
    {
        hit = GetObject(mapTileMask);
        charecterHit = GetObject(charecterMask);
        if (hit != null)
        {
            if (CurrentGameObject != null && !tilesInRange.Contains(CurrentGameObject))
            {
                CurrentGameObject.GetComponent<Highlight>().ToggleHighlight(false);
            }
            CurrentGameObject = hit;
            CurrentGameObject.GetComponent<Highlight>().ToggleHighlight(true);
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

    private void highlightTiles(List<GameObject> tiles, bool highlight)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Highlight>().ToggleHighlight(highlight);
        }
    }
}
