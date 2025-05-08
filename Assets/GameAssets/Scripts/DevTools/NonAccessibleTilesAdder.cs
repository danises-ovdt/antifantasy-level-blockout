using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TileData))]
public class NonAccessibleTilesAdder : MonoBehaviour
{
    [SerializeField] private int tileXCoordinate = -999;
    [SerializeField] private float tileYCoordinate = -999f;
    [SerializeField] private int tileZCoordinate = -999;
    private TileData _tileData;
    private bool _canExecute;

    private void Start()
    {
        _canExecute = true;
        enabled = false;
    }

    private void OnEnable()
    {
        if (!_canExecute)
        {
            return;
        }
        if (!_tileData)
        {
            _tileData = GetComponent<TileData>();    
        }
        
        TileData[] sceneTiles = FindObjectsOfType<TileData>();

        for (int i = 0; i < sceneTiles.Length; i++)
        {
            if (sceneTiles[i].GetXCoordinate() == tileXCoordinate && sceneTiles[i].GetYCoordinate() == tileYCoordinate && sceneTiles[i].GetZCoordinate() == tileZCoordinate)
            {
                _tileData.AddNonAccessibleTile(sceneTiles[i]);
                break;
            }
        }

        enabled = false;
    }
}
