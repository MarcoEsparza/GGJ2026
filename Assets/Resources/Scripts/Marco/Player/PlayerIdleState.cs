using UnityEngine;

public class PlayerIdleState : IState
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

    public void OnStateEnter()
    {

    }

    public void OnExecuteState()
    {

    }

    public void OnExitState()
    {

    }

    public void CheckStateConditions()
    {

    }
}
