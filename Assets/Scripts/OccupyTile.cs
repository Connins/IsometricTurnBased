using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyTile : MonoBehaviour
{
    private Grid grid;
    private MapManager mapManager;

    private GameObject tileObjectOn;
    private Transform objectTransform;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindAnyObjectByType<Grid>();
        mapManager = grid.GetComponent<MapManager>();
        objectTransform = GetComponent<Transform>();
        tileObjectOn = mapManager.getTile(objectTransform.position);
        mapManager.addTileToOccupied(tileObjectOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
