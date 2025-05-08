using UnityEngine;

[RequireComponent(typeof(EnemyIdleState), typeof(EnemyWanderState), typeof(EnemyChaseState))]

public class EnemyStateMotor : MonoBehaviour
{
    [SerializeField] private TileData originTile;
    [SerializeField] private TileData[] nonWalkableTilesThatEnemyCanPassThrough;
    [SerializeField] private TileData[] walkableTilesThatEnemyCannotPassThrough;
    [SerializeField] private MoveDirection initialMoveDirection = MoveDirection.South;
    [SerializeField, Range(1, 100)] private int tileXMovementRangeByOrigin;
    [SerializeField, Range(1, 100)] private int tileZMovementRangeByOrigin;
    [SerializeField] private Transform pointToMove;
    [SerializeField, Range(1, 10)] private int sightRange;
    [SerializeField] private bool isPassive;
    [SerializeField] private bool canWander;
    [SerializeField] private bool canChase;
    [SerializeField] private CharacterVisualsController characterVisualsController;
    private EnemyBaseState _state;
    private MoveDirection _currentMoveDirection;

    private void Start()
    {
        if (!originTile || !pointToMove)
        {
            Destroy(gameObject);
        }
        
        if (pointToMove)
        {
            pointToMove.parent = null;
        }
        
        _currentMoveDirection = initialMoveDirection;
        _state = GetComponent<EnemyIdleState>();
        _state.Construct();
    }

    private void Update()
    {
        _state.Transition();
        _state.UpdateState();
    }
    
    public void ChangeState(EnemyBaseState newState)
    {
        _state.Destruct();
        _state = newState;
        _state.Construct();
    }

    public bool CanEnemyPassThroughNonWalkableTile(int xCoordinate, int zCoordinate)
    {
        if (nonWalkableTilesThatEnemyCanPassThrough.Length <= 0)
        {
            return false;
        }

        for (int i = 0; i < nonWalkableTilesThatEnemyCanPassThrough.Length; i++)
        {
            if (nonWalkableTilesThatEnemyCanPassThrough[i].GetXCoordinate() == xCoordinate && nonWalkableTilesThatEnemyCanPassThrough[i].GetZCoordinate() == zCoordinate)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool CanEnemyPassThroughWalkableTile(int xCoordinate, int zCoordinate)
    {
        if (walkableTilesThatEnemyCannotPassThrough.Length <= 0)
        {
            return true;
        }

        for (int i = 0; i < walkableTilesThatEnemyCannotPassThrough.Length; i++)
        {
            if (walkableTilesThatEnemyCannotPassThrough[i].GetXCoordinate() == xCoordinate && walkableTilesThatEnemyCannotPassThrough[i].GetZCoordinate() == zCoordinate)
            {
                return false;
            }
        }

        return true;
    }

    public TileData GetOriginTile()
    {
        return originTile;
    }

    public int GetTileXMovementRangeByOrigin()
    {
        return tileXMovementRangeByOrigin;
    }

    public int GetTileZMovementRangeByOrigin()
    {
        return tileZMovementRangeByOrigin;
    }

    public Transform GetPointToMove()
    {
        return pointToMove;
    }

    public int GetSightRange()
    {
        return sightRange;
    }

    public bool IsPassive()
    {
        return isPassive;
    }

    public bool CanWander()
    {
        return canWander;
    }

    public bool CanChase()
    {
        return canChase;
    }

    public MoveDirection GetCurrentMoveDirection()
    {
        return _currentMoveDirection;
    }

    public void SetCurrentMoveDirection(MoveDirection moveDirection)
    {
        _currentMoveDirection = moveDirection;
    }

    public CharacterVisualsController GetCharacterVisualsController()
    {
        return characterVisualsController;
    }
}
