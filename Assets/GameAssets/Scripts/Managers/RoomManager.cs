using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    [SerializeField] private TileData[] walkableTiles;
    [SerializeField] private TileData[] nonWalkableTiles;
    [SerializeField] private float baseTileHeight;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TileData GetTileData(float xCoordinate, float yCoordinate, float zCoordinate)
    {
        if (walkableTiles.Length <= 0 && nonWalkableTiles.Length <= 0)
        {
            return null;
        }

        if (walkableTiles.Length > 0)
        {
            for (int i = 0; i < walkableTiles.Length; i++)
            {
                if (walkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(xCoordinate) && walkableTiles[i].GetYCoordinate() == yCoordinate && walkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(zCoordinate))
                {
                    return walkableTiles[i];
                }
            }
        }

        if (nonWalkableTiles.Length > 0)
        {
            for (int i = 0; i < nonWalkableTiles.Length; i++)
            {
                if (nonWalkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(xCoordinate) && nonWalkableTiles[i].GetYCoordinate() == yCoordinate && nonWalkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(zCoordinate))
                {
                    return nonWalkableTiles[i];
                }
            }
        }
        
        return null;
    }

    public bool IsTileWalkable(float xCoordinate, float zCoordinate)
    {
        if (walkableTiles.Length <= 0 && nonWalkableTiles.Length <= 0)
        {
            return true;
        }

        if (walkableTiles.Length > 0)
        {
            for (int i = 0; i < walkableTiles.Length; i++)
            {
                if (walkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(xCoordinate) && walkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(zCoordinate))
                {
                    return true;
                }
            }
        }

        if (nonWalkableTiles.Length > 0)
        {
            for (int i = 0; i < nonWalkableTiles.Length; i++)
            {
                if (nonWalkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(xCoordinate) && nonWalkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(zCoordinate))
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    public bool DoesTileAllowEnemiesToSeeThroughIt(float xCoordinate, float yCoordinate, float zCoordinate)
    {
        if (walkableTiles.Length <= 0 && nonWalkableTiles.Length <= 0)
        {
            return true;
        }

        if (walkableTiles.Length > 0)
        {
            for (int i = 0; i < walkableTiles.Length; i++)
            {
                if (walkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(xCoordinate) && walkableTiles[i].GetYCoordinate() == yCoordinate && walkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(zCoordinate))
                {
                    return walkableTiles[i].CanEnemiesSeeThroughIt();
                }
            }
        }

        if (nonWalkableTiles.Length > 0)
        {
            for (int i = 0; i < nonWalkableTiles.Length; i++)
            {
                if (nonWalkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(xCoordinate) && nonWalkableTiles[i].GetYCoordinate() == yCoordinate && nonWalkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(zCoordinate))
                {
                    return nonWalkableTiles[i].CanEnemiesSeeThroughIt();;
                }
            }
        }
        
        return true;
    }
    
    public float GetTileToMoveHeight(float currentXCoordinate, float currentYCoordinate, float currentZCoordinate, float targetXCoordinate, float targetZCoordinate)
    {
        if (walkableTiles.Length <= 0 && nonWalkableTiles.Length <= 0)
        {
            return baseTileHeight;
        }
        
        TileData currentTile = GetTileData(currentXCoordinate, currentYCoordinate, currentZCoordinate);

        if (!currentTile || currentTile.GetNonAccessibleTiles().Length <= 0)
        {
            if (walkableTiles.Length > 0)
            {
                for (int i = 0; i < walkableTiles.Length; i++)
                {
                    if (walkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(targetXCoordinate) && walkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(targetZCoordinate))
                    {
                        return walkableTiles[i].GetYCoordinate();
                    }
                }
            }

            if (nonWalkableTiles.Length > 0)
            {
                for (int i = 0; i < nonWalkableTiles.Length; i++)
                {
                    if (nonWalkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(targetXCoordinate) && nonWalkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(targetZCoordinate))
                    {
                        return nonWalkableTiles[i].GetYCoordinate();
                    }
                }
            }
        
            return baseTileHeight;
        }
        
        TileData[] nonAccessibleTiles = currentTile.GetNonAccessibleTiles();
        List<TileData> tilesWithTargetCoordinates = new List<TileData>();
        
        if (walkableTiles.Length > 0)
        {
            for (int i = 0; i < walkableTiles.Length; i++)
            {
                if (walkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(targetXCoordinate) && walkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(targetZCoordinate))
                {
                    tilesWithTargetCoordinates.Add(walkableTiles[i]);
                }
            }
        }

        if (nonWalkableTiles.Length > 0)
        {
            for (int i = 0; i < nonWalkableTiles.Length; i++)
            {
                if (nonWalkableTiles[i].GetXCoordinate() == Mathf.FloorToInt(targetXCoordinate) && nonWalkableTiles[i].GetZCoordinate() == Mathf.FloorToInt(targetZCoordinate))
                {
                    tilesWithTargetCoordinates.Add(nonWalkableTiles[i]);
                }
            }
        }

        if (tilesWithTargetCoordinates.Count <= 0)
        {
            return baseTileHeight;
        }

        for (int i = 0; i < tilesWithTargetCoordinates.Count; i++)
        {
            if (tilesWithTargetCoordinates[i])
            {
                for (int j = 0; j < nonAccessibleTiles.Length; j++)
                {
                    if (tilesWithTargetCoordinates[i] && tilesWithTargetCoordinates[i].GetYCoordinate() == nonAccessibleTiles[j].GetYCoordinate())
                    {
                        tilesWithTargetCoordinates[i] = null;
                    }
                }
            }
        }

        for (int i = 0; i < tilesWithTargetCoordinates.Count; i++)
        {
            if (tilesWithTargetCoordinates[i])
            {
                return tilesWithTargetCoordinates[i].GetYCoordinate();
            }
        }
        
        return baseTileHeight;
    }

    public bool CanGoToDirectionFromCurrentTile(float xCoordinate, float yCoordinate, float zCoordinate, MoveDirection moveDirection)
    {
        if (walkableTiles.Length <= 0 && nonWalkableTiles.Length <= 0)
        {
            return true;
        }
        
        TileData currentTile = GetTileData(xCoordinate, yCoordinate, zCoordinate);

        if (!currentTile || currentTile.GetNonAccessibleDirection().Length <= 0)
        {
            return true;
        }
        
        MoveDirection[] nonAccessibleDirections = currentTile.GetNonAccessibleDirection();

        for (int i = 0; i < nonAccessibleDirections.Length; i++)
        {
            if (nonAccessibleDirections[i] == moveDirection)
            {
                return false;
            }
        }

        return true;
    }

    public void SetWalkableTiles(TileData[] newWalkableTiles)
    {
        walkableTiles = newWalkableTiles;
    }
    
    public void SetNonWalkableTiles(TileData[] newNonWalkableTiles)
    {
        nonWalkableTiles = newNonWalkableTiles;
    }

    public TileData[] GetWalkableTiles()
    {
        return walkableTiles;
    }

    public TileData[] GetNonWalkableTiles()
    {
        return nonWalkableTiles;
    }

    public TileData[] GetSceneTiles()
    {
        List<TileData> sceneTiles = walkableTiles.ToList();
        sceneTiles.AddRange(nonWalkableTiles);
        return sceneTiles.ToArray();
    }
}
