using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class EnemyWanderState : EnemyBaseState
{
    [SerializeField, Range(0, 100)] private int maxRandomValueForRotatingAction;
    [SerializeField, Range(1, 50)] private int timesToTryToGetNewDirectionWhenOnlyRotating;
    [SerializeField, Range(1, 50)] private int timesToTryMoveBeforeReturningToIdle;
    [SerializeField, Range(0f, 10f)] private float enemyMoveTime;
    [SerializeField, Range(0f, 10f)] private float minTimeToGoIdleWanderAfterOnlyRotating;
    [SerializeField, Range(0f, 10f)] private float maxTimeToGoIdleWanderAfterOnlyRotating;
    [SerializeField, Range(0f, 10f)] private float minTimeToGoIdleWanderAfterMoving;
    [SerializeField, Range(0f, 10f)] private float maxTimeToGoIdleWanderAfterMoving;
    private MoveDirection _moveDirection;
    private Coroutine _goIdleAfterOnlyRotatingCoroutine;
    private Coroutine _goIdleAfterMovingCoroutine;
    private int _timesTriedToMoveBeforeReturningToIdle;
    private int _timesTriedToGetNewDirectionWhenOnlyRotating;
    private bool _moveDirectionIsSet;
    private bool _goIdle;
    private bool _mainCharacterSpotted;
    
    private void MoveCharacterToPoint()
    {
        Transform.DOKill();
        
        CanCheckMovement = false;
        
        Tweener movementTweener = Transform.DOMove(EnemyStateMotor.GetPointToMove().position, enemyMoveTime).SetEase(Ease.Linear);
        movementTweener.OnComplete(() =>
        {
            CanCheckMovement = true;
            Transform.DOKill();
            _goIdleAfterMovingCoroutine = StartCoroutine(GoIdleAfterMoving());
        });
    }

    private void SetMoveDirectionOnOnlyRotation()
    {
        if (_moveDirectionIsSet)
        {
            return;
        }
        
        _moveDirection = (MoveDirection)Random.Range(1, Enum.GetNames(typeof(MoveDirection)).Length);
        _timesTriedToGetNewDirectionWhenOnlyRotating++;
        if (_timesTriedToGetNewDirectionWhenOnlyRotating >= timesToTryToGetNewDirectionWhenOnlyRotating)
        {
            _moveDirectionIsSet = true;
            return;
        }
        
        if (_moveDirection == EnemyStateMotor.GetCurrentMoveDirection())
        {
            SetMoveDirectionOnOnlyRotation();
        }
        else
        {
            _moveDirectionIsSet = true;
        }
    }
    
    private void SetMoveDirectionToGo()
    {
        if (_moveDirectionIsSet)
        {
            return;
        }
        
        _moveDirection = (MoveDirection)Random.Range(1, Enum.GetNames(typeof(MoveDirection)).Length);
        _timesTriedToMoveBeforeReturningToIdle++;
        if (_timesTriedToMoveBeforeReturningToIdle >= timesToTryMoveBeforeReturningToIdle)
        {
            return;
        }
        switch (_moveDirection)
        {
            case MoveDirection.North:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x, EnemyStateMotor.GetPointToMove().position.z + 1) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z + 1))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x, EnemyStateMotor.GetPointToMove().position.z + 1) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z + 1)))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetZCoordinate() - (EnemyStateMotor.GetPointToMove().position.z + 1))  > EnemyStateMotor.GetTileZMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(0f, 0f, 1f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
            case MoveDirection.South:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x, EnemyStateMotor.GetPointToMove().position.z - 1) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z - 1))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x, EnemyStateMotor.GetPointToMove().position.z - 1) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z - 1)))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetZCoordinate() - (EnemyStateMotor.GetPointToMove().position.z - 1))  > EnemyStateMotor.GetTileZMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(0f, 0f, -1f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
            case MoveDirection.East:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x + 1, EnemyStateMotor.GetPointToMove().position.z ) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x + 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x + 1, EnemyStateMotor.GetPointToMove().position.z) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x + 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z )))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetXCoordinate() - (EnemyStateMotor.GetPointToMove().position.x + 1))  > EnemyStateMotor.GetTileXMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(1f, 0f, 0f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
            case MoveDirection.West:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x - 1, EnemyStateMotor.GetPointToMove().position.z ) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x - 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x - 1, EnemyStateMotor.GetPointToMove().position.z) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x - 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z )))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetXCoordinate() - (EnemyStateMotor.GetPointToMove().position.x - 1))  > EnemyStateMotor.GetTileXMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(-1f, 0f, 0f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
            case MoveDirection.NorthEast:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x + 1, EnemyStateMotor.GetPointToMove().position.z + 1) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x + 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z + 1))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x + 1, EnemyStateMotor.GetPointToMove().position.z + 1) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x + 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z + 1)))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetZCoordinate() - (EnemyStateMotor.GetPointToMove().position.z + 1))  > EnemyStateMotor.GetTileZMovementRangeByOrigin()
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetXCoordinate() - (EnemyStateMotor.GetPointToMove().position.x + 1))  > EnemyStateMotor.GetTileXMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(1f, 0f, 1f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
            case MoveDirection.SouthEast:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x + 1, EnemyStateMotor.GetPointToMove().position.z - 1) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x + 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z - 1))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x + 1, EnemyStateMotor.GetPointToMove().position.z - 1) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x + 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z - 1)))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetZCoordinate() - (EnemyStateMotor.GetPointToMove().position.z - 1))  > EnemyStateMotor.GetTileZMovementRangeByOrigin()
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetXCoordinate() - (EnemyStateMotor.GetPointToMove().position.x + 1))  > EnemyStateMotor.GetTileXMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(1f, 0f, -1f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
            case MoveDirection.SouthWest:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x - 1, EnemyStateMotor.GetPointToMove().position.z - 1) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x - 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z - 1))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x - 1, EnemyStateMotor.GetPointToMove().position.z - 1) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x - 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z - 1)))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetZCoordinate() - (EnemyStateMotor.GetPointToMove().position.z - 1))  > EnemyStateMotor.GetTileZMovementRangeByOrigin()
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetXCoordinate() - (EnemyStateMotor.GetPointToMove().position.x - 1))  > EnemyStateMotor.GetTileXMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(-1f, 0f, -1f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
            case MoveDirection.NorthWest:
                if ((!RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x - 1, EnemyStateMotor.GetPointToMove().position.z + 1) && !EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x - 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z + 1))) 
                    || (RoomManager.Instance.IsTileWalkable(EnemyStateMotor.GetPointToMove().position.x - 1, EnemyStateMotor.GetPointToMove().position.z + 1) && !EnemyStateMotor.CanEnemyPassThroughWalkableTile(Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.x - 1), Mathf.FloorToInt(EnemyStateMotor.GetPointToMove().position.z + 1)))
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetZCoordinate() - (EnemyStateMotor.GetPointToMove().position.z + 1))  > EnemyStateMotor.GetTileZMovementRangeByOrigin()
                    || Mathf.Abs(EnemyStateMotor.GetOriginTile().GetXCoordinate() - (EnemyStateMotor.GetPointToMove().position.x - 1))  > EnemyStateMotor.GetTileXMovementRangeByOrigin()
                    || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(Transform.position.x, Transform.position.y, Transform.position.z, _moveDirection)
                    )
                {
                    SetMoveDirectionToGo();
                }
                else
                {
                    Transform currentPointToMove = EnemyStateMotor.GetPointToMove();
                    currentPointToMove.position += new Vector3(-1f, 0f, 1f);
                    EnemyStateMotor.GetPointToMove().position = new Vector3(currentPointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(Transform.position.x, Transform.position.y, Transform.position.z, currentPointToMove.position.x, currentPointToMove.position.z), currentPointToMove.position.z);
                    
                    _moveDirectionIsSet = true;
                }
                break;
        }
    }

    private IEnumerator GoIdleAfterOnlyRotating()
    {
        yield return new WaitForSeconds(Random.Range(minTimeToGoIdleWanderAfterOnlyRotating, maxTimeToGoIdleWanderAfterOnlyRotating));
        _goIdle = true;
    }
    
    private IEnumerator GoIdleAfterMoving()
    {
        yield return new WaitForSeconds(Random.Range(minTimeToGoIdleWanderAfterMoving, maxTimeToGoIdleWanderAfterMoving));
        _goIdle = true;
    }
    
    public override void Construct()
    {
        if (Random.Range(0, maxRandomValueForRotatingAction + 1) == 0 || !CanMoveFromPlace())
        {
            // TODO: Change sprite animation to idle
            SetMoveDirectionOnOnlyRotation();
            if (EnemyStateMotor.GetCharacterVisualsController())
            {
                EnemyStateMotor.GetCharacterVisualsController().SetMoveDirection(_moveDirection);
            }
            _goIdleAfterOnlyRotatingCoroutine = StartCoroutine(GoIdleAfterOnlyRotating());
            return;
        }

        if (!CanCheckMovement || !RoomManager.Instance)
        {
            _goIdle = true;
            return;
        }

        // TODO: Change sprite animation to move
        SetMoveDirectionToGo();
        if (!_moveDirectionIsSet)
        {
            // TODO: Change sprite animation to idle
            SetMoveDirectionOnOnlyRotation();
            if (EnemyStateMotor.GetCharacterVisualsController())
            {
                EnemyStateMotor.GetCharacterVisualsController().SetMoveDirection(_moveDirection);
            }
            _goIdleAfterOnlyRotatingCoroutine = StartCoroutine(GoIdleAfterOnlyRotating());
            return;
        }
        if (EnemyStateMotor.GetCharacterVisualsController())
        {
            EnemyStateMotor.GetCharacterVisualsController().SetMoveDirection(_moveDirection);
        }
        MoveCharacterToPoint();
    }

    public override void Destruct()
    {
        _timesTriedToGetNewDirectionWhenOnlyRotating = 0;
        _timesTriedToMoveBeforeReturningToIdle = 0;
        _moveDirectionIsSet = false;
        _goIdle = false;
        _mainCharacterSpotted = false;
        CanCheckMovement = true;
        
        if (_goIdleAfterOnlyRotatingCoroutine != null)
        {
            StopCoroutine(_goIdleAfterOnlyRotatingCoroutine);
            _goIdleAfterOnlyRotatingCoroutine = null;
        }

        if (_goIdleAfterMovingCoroutine == null)
        {
            return;
        }
        StopCoroutine(_goIdleAfterMovingCoroutine);
        _goIdleAfterMovingCoroutine = null;
    }

    public override void Transition()
    {
        if (_goIdle)
        {
            EnemyStateMotor.ChangeState(GetComponent<EnemyIdleState>());
            return;
        }

        if (!_mainCharacterSpotted)
        {
            return;
        }
        EnemyStateMotor.ChangeState(GetComponent<EnemyChaseState>());
    }
    
    public override void UpdateState()
    {
        if (!EnemyStateMotor.CanChase() || EnemyStateMotor.IsPassive() || !CanSeePlayer())
        {
            return;
        }
        
        if (_goIdleAfterOnlyRotatingCoroutine != null)
        {
            StopCoroutine(_goIdleAfterOnlyRotatingCoroutine);
            _goIdleAfterOnlyRotatingCoroutine = null;
        }

        if (_goIdleAfterMovingCoroutine != null)
        {
            StopCoroutine(_goIdleAfterMovingCoroutine);
            _goIdleAfterMovingCoroutine = null;
        }

        _mainCharacterSpotted = true;
    }
}
