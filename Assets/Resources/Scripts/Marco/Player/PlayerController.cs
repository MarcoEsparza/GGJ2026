using UnityEngine;

enum PlayerMask
{
    None = 0,
    Monkey = 1,
    Jaguar = 2,
    Axolotl = 3
}

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_jumpForce = 10f;
    [SerializeField] private float m_airControll = 0.8f;
    [SerializeField] private float m_gravityScale = 1.0f;
    [SerializeField] private float m_jaguarSpeed = 8.0f;
    [SerializeField] private float m_monkeyJumpBoost = 10.0f;
    [SerializeField] private float m_climbSpeed = 3.0f;
    [SerializeField] private float m_abilityCooldown = 1.0f;
    [SerializeField] private float m_monkeyBoostDuration = 2.0f;

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
    private SpriteRenderer m_spriteRenderer;
    private PlayerMask m_currentMask = PlayerMask.None;

    private Vector2 m_movementInput;
    private Vector2 m_climbInput;
    private bool m_isGrounded;
    private bool m_isJumping = false;
    private bool m_touchingWallLeft = false;
    private bool m_touchingWallRight = false;
    private bool m_isWallJumping = false;
    private bool m_isLookingRight = true;
    private bool m_canClimb = false;
    private bool m_useAbility = false;
    private bool m_canUseAbility = true;
    private bool m_usingMonkeyAbility = false;

    private float m_wallJumpTimer = 0.0f;
    private float m_abilityTimer = 0.0f;
    private float m_monkeyAbilityTimer = 0.0f;

    public bool IsGrounded { get { return m_isGrounded; } }
    public Vector2 MovementInput { get { return m_movementInput; } }
    public bool IsJumping { get { return m_isJumping; } }
    public Rigidbody2D PlayerRB { get { return m_rb; } }
    public bool IsWallJumping { get { return m_isWallJumping; } }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("NormalWall"))
        {
            m_canClimb = false;
        }

        if (collision.gameObject.CompareTag("ClimbableWall"))
        {
            if (m_currentMask == PlayerMask.Monkey)
            {
                m_canClimb = true;
            }
            else
            {
                m_canClimb = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ClimbableWall"))
        {
            if (m_currentMask == PlayerMask.Monkey)
            {
                m_canClimb = true;
            }
            else
            {
                m_canClimb = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NormalWall"))
        {
            m_canClimb = false;
        }
        if (collision.gameObject.CompareTag("ClimbableWall"))
        {
            m_canClimb = false;
        }
    }

    private void Awake()
    {
        //m_instance = this;
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.gravityScale = m_gravityScale;
        m_inputActions = new PlayerInputActions();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        SetUpStateMachine();
        SetUpInputActions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.CurrentState.OnExecuteState();

        if (m_isWallJumping)
        {
            m_wallJumpTimer += Time.deltaTime;
            if (m_wallJumpTimer >= m_wallJumpDuration)
            {
                m_isWallJumping = false;
                m_wallJumpTimer = 0.0f;
            }
        }

        m_abilityTimer += Time.deltaTime;
        if (m_abilityTimer >= m_abilityCooldown)
        {
            m_canUseAbility = true;
            m_abilityTimer = 0.0f;
        }

        if(m_usingMonkeyAbility)
        {
            m_monkeyAbilityTimer += Time.deltaTime;
            if(m_monkeyAbilityTimer >= m_monkeyBoostDuration)
            {
                m_usingMonkeyAbility = false;
                m_monkeyAbilityTimer = 0.0f;
            }
        }

        if (m_movementInput.x < 0.0f && m_isLookingRight)
        {
            // Flip to the left
            m_spriteRenderer.flipX = true;
            m_isLookingRight = false;
        }
        else if (m_movementInput.x > 0.0f && !m_isLookingRight)
        {
            m_spriteRenderer.flipX = false;
            m_isLookingRight = true;
        }

        if(m_currentMask == PlayerMask.None)
        {
            // Set default abilities
        }
        else if(m_currentMask == PlayerMask.Monkey)
        {
            m_spriteRenderer.color = Color.brown;
        }
        else if(m_currentMask == PlayerMask.Jaguar)
        {
            m_spriteRenderer.color = Color.yellow;
        }
        else if(m_currentMask == PlayerMask.Axolotl)
        {
            m_spriteRenderer.color = Color.pink;
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
        m_stateMachine.AddState(new PlayerClimbingState(), "Climb");
        // Setting initial state
        m_stateMachine.ChangeState("Idle");
    }

    void SetUpInputActions()
    {
        m_inputActions.Player.Enable();
        m_inputActions.Player.Move.performed += ctx => m_movementInput = ctx.ReadValue<Vector2>();
        m_inputActions.Player.Move.canceled += ctx => m_movementInput = Vector2.zero;
        m_inputActions.Player.Climb.performed += ctx => m_climbInput = ctx.ReadValue<Vector2>();
        m_inputActions.Player.Climb.canceled += ctx => m_climbInput = Vector2.zero;
        m_inputActions.Player.Jump.performed += ctx => m_isJumping = true;
        m_inputActions.Player.Jump.canceled += ctx => m_isJumping = false;
        m_inputActions.Player.MonkeyMask.performed += ctx => m_currentMask = PlayerMask.Monkey;
        m_inputActions.Player.JaguarMask.performed += ctx => m_currentMask = PlayerMask.Jaguar;
        m_inputActions.Player.AxolotlMask.performed += ctx => m_currentMask = PlayerMask.Axolotl;
        m_inputActions.Player.MaskAbilitie.performed += ctx => m_useAbility = true;
        m_inputActions.Player.MaskAbilitie.canceled += ctx => m_useAbility = false;
    }

    public void Movement()
    {
        if(m_isWallJumping)
        {
            return;
        }

        float additionalVel = 0.0f;
        if(m_currentMask == PlayerMask.Jaguar)
        {
            additionalVel = m_jaguarSpeed;
        }

        float controllFactor = m_isGrounded ? 1.0f : m_airControll;
        float horizontalVelocity = (m_movementInput.x * (additionalVel + m_moveSpeed)) * controllFactor;
        m_rb.linearVelocity = new Vector2(horizontalVelocity, m_rb.linearVelocity.y);
    }

    public void Jumping()
    {
        float additionalJump = 0.0f;
        if(m_currentMask == PlayerMask.Monkey && m_canUseAbility && m_useAbility)
        {
            additionalJump = m_monkeyJumpBoost;
        }
        float totalJumpForce = m_jumpForce + additionalJump;

        if (m_isGrounded && m_isJumping)
        {
            m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, totalJumpForce);
        }
        else if(CheckClimbActivation() && m_isJumping)
        {
            m_rb.gravityScale = m_gravityScale;
            m_isWallJumping = true;
            m_wallJumpTimer = 0.0f;
            int direction = m_touchingWallLeft ? 1 : -1;
            m_rb.linearVelocity = new Vector2(direction * m_wallJumpForce.x, m_wallJumpForce.y);
        }
    }

    public void Climbing()
    {
        m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, m_climbInput.y * m_climbSpeed);
    }

    public bool CheckClimbActivation()
    {
        if(m_currentMask != PlayerMask.Monkey)
        {
            return false;
        }

        if ((m_touchingWallLeft || m_touchingWallRight) && !m_canClimb)
        {
            m_movementInput.y = Mathf.Min(0.0f, m_movementInput.y);
            return false;
        }
        else if ((m_touchingWallLeft || m_touchingWallRight) && m_canClimb)
        {
            //m_rb.gravityScale = 0.0f;
            //m_rb.linearVelocityY = 0.0f;
            return true;
        }

        return false;
    }

    public void ResetGravityScale()
    {
        m_rb.gravityScale = m_gravityScale;
    }
}
