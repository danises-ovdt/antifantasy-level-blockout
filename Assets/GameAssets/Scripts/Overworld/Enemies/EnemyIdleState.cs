using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    [SerializeField, Range(0f, 10f)] private float minTimeToGoWander;
    [SerializeField, Range(0f, 10f)] private float maxTimeToGoWander;
    private Coroutine _goWanderCoroutine;
    private bool _goWander;
    private bool _mainCharacterSpotted;

    private IEnumerator GoWander()
    {
        yield return new WaitForSeconds(Random.Range(minTimeToGoWander, maxTimeToGoWander));
        _goWander = true;
    }
    
    public override void Construct()
    {
        // TODO: Change sprite animation to idle
        if (!EnemyStateMotor.CanWander())
        {
            return;
        }
        _goWanderCoroutine = StartCoroutine(GoWander());
    }

    public override void Destruct()
    {
        _goWander = false;
        _mainCharacterSpotted = false;

        if (_goWanderCoroutine == null)
        {
            return;
        }
        StopCoroutine(_goWanderCoroutine);
        _goWanderCoroutine = null;
    }

    public override void Transition()
    {
        if (_goWander)
        {
            EnemyStateMotor.ChangeState(GetComponent<EnemyWanderState>());
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
        
        if (_goWanderCoroutine != null)
        {
            StopCoroutine(_goWanderCoroutine);
            _goWanderCoroutine = null;
        }

        _mainCharacterSpotted = true;
    }
}
