using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int m_health = 100;

    [Header("Movement")]
    [SerializeField] private float m_moveSpeed = 5f;

    [Header("Jumping")]
    [SerializeField] private float m_jumpForce = 10f;

    private Rigidbody2D m_rb;
    private PlayerInputActions m_inputActions;
    private StateMachine m_stateMachine;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_inputActions = new PlayerInputActions();
        SetUpStateMachine();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.CurrentState.OnExecuteState();
    }

    void SetUpStateMachine()
    {
        m_stateMachine = new StateMachine();
        m_stateMachine.Owner = this.gameObject;
        // Adding states to the state machine
        m_stateMachine.AddState(new PlayerIdleState(), "Idle");
        m_stateMachine.AddState(new PlayerMoveState(), "Move");
        m_stateMachine.AddState(new PlayerJumpState(), "Jump");
        // Setting initial state
        m_stateMachine.ChangeState("Idle");
    }
}
