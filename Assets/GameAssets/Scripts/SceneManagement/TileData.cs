using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileType { Walkable, NonWalkable, Structural }

[ExecuteInEditMode]
public class TileData : MonoBehaviour
{
    [SerializeField] private TileType tileType = TileType.Walkable;
    [SerializeField] private bool canEnemiesSeeThroughIt = true;
    [SerializeField] private TileData[] nonAccessibleTiles;
    [SerializeField] private MoveDirection[] nonAccessibleDirections;
    [SerializeField] private bool hideEditModeTileOnPlay;
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
        
        TileEditMode tileEditMode = GetComponent<TileEditMode>();
        
        if (!hideEditModeTileOnPlay)
        {
            return;
        }
        
        for (int i = 0; i < _transform.childCount; i++) {
            Destroy(_transform.GetChild(i).gameObject);
        }
        
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (tileEditMode)
        {
            tileEditMode.enabled = false;
        }
        if (!meshRenderer)
        {
            return;
        }
        meshRenderer.enabled = false;
    }

    public int GetXCoordinate()
    {
        return Mathf.RoundToInt(_transform.position.x);
    }
    
    public float GetYCoordinate()
    {
        return _transform.position.y;
    }

    public int GetZCoordinate()
    {
        return Mathf.RoundToInt(_transform.position.z);
    }

    public TileType GetTileType()
    {
        return tileType;
    }

    public bool CanEnemiesSeeThroughIt()
    {
        return canEnemiesSeeThroughIt;
    }

    public TileData[] GetNonAccessibleTiles()
    {
        return nonAccessibleTiles;
    }

    public MoveDirection[] GetNonAccessibleDirection()
    {
        return nonAccessibleDirections;
    }

    public void AddNonAccessibleTile(TileData nonAccessibleTile)
    {
        List<TileData> nonAccessibleTilesList = nonAccessibleTiles.ToList();
        nonAccessibleTilesList.Add(nonAccessibleTile);
        nonAccessibleTiles = nonAccessibleTilesList.ToArray();
    }
}
