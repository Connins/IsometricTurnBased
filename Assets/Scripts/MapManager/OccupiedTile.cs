using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupiedTile : MonoBehaviour
{
    private GameObject _tile;
    private GameObject _occuppier;
    private Vector3Int _position;
    
    public OccupiedTile(GameObject tile, GameObject occupier, Vector3Int position)
    {
        _tile = tile;
        _occuppier = occupier;
        _position = position;
    }

    public GameObject GetTile()
    {
        return _tile;
    }

    public GameObject GetOccupier()
    {
        return _occuppier;
    }

    public Vector3Int GetPosition() 
    {
        return _position; 
    }
}
