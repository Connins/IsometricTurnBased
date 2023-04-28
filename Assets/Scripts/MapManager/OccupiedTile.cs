using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupiedTile
{
    private GameObject _tile;
    private GameObject _occuppier;
    private Vector3Int _tileIndex;
    
    public OccupiedTile(GameObject tile, GameObject occupier, Vector3Int tileIndex)
    {
        _tile = tile;
        _occuppier = occupier;
        _tileIndex = tileIndex;
    }

    public GameObject GetTile()
    {
        return _tile;
    }

    public GameObject GetOccupier()
    {
        return _occuppier;
    }

    public Vector3Int GetTileIndex() 
    {
        return _tileIndex; 
    }
}
