using UnityEngine;
using DG.Tweening;

public class CharacterFollower : MonoBehaviour
{
    [SerializeField] private MainCharacterController mainCharacterController;
    [SerializeField] private CharacterFollower follower;
    [SerializeField] private MoveDirection initialMoveDirection = MoveDirection.South;
    [SerializeField] private CharacterVisualsController characterVisualsController;
    private Transform _transform;
    private MoveDirection _currentMoveDirection;

    private void Start()
    {
        if (characterVisualsController)
        {
            characterVisualsController.SetMoveDirection(initialMoveDirection);
        }
        
        _transform = transform;
        _currentMoveDirection = initialMoveDirection;
    }
    
    public void MoveCharacterToPoint(Transform pointToMove)
    {
        _transform.DOKill();
        
        if (!mainCharacterController)
        {
            return;
        }

        if (follower)
        {
            follower.MoveCharacterToPoint(_transform);
        }
        
        MoveDirection moveDirection = GetMoveDirection(pointToMove.position.x - _transform.position.x, pointToMove.position.z - _transform.position.z);
        if (characterVisualsController)
        {
            characterVisualsController.SetMoveDirection(moveDirection);
        }
        if (moveDirection != MoveDirection.None)
        {
            _currentMoveDirection = moveDirection;
        }
        
        Tweener movementTweener = _transform.DOMove(pointToMove.position, mainCharacterController.GetCharacterMoveTime()).SetEase(Ease.Linear);

        movementTweener.OnComplete(() =>
        {
            _transform.DOKill();
        });
    }
    
    private MoveDirection GetMoveDirection(float horizontalValue, float verticalValue)
    {
        if (Mathf.Abs(horizontalValue) >= 1f)
        {
            if (horizontalValue > 0f)
            {
                if (Mathf.Abs(verticalValue) >= 1f)
                {
                    if (verticalValue > 0f)
                    {
                        return MoveDirection.NorthEast;
                    }
                    return MoveDirection.SouthEast;
                }
                return MoveDirection.East;
            }
            if (Mathf.Abs(verticalValue) >= 1f)
            {
                if (verticalValue > 0f)
                {
                    return MoveDirection.NorthWest;
                }
                return MoveDirection.SouthWest;
            }
            return MoveDirection.West;
        }
        if (Mathf.Abs(verticalValue) >= 1f)
        {
            if (verticalValue > 0f)
            {
                return MoveDirection.North;
            }

            return MoveDirection.South;
        }
        return MoveDirection.None;
    }
    
    public void SetMoveDirection(MoveDirection moveDirection)
    {
        if (!characterVisualsController)
        {
            return;
        }
        characterVisualsController.SetMoveDirection(moveDirection);
    }
}
