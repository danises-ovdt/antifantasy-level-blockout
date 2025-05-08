using UnityEngine;

public class CharacterVisualsController : MonoBehaviour
{
    // Test only, delete later
    [SerializeField] private Transform faceTransform;
    //
    public void SetMoveDirection(MoveDirection moveDirection)
    {
        // Test only, delete later
        if (!faceTransform)
        {
            return;
        }
        
        float faceRotation = 0f;
        switch (moveDirection)
        {
            case MoveDirection.South:
                faceRotation = 0f;
                break;
            case MoveDirection.SouthWest:
                faceRotation = 45f;
                break;
            case MoveDirection.West:
                faceRotation = 90f;
                break;
            case MoveDirection.NorthWest:
                faceRotation = 135;
                break;
            case MoveDirection.North:
                faceRotation = 180f;
                break;
            case MoveDirection.NorthEast:
                faceRotation = 225f;
                break;
            case MoveDirection.East:
                faceRotation = 270f;
                break;
            case MoveDirection.SouthEast:
                faceRotation = 315f;
                break;
            case MoveDirection.None:
                return;
        }
        
        faceTransform.localRotation = Quaternion.Euler(0f, faceRotation, 0f);
        //
    }
}
