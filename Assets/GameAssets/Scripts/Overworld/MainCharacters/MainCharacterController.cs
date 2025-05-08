using System.Collections;
using UnityEngine;
using DG.Tweening;
public class MainCharacterController : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float characterMoveTime;
    [SerializeField] private Transform pointToMove;
    [SerializeField] private CharacterFollower characterFirstFollower;
    [SerializeField] private MoveDirection initialMoveDirection = MoveDirection.South;
    [SerializeField] private CharacterVisualsController characterVisualsController;
    [SerializeField, Range(0f, 10f)] private float rotateInPlaceCooldown;
    private Transform _transform;
    private Coroutine _enableDifferentInputAfterRotationCoroutine;
    private MoveDirection _currentMoveDirection;
    private int _xPosition;
    private int _zPosition;
    private bool _canInputDifferentMove = true;

    private void Start()
    {
        if (pointToMove)
        {
            pointToMove.parent = null;
        }

        if (characterVisualsController)
        {
            characterVisualsController.SetMoveDirection(initialMoveDirection);
        }

        _transform = transform;
        _canInputDifferentMove = true;
        _currentMoveDirection = initialMoveDirection;
        
        _xPosition = Mathf.FloorToInt(_transform.position.x);
        _zPosition = Mathf.FloorToInt(_transform.position.z);
    }

    private void Update()
    {
        if (_canInputDifferentMove && RoomManager.Instance)
        {
            float horizontalAxisRaw = Input.GetAxisRaw("Horizontal");
            horizontalAxisRaw = horizontalAxisRaw > 0 ? 1f : horizontalAxisRaw < 0 ? -1f : 0f;
            float verticalAxisRaw = Input.GetAxisRaw("Vertical");
            verticalAxisRaw = verticalAxisRaw > 0 ? 1f : verticalAxisRaw < 0 ? -1f : 0f;
            MoveDirection moveDirection = GetMoveDirection(horizontalAxisRaw, verticalAxisRaw);
            if (characterVisualsController)
            {
                characterVisualsController.SetMoveDirection(moveDirection);
            }
            if (moveDirection != MoveDirection.None)
            {
                _currentMoveDirection = moveDirection;
            }

            if (Input.GetButton("Rotate"))
            {
                _canInputDifferentMove = false;
                _enableDifferentInputAfterRotationCoroutine = StartCoroutine(EnableDifferentInputAfterRotation());
                return;
            }
            
            if (Mathf.Abs(horizontalAxisRaw) >= 1f && Mathf.Abs(verticalAxisRaw) >= 1f)
            {
                if (!RoomManager.Instance.IsTileWalkable(pointToMove.position.x + Mathf.FloorToInt(horizontalAxisRaw), pointToMove.position.z + Mathf.FloorToInt(verticalAxisRaw)) || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(_transform.position.x, _transform.position.y, _transform.position.z, moveDirection))
                {
                    return;
                }

                pointToMove.position += new Vector3(Mathf.FloorToInt(horizontalAxisRaw), 0f, Mathf.FloorToInt(verticalAxisRaw));
                pointToMove.position = new Vector3(pointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(_transform.position.x, _transform.position.y, _transform.position.z, pointToMove.position.x, pointToMove.position.z), pointToMove.position.z);
                
                MoveCharacterToPoint();
                return;
            }
            
            if (Mathf.Abs(horizontalAxisRaw) >= 1f)
            {
                if (!RoomManager.Instance.IsTileWalkable(pointToMove.position.x + Mathf.FloorToInt(horizontalAxisRaw), pointToMove.position.z) || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(_transform.position.x, _transform.position.y, _transform.position.z, moveDirection))
                {
                    return;
                }
                
                pointToMove.position += new Vector3(Mathf.FloorToInt(horizontalAxisRaw), 0f, 0f);
                pointToMove.position = new Vector3(pointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(_transform.position.x, _transform.position.y, _transform.position.z, pointToMove.position.x, pointToMove.position.z), pointToMove.position.z);
                
                MoveCharacterToPoint();
                return;
            }
        
            if (Mathf.Abs(verticalAxisRaw) >= 1f)
            {
                if (!RoomManager.Instance.IsTileWalkable(pointToMove.position.x, pointToMove.position.z + Mathf.FloorToInt(verticalAxisRaw)) || !RoomManager.Instance.CanGoToDirectionFromCurrentTile(_transform.position.x, _transform.position.y, _transform.position.z, moveDirection))
                {
                    return;
                }
                
                pointToMove.position += new Vector3(0f, 0f, Mathf.FloorToInt(verticalAxisRaw));
                pointToMove.position = new Vector3(pointToMove.position.x, RoomManager.Instance.GetTileToMoveHeight(_transform.position.x, _transform.position.y, _transform.position.z, pointToMove.position.x, pointToMove.position.z), pointToMove.position.z);

                MoveCharacterToPoint();
            }
        }
    }

    private void OnDestroy()
    {
        if (_enableDifferentInputAfterRotationCoroutine == null)
        {
            return;
        }
        
        StopCoroutine(_enableDifferentInputAfterRotationCoroutine);
        _enableDifferentInputAfterRotationCoroutine = null;

    }

    private void MoveCharacterToPoint()
    {
        _transform.DOKill();
        
        _canInputDifferentMove = false;

        if (characterFirstFollower)
        {
            characterFirstFollower.MoveCharacterToPoint(_transform);
        }
        
        Tweener movementTweener = _transform.DOMove(pointToMove.position, characterMoveTime).SetEase(Ease.Linear);
        movementTweener.OnComplete(() =>
        {
            _canInputDifferentMove = true;
            _transform.DOKill();
            
            _xPosition = Mathf.FloorToInt(_transform.position.x);
            _zPosition = Mathf.FloorToInt(_transform.position.z);
        });
    }

    private MoveDirection GetMoveDirection(float horizontalAxisRaw, float verticalAxisRaw)
    {
        if (Mathf.Abs(horizontalAxisRaw) >= 1f)
        {
            if (horizontalAxisRaw > 0f)
            {
                if (Mathf.Abs(verticalAxisRaw) >= 1f)
                {
                    if (verticalAxisRaw > 0f)
                    {
                        return MoveDirection.NorthEast;
                    }
                    return MoveDirection.SouthEast;
                }
                return MoveDirection.East;
            }
            if (Mathf.Abs(verticalAxisRaw) >= 1f)
            {
                if (verticalAxisRaw > 0f)
                {
                    return MoveDirection.NorthWest;
                }
                return MoveDirection.SouthWest;
            }
            return MoveDirection.West;
        }
        if (Mathf.Abs(verticalAxisRaw) >= 1f)
        {
            if (verticalAxisRaw > 0f)
            {
                return MoveDirection.North;
            }

            return MoveDirection.South;
        }
        return MoveDirection.None;
    }

    private IEnumerator EnableDifferentInputAfterRotation()
    {
        yield return new WaitForSeconds(rotateInPlaceCooldown);
        _canInputDifferentMove = true;
    }
    
    public void SetMoveDirection(MoveDirection moveDirection)
    {
        if (!characterVisualsController)
        {
            return;
        }
        characterVisualsController.SetMoveDirection(moveDirection);
    }

    public float GetCharacterMoveTime()
    {
        return characterMoveTime;
    }

    public int GetXPosition()
    {
        return _xPosition;
    }

    public int GetZPosition()
    {
        return _zPosition;
    }
}
