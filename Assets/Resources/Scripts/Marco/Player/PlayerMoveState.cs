using UnityEngine;

public class PlayerMoveState : IState
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

        Debug.Log("Enter Move State");
        m_playerController.AnimatorsSetBool("Move", true);
    }

    public void OnExecuteState()
    {
        CheckStateConditions();
        m_playerController.Movement();
    }

    public void OnExitState()
    {
        m_playerController.AnimatorsSetBool("Move", false);
    }

    public void CheckStateConditions()
    {
        if(m_playerController.MovementInput == Vector2.zero && m_playerController.IsGrounded)
        {
            m_stateMachine.ChangeState("Idle");
        }
        else if(m_playerController.IsGrounded && m_playerController.IsJumping)
        {
            m_stateMachine.ChangeState("Jump");
        }
        else if (m_playerController.CheckClimbActivation())
        {
            m_stateMachine.ChangeState("Climb");
        }
    }
}
