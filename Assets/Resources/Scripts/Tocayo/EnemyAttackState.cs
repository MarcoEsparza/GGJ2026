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
        m_controller.TickAttackTime();
        // if the attack timer is greater than the delay set for the attack to hit
        if (m_controller.GetAttackTime() > m_controller.GetAttackDelay()) {
            // if the player is still in range
            if (m_controller.GetTarget()) {
                GameManager.Instance.ResetLevel();
            }
        }
        CheckStateConditions();
    }

    //Method executed at the end of a state.
    public void OnExitState()
    {
        m_controller.ResetAttackTime();
    }

    //Method executed to check if the current state should change state.
    public void CheckStateConditions()
    {
        // if there is no longer a target or the target is out of sight
        if ((!m_controller.GetTarget() || !m_controller.InLineOfSight()) ||
            (m_controller.GetAttackTime() > m_controller.GetAttackDuration())) { // if attack time is greater than the attack duration.
            m_stateMachine.ChangeState("Move");
        }
    }
}
