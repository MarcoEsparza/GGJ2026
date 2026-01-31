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
        Debug.Log("Enter Climb State");
    }

    public void OnExecuteState()
    {
        CheckStateConditions();
        m_playerController.Climbing();
        //m_playerController.Jumping();
    }

    public void OnExitState()
    {
        m_playerController.ResetGravityScale();
    }

    public void CheckStateConditions()
    {
        if(m_playerController.IsGrounded &&
           !m_playerController.CheckClimbActivation() &&
           !m_playerController.MovementInput.Equals(Vector2.zero))
        {
            m_stateMachine.ChangeState("Move");
            m_playerController.PlayerRB.linearVelocityY = 5.0f;
        }
        else if(!m_playerController.CheckClimbActivation() &&
                m_playerController.MovementInput.Equals(Vector2.zero))
        {
            m_stateMachine.ChangeState("Idle");
            m_playerController.PlayerRB.AddForceY(3.0f);
        }
        else if(m_playerController.CheckClimbActivation() && m_playerController.IsJumping)
        {
            m_stateMachine.ChangeState("Jump");
        }
    }
}
