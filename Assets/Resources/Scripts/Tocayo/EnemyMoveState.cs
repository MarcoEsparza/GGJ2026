using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyMoveState : IState
{
    private StateMachine m_stateMachine;
    private EnemyController m_controller;

    // Movement parameters
    public StateMachine StateController
    {
        get { return m_stateMachine; }
        set { m_stateMachine = value; }
    }

    private GameObject m_owner;
    public GameObject Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }

    //Method executed when a state has been entered.
    public void OnStateEnter()
    {
        if (!m_controller) {
            m_controller = m_owner.GetComponent<EnemyController>();
        }
    }

    //Method executed every frame for the current state.
    public void OnExecuteState()
    {
        if (m_controller.m_isGrounded &&
            (!m_controller.CheckDown() || m_controller.CheckForward()))
        {
            float dir = m_controller.GetDirection() * -1f;
            m_controller.SetDirection(dir);
            m_owner.transform.rotation = Quaternion.Euler(0f,
                                                          m_controller.GetDirection() > 0f ? 0f :
                                                          180f, 0f);
        }
        m_controller.MoveForward();

        CheckStateConditions();
    }

    //Method executed at the end of a state.
    public void OnExitState()
    {

    }

    //Method executed to check if the current state should change state.
    public void CheckStateConditions()
    {
        // if there is a target and the enemy is in line of sight of the target.
        if (m_controller.GetTarget() && m_controller.InLineOfSight()) {
            m_stateMachine.ChangeState("Attack");
        }
    }
}
