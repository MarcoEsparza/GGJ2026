using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_jumpForce = 10f;
    [SerializeField] private float m_airControll = 0.8f;

    [Header("GroundCheck")]
    [SerializeField] private Transform m_groundCheck;
    [SerializeField] private float m_groundRadius = 0.2f;
    [SerializeField] private LayerMask m_groundLayer;

    [Header("WallCheck")]
    [SerializeField] private Transform m_wallCheckLeft;
    [SerializeField] private Transform m_wallCheckRight;
    [SerializeField] private float m_wallCheckRadius = 0.1f;
    [SerializeField] private LayerMask m_wallLayer;
    [SerializeField] private Vector2 m_wallJumpForce = new Vector2(10.0f, 15.0f);
    [SerializeField] private float m_wallJumpDuration = 0.2f;

    //private PlayerController m_instance;
    private Rigidbody2D m_rb;
    private PlayerInputActions m_inputActions;
    private StateMachine m_stateMachine;

    private Vector2 m_movementInput;
    private bool m_isGrounded;
    private bool m_isJumping = false;
    private bool m_touchingWallLeft = false;
    private bool m_touchingWallRight = false;
    private float m_wallJumpTimer = 0.0f;
    private bool m_isLookingRight = true;

    public bool IsGrounded { get { return m_isGrounded; } }
    public Vector2 MovementInput { get { return m_movementInput; } }
    public bool IsJumping { get { return m_isJumping; } }
    public Rigidbody2D PlayerRB { get { return m_rb; } }

    private void Awake()
    {
        //m_instance = this;
        m_rb = GetComponent<Rigidbody2D>();
        m_inputActions = new PlayerInputActions();
        SetUpStateMachine();

        m_inputActions.Player.Enable();
        m_inputActions.Player.Move.performed += ctx => m_movementInput = ctx.ReadValue<Vector2>();
        m_inputActions.Player.Move.canceled += ctx => m_movementInput = Vector2.zero;
        m_inputActions.Player.Jump.performed += ctx => m_isJumping = true;
        m_inputActions.Player.Jump.canceled += ctx => m_isJumping = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.CurrentState.OnExecuteState();

        if (m_movementInput.x < 0.0f && m_isLookingRight)
        {
            // Flip to the left
            m_isLookingRight = false;
        }
        else if (m_movementInput.x > 0.0f && !m_isLookingRight)
        {
            // Flip to the right
            m_isLookingRight = true;
        }
    }

    private void FixedUpdate()
    {
        m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position,
                                               m_groundRadius,
                                               m_groundLayer);
        m_touchingWallLeft = Physics2D.OverlapCircle(m_wallCheckLeft.position,
                                                     m_wallCheckRadius,
                                                     m_wallLayer);
        m_touchingWallRight = Physics2D.OverlapCircle(m_wallCheckRight.position,
                                                      m_wallCheckRadius,
                                                      m_wallLayer);
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

    public void Movement()
    {
        m_rb.linearVelocity = new Vector2(m_movementInput.x * m_moveSpeed,
                                          m_rb.linearVelocity.y);
    }

    public void Jumping()
    {
        if (m_isGrounded && m_isJumping)
        {
            m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x,
                                              m_jumpForce);
        }
    }
}
