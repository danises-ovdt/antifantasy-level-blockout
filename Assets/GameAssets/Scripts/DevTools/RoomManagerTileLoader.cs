using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RoomManager))]
public class RoomManagerTileLoader : MonoBehaviour
{
    [SerializeField] private Transform walkableTilesTransform;
    [SerializeField] private Transform nonWalkableTilesTransform;
    [SerializeField] private Transform structuralTilesTransform;
    private RoomManager _roomManager;
    
    private void OnEnable()
    {
        if (!_roomManager)
        {
            _roomManager = GetComponent<RoomManager>();
        }
        
        TileData[] sceneTiles = FindObjectsOfType<TileData>();
        List<TileData> walkableTiles = new List<TileData>();
        List<TileData> nonWalkableTiles = new List<TileData>();
        List<TileData> structuralTiles = new List<TileData>();

        for (int i = 0; i < sceneTiles.Length; i++)
        {
            if (sceneTiles[i].GetTileType() == TileType.NonWalkable)
            {
                nonWalkableTiles.Add(sceneTiles[i]);
            }
            else if (sceneTiles[i].GetTileType() == TileType.Structural)
            {
                structuralTiles.Add(sceneTiles[i]);
            }
            else
            {
                walkableTiles.Add(sceneTiles[i]);
            }
        }

        if (walkableTilesTransform)
        {
            for (int i = 0; i < walkableTiles.Count; i++)
            {
                walkableTiles[i].transform.parent = walkableTilesTransform;
            }
            
            for (int i = 0; i < walkableTilesTransform.childCount; i++) 
            {
                Transform child = walkableTilesTransform.GetChild(i);
                child.name = $"Tile ({child.position.x}, {Mathf.FloorToInt(child.position.y)}, {child.position.z})";
            }
            
            List<Transform> children = new List<Transform>();
        
            for (int i = 0; i < walkableTilesTransform.childCount; i++)
            {
                Transform child = walkableTilesTransform.GetChild(i);
                if (children.Count <= 0 || children.Any(childItem => childItem.name != child.name))
                {
                    children.Add(child);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            for (int i = 0; i < children.Count; i++)
            {
                children[i].parent = null;
            }
        
            children.Sort((Transform firstTransform, Transform secondTransform) =>  {
                return firstTransform.name.CompareTo(secondTransform.name);
            });
        
            foreach (Transform child in children)
            {
                child.parent = walkableTilesTransform;
            }
        }
        
        if (nonWalkableTilesTransform)
        {
            for (int i = 0; i < nonWalkableTiles.Count; i++)
            {
                nonWalkableTiles[i].transform.parent = nonWalkableTilesTransform;
            }
            
            for (int i = 0; i < nonWalkableTilesTransform.childCount; i++) 
            {
                Transform child = nonWalkableTilesTransform.GetChild(i);
                child.name = $"Tile ({child.position.x}, {Mathf.FloorToInt(child.position.y)}, {child.position.z})";
            }
            
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < nonWalkableTilesTransform.childCount; i++)
            {
                Transform child = nonWalkableTilesTransform.GetChild(i);
                if (children.Count <= 0 || children.Any(childItem => childItem.name != child.name))
                {
                    children.Add(child);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            for (int i = 0; i < children.Count; i++)
            {
                children[i].parent = null;
            }

            children.Sort((Transform firstTransform, Transform secondTransform) =>  {
                return firstTransform.name.CompareTo(secondTransform.name);
            });
        
            foreach (Transform child in children)
            {
                child.parent = nonWalkableTilesTransform;
            }
        }
        
        if (structuralTilesTransform)
        {
            for (int i = 0; i < structuralTiles.Count; i++)
            {
                structuralTiles[i].transform.parent = structuralTilesTransform;
            }
            
            for (int i = 0; i < structuralTilesTransform.childCount; i++) 
            {
                Transform child = structuralTilesTransform.GetChild(i);
                child.name = $"Tile ({child.position.x}, {Mathf.FloorToInt(child.position.y)}, {child.position.z})";
            }
            
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < structuralTilesTransform.childCount; i++)
            {
                Transform child = structuralTilesTransform.GetChild(i);
                if (children.Count <= 0 || children.Any(childItem => childItem.name != child.name))
                {
                    children.Add(child);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            for (int i = 0; i < children.Count; i++)
            {
                children[i].parent = null;
            }

            children.Sort((Transform firstTransform, Transform secondTransform) =>  {
                return firstTransform.name.CompareTo(secondTransform.name);
            });
        
            foreach (Transform child in children)
            {
                child.parent = structuralTilesTransform;
            }
        }
        
        _roomManager.SetWalkableTiles(walkableTiles.ToArray());
        _roomManager.SetNonWalkableTiles(nonWalkableTiles.ToArray());

        enabled = false;
    }
}
