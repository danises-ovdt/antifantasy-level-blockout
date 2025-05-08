using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(TileData))]
public class TileEditMode : MonoBehaviour
{
    [SerializeField] private TMP_Text coordinatesText;
    [SerializeField] private GameObject canSeeThroughIcon;
    [SerializeField] private Material walkableTileMaterial;
    [SerializeField] private Material nonWalkableTileMaterial;
    [SerializeField] private Material structuralTileMaterial;
    private Transform _transform;
    private MeshRenderer _meshRenderer;
    private TileData _tileData;
    private void Start()
    {
        _transform = transform;
        _meshRenderer = GetComponent<MeshRenderer>();
        _tileData = GetComponent<TileData>();
    }

    private void Update()
    {
        if (!_tileData)
        {
            _tileData = GetComponent<TileData>();
        }
        
        switch (_tileData.GetTileType())
        {
            case TileType.Walkable:
                _meshRenderer.material = walkableTileMaterial;
                
                break;
            case TileType.NonWalkable:
                _meshRenderer.material = nonWalkableTileMaterial;
                break;
            case TileType.Structural:
                _meshRenderer.material = structuralTileMaterial;
                break;
        }
        
        if (canSeeThroughIcon)
        {
            canSeeThroughIcon.SetActive(!_tileData.CanEnemiesSeeThroughIt());
        }

        if (!coordinatesText)
        {
            return;
        }
        
        coordinatesText.text = $"{_transform.position.x}, {Mathf.FloorToInt(_transform.position.y)}, {_transform.position.z}";
    }
}
