using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAddToOccupied : MonoBehaviour
{
    private Grid grid;
    private MapManager mapManager;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindAnyObjectByType<Grid>();
        mapManager = grid.GetComponent<MapManager>();
        mapManager.addToOccupied(transform.gameObject, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
