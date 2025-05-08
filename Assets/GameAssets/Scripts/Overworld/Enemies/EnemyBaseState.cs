using UnityEngine;

[RequireComponent(typeof(EnemyStateMotor))]

public class EnemyBaseState : MonoBehaviour
{
    protected EnemyStateMotor EnemyStateMotor;
    protected Transform Transform;
    protected bool CanCheckMovement;

    private void Awake()
    {
        EnemyStateMotor = GetComponent<EnemyStateMotor>();
        Transform = transform;
        CanCheckMovement = true;
    }

    public virtual void Construct()
    {
    }

    public virtual void Destruct()
    {
    }

    public virtual void Transition()
    {
    }

    public virtual void UpdateState()
    {
    }

    public bool CanSeePlayer()
    {
        // TODO
        return false;
    }

    public bool CanMoveFromPlace()
    {
        if (!RoomManager.Instance)
        {
            return false;
        }
        
        for (int xPosition = -1; xPosition <= 1; xPosition++)
        {
            for (int zPosition = -1; zPosition <= 1; zPosition++)
            {
                if ((RoomManager.Instance.IsTileWalkable(xPosition, zPosition) && EnemyStateMotor.CanEnemyPassThroughWalkableTile(xPosition, zPosition)) || EnemyStateMotor.CanEnemyPassThroughNonWalkableTile(xPosition, zPosition))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

}
