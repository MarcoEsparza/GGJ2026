using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerMask
{
    None = 0,
    Monkey = 1,
    Jaguar = 2,
    Axolotl = 3
}

public class PlayerController : MonoBehaviour
{
    public static event Action<bool> axolotlMaskOn;
    public static event Action<PlatformType> selectPlatform;

    [Header("Stats")]
    [Tooltip("Movement speed of the player")]
    [SerializeField] private float m_moveSpeed = 5f;
    [Tooltip("Jump force applied to the player")]
    [SerializeField] private float m_jumpForce = 10f;
    [Tooltip("The amount of control the player has while in the air")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_airControll = 0.8f;
    [Tooltip("The amount of force you will be pulled to the ground")]
    [SerializeField] private float m_gravityScale = 1.0f;
    [Tooltip("The speed boost the jaguar mask will ADD to the basic move speed")]
    [SerializeField] private float m_jaguarSpeed = 8.0f;
    [Tooltip("The jump force the monkey mask will ADD to the basic jump force")]
    [SerializeField] private float m_monkeyJumpBoost = 10.0f;
    [Tooltip("Climb speed with the monkey mask")]
    [SerializeField] private float m_climbSpeed = 3.0f;
    [Tooltip("Cooldown for using abilities (it stays even if player change mask)")]
    [SerializeField] private float m_abilityCooldown = 1.0f;
    [Tooltip("Amount of time the player has for using the boosted jump")]
    [SerializeField] private float m_monkeyBoostDuration = 2.0f;
    [Tooltip("The distance from the player ")]
    [SerializeField] private float m_attackDistance = 1.0f;

    [Header("GroundCheck")]
    [Tooltip("The left point for ground detection")]
    [SerializeField] private Transform m_lGroundCheck;
    [Tooltip("The right point for ground detection")]
    [SerializeField] private Transform m_rGroundCheck;
    [Tooltip("Radius for ground detection")]
    [SerializeField] private float m_groundRadius = 0.2f;
    [Tooltip("Layer used for ground detection")]
    [SerializeField] private LayerMask m_groundLayer;

    [Header("WallCheck")]
    [Tooltip("The left point for wall detection")]
    [SerializeField] private Transform m_wallCheckLeft;
    [Tooltip("The right point for wall detection")]
    [SerializeField] private Transform m_wallCheckRight;
    [Tooltip("Radius for wall detection")]
    [SerializeField] private float m_wallCheckRadius = 0.1f;
    [Tooltip("Layer used for wall detection")]
    [SerializeField] private LayerMask m_wallLayer;
    [Tooltip("Jump force from a wall (apply force on both axes)")]
    [SerializeField] private Vector2 m_wallJumpForce = new Vector2(10.0f, 15.0f);
    [Tooltip("Amount of time the wall jump force is going to be preserved")]
    [SerializeField] private float m_wallJumpDuration = 0.2f;

    [Header("Attack")]
    [Tooltip("Instantieted prefab for attacking")]
    [SerializeField] private GameObject m_attackBox;

    [Header("Animators")]
    [SerializeField] private List<GameObject> m_maskGOList;

    //private List<Animator> m_animatorsList;

    // Rigidbody component
    private Rigidbody2D m_rb;
    // Player input actions
    private PlayerInputActions m_inputActions;
    // Player state machine
    private StateMachine m_stateMachine;
    // Sprite renderer component
    private SpriteRenderer m_currentSpriteRenderer;

    private AudioSource m_audioSrc;
    // Mask in use
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
    private bool m_usingAbility = false;
    private bool m_canUseAbility = true;
    private bool m_usingMonkeyAbility = false;
    private PlatformType m_currentPlatformVisibility = PlatformType.Type1;

    private float m_wallJumpTimer = 0.0f;
    private float m_abilityTimer = 0.0f;
    private float m_monkeyAbilityTimer = 0.0f;

    public bool IsGrounded { get { return m_isGrounded; } }
    public Vector2 MovementInput { get { return m_movementInput; } }
    public bool IsJumping { get { return m_isJumping; } }
    public Rigidbody2D PlayerRB { get { return m_rb; } }
    public bool IsWallJumping { get { return m_isWallJumping; } }
    public PlayerMask CurrentMask { get { return m_currentMask; } }
    public AudioSource PlayerAudioSrc { get { return m_audioSrc; } }
    public bool IsUsingMonkeyAbility { get { return m_usingMonkeyAbility; } }

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
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.gravityScale = m_gravityScale;
        m_inputActions = new PlayerInputActions();
        m_audioSrc = GetComponent<AudioSource>();
        SetUpStateMachine();
        SetUpInputActions();
        GameObject currentMaskObj = m_maskGOList[(int)PlayerMask.None];
        m_currentSpriteRenderer = currentMaskObj.GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.CurrentState.OnExecuteState();

        if(m_stateMachine.CurrentState.GetType() == typeof(PlayerClimbingState)) { }
        else if (m_movementInput.x < 0.0f && m_isLookingRight)
        {
            // Flip to the left
            FlipAllSprites(true);
            m_isLookingRight = false;
        }
        else if (m_movementInput.x > 0.0f && !m_isLookingRight)
        {
            // Flip to the right
            FlipAllSprites(false);
            m_isLookingRight = true;
        }

        if (m_isWallJumping)
        {
            m_wallJumpTimer += Time.deltaTime;
            if (m_wallJumpTimer >= m_wallJumpDuration)
            {
                m_isWallJumping = false;
                m_wallJumpTimer = 0.0f;
            }
        }

        if (!m_canUseAbility)
        {
            m_abilityTimer += Time.deltaTime;
            if (m_abilityTimer >= m_abilityCooldown)
            {
                m_canUseAbility = true;
                m_abilityTimer = 0.0f;
            }
        }

        if (m_canUseAbility && m_usingAbility)
        {
            if(m_currentMask == PlayerMask.Monkey)
            {
                m_usingMonkeyAbility = true;
                m_canUseAbility = false;
            }
            else if(m_currentMask == PlayerMask.Jaguar)
            {
                Attack();
            }
            else if(m_currentMask == PlayerMask.Axolotl)
            {
                if(selectPlatform != null)
                {
                    m_canUseAbility = false;
                    AudioManager.Instance.Play(m_audioSrc, "AxolotlVision");
                    if(m_currentPlatformVisibility == PlatformType.Type1)
                    {
                        m_currentPlatformVisibility = PlatformType.Type2;
                        selectPlatform(PlatformType.Type2);
                    }
                    else
                    {
                        m_currentPlatformVisibility = PlatformType.Type1;
                        selectPlatform(PlatformType.Type1);
                    }
                }
            }
        }

        if (m_usingMonkeyAbility)
        {
            m_monkeyAbilityTimer += Time.deltaTime;
            if (m_monkeyAbilityTimer >= m_monkeyBoostDuration)
            {
                m_usingMonkeyAbility = false;
                m_monkeyAbilityTimer = 0.0f;
            }
        }
    }

    private void FixedUpdate()
    {
        bool leftGroundDetection = Physics2D.OverlapCircle(m_lGroundCheck.position,
                                                           m_groundRadius,
                                                           m_groundLayer);
        bool rightGroundDetection = Physics2D.OverlapCircle(m_rGroundCheck.position,
                                                            m_groundRadius,
                                                            m_groundLayer);
        m_isGrounded = leftGroundDetection || rightGroundDetection;

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
        m_inputActions.Player.MonkeyMask.performed += ctx => PutMonkeyMaskOn();
        m_inputActions.Player.JaguarMask.performed += ctx => PutJaguarMaskOn();
        m_inputActions.Player.AxolotlMask.performed += ctx => PutAxolotlMaskOn();
        m_inputActions.Player.MaskAbilitie.performed += ctx => m_usingAbility = true;
        m_inputActions.Player.MaskAbilitie.canceled += ctx => m_usingAbility = false;
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

        // Preserve momentum when no input is given
        //if(m_movementInput.x == 0.0f)
        //{
        //    float decelFactor = m_isGrounded ? 0.3f : 1.0f;
        //    m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x * decelFactor, m_rb.linearVelocity.y);
        //    return;
        //}

        float controllFactor = m_isGrounded ? 1.0f : m_airControll;
        float horizontalVelocity = m_movementInput.x *
                                   (additionalVel + m_moveSpeed) *
                                   controllFactor;
        m_rb.linearVelocity = new Vector2(horizontalVelocity, m_rb.linearVelocity.y);
    }

    public void Jumping()
    {
        float additionalJump = 0.0f;
        //string audioStr = "NormalJump";
        if(m_usingMonkeyAbility)
        {
            additionalJump = m_monkeyJumpBoost;
            //audioStr = "MonkeyJump";
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
            m_rb.linearVelocity = new Vector2(direction * m_wallJumpForce.x,
                                              m_wallJumpForce.y);
            AnimatorsSetTrigger("WallJump");
        }

        //AudioManager.Instance.Play(m_audioSrc, audioStr);
    }

    public void Climbing()
    {
        m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, m_climbInput.y * m_climbSpeed);
    }

    public void Attack()
    {
        float distanceFromPlayer = m_isLookingRight ? m_attackDistance : -m_attackDistance;
        Vector3 boxPos = new Vector3(transform.position.x + distanceFromPlayer,
                                     transform.position.y,
                                     transform.position.z);
        Instantiate(m_attackBox, boxPos, Quaternion.identity);

        m_canUseAbility = false;

        AnimatorsSetTrigger("Attack");
        AudioManager.Instance.Play(m_audioSrc, "Attack");
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
            return true;
        }

        return false;
    }

    public void ResetGravityScale()
    {
        m_rb.gravityScale = m_gravityScale;
    }

    public void PutMonkeyMaskOn()
    {
        m_currentMask = PlayerMask.Monkey;

        if (axolotlMaskOn != null)
        {
            axolotlMaskOn(false);
        }

        UpdateMaskVariables();
    }

    public void PutJaguarMaskOn()
    {
        m_currentMask = PlayerMask.Jaguar;
        m_usingMonkeyAbility = false;

        if (axolotlMaskOn != null)
        {
            axolotlMaskOn(false);
        }

        UpdateMaskVariables();
    }

    public void PutAxolotlMaskOn()
    {
        m_currentMask = PlayerMask.Axolotl;
        m_usingMonkeyAbility = false;

        if (axolotlMaskOn != null)
        {
            axolotlMaskOn(true);
        }

        UpdateMaskVariables();
    }

    private void UpdateMaskVariables()
    {
        if(m_currentSpriteRenderer == null) { return; }
        m_currentSpriteRenderer.enabled = false;
        GameObject currentMaskObj = m_maskGOList[(int)m_currentMask];
        m_currentSpriteRenderer = currentMaskObj.GetComponent<SpriteRenderer>();
        m_currentSpriteRenderer.enabled = true;
        AudioManager.Instance.Play(m_audioSrc, "MaskChange");
    }

    public void AnimatorsSetBool(string name, bool active)
    {
        for(int i = 0; i < m_maskGOList.Count; i++)
        {
            m_maskGOList[i].GetComponent<Animator>().SetBool(name, active);
        }
    }

    public void AnimatorsSetTrigger(string name)
    {
        for (int i = 0; i < m_maskGOList.Count; i++)
        {
            m_maskGOList[i].GetComponent<Animator>().SetTrigger(name);
        }
    }

    private void FlipAllSprites(bool flip)
    {
        for (int i = 0; i < m_maskGOList.Count; i++)
        {
            m_maskGOList[i].GetComponent<SpriteRenderer>().flipX = flip;
        }
    }
}
