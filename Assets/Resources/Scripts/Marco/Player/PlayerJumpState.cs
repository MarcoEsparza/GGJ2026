using UnityEngine;

public class PlayerJumpState : IState
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

        Debug.Log("Enter Jump State");
    }

    public void OnExecuteState()
    {
        CheckStateConditions();
        m_playerController.Jumping();
        m_playerController.Movement();
    }

    public void OnExitState()
    {

    }

    public void CheckStateConditions()
    {
        if(m_playerController.IsGrounded &&
           !m_playerController.MovementInput.Equals(Vector2.zero) &&
           m_playerController.PlayerRB.linearVelocityY == 0.0f)
        {
            m_stateMachine.ChangeState("Move");
        }
        else if(m_playerController.IsGrounded &&
                m_playerController.MovementInput.Equals(Vector2.zero) &&
                m_playerController.PlayerRB.linearVelocity.Equals(Vector2.zero))
        {
            m_stateMachine.ChangeState("Idle");
        }
        else if(m_playerController.CheckClimbActivation() && !m_playerController.IsJumping)
        {
            m_stateMachine.ChangeState("Climb");
        }
    }
}
