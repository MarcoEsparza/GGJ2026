using UnityEngine;

public class PlayerClimbingState : IState
{
    private StateMachine m_stateMachine;
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

    PlayerController m_playerController;

    public void OnStateEnter()
    {
        if (m_playerController == null)
        {
            m_playerController = m_owner.GetComponent<PlayerController>();
        }
    }

    public void OnExecuteState()
    {
        CheckStateConditions();
    }

    public void OnExitState()
    {

    }

    public void CheckStateConditions()
    {
        
    }
}
