using UnityEngine;

public class EnemyAttackState : IState
{
    private StateMachine m_stateMachine;
    private EnemyController m_controller;
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
        CheckStateConditions();
    }

    //Method executed at the end of a state.
    public void OnExitState()
    {

    }

    //Method executed to check if the current state should change state.
    public void CheckStateConditions()
    {
        // if there is no longer a target or the target is out of sight
        if (!m_controller.GetTarget() || !m_controller.InLineOfSight())
        {
            m_stateMachine.ChangeState("Move");
        }
    }
}
