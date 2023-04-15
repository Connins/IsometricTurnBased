using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private LayerMask mapTileMask;
    [SerializeField] private LayerMask charecterMask;

    [SerializeField] private Grid grid;
    private MapManager mapManager;
    private TurnManager turnManager;
    private GameObject CurrentGameObject;
    private GameObject CurrentSelectedPlayer;

    private GameObject hit;
    private GameObject charecterHit;

    private float offset = 1f;

    private List<GameObject> tilesInRange;
    // Start is called before the first frame update
    void Start()
    {
        CurrentGameObject = null;
        tilesInRange = new List<GameObject>();
        mapManager = grid.GetComponent<MapManager>();
        turnManager = GameObject.Find("Charecters").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (charecterHit != null && turnManager.activePlayer(charecterHit))
            {
                highlightTiles(tilesInRange, false);
                CurrentSelectedPlayer = charecterHit;
                Vector3Int location = new Vector3Int((int)(CurrentSelectedPlayer.transform.position.x - offset), (int)(CurrentSelectedPlayer.transform.position.y - offset), (int)(CurrentSelectedPlayer.transform.position.z - offset));
                uint move = CurrentSelectedPlayer.GetComponent<CharecterStats>().Move;
                uint jump = CurrentSelectedPlayer.GetComponent<CharecterStats>().Jump;
                
                tilesInRange.Clear();
                tilesInRange = mapManager.getTilesInRange(move, jump, location, tilesInRange);
                highlightTiles(tilesInRange, true);
            }
            else
            {
                if (tilesInRange.Contains(hit) && !mapManager.isTileOccupied(hit))
                {
                    CurrentSelectedPlayer.GetComponent<PlayerController>().MoveCharecter(hit);
                    turnManager.charecterMoved(CurrentSelectedPlayer);
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
