using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        GetComponent<CircleCollider2D>().radius = m_attackRange;
        SetUpStateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.CurrentState.OnExecuteState();
    }

    private void FixedUpdate()
    {
        m_isGrounded = IsGrounded();
    }

    // adds the target when the player enters the sight range.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_target = collision.transform;
        }
        // if (collision.CompareTag("Attack"))
        // {
        //     gameObject.SetActive(false);
        // }
    }

    // remove the target when the player exits the sight range.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_target = null;
        }
    }

    /// <summary>
    /// Check if the target is in line of sight.
    /// </summary>
    /// <returns>Wether the line is clear or not.</returns>
    public bool
    InLineOfSight()
    {
        // if there is no target to seek, ignore.
        if (!m_target) {
            return false;
        }

        // shoot the raycast towards the target.
        Vector2 direction = (m_target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                                             direction,
                                             m_attackRange,
                                             ~m_enemyLayer);

        if (hit.collider && hit.collider.CompareTag("Player")) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if the entity is grounded.
    /// </summary>
    /// <returns>Wether the entity is grounded or not.</returns>
    public bool IsGrounded()
    {
        m_isGrounded = Physics2D.Raycast(m_feet.position,
                                         Vector2.down,
                                         m_groundCheckDist,
                                         m_groundLayer);
        return m_isGrounded;
    }

    // to do: find in the future a better way to implement this movement
    public void MoveForward()
    {
        m_rb.linearVelocity = new Vector2(m_speed * m_direction, m_rb.linearVelocityY);
    }

    /// <summary>
    /// Get the current target of the enemy.
    /// </summary>
    /// <returns>Target of the enemy.</returns>
    public Transform GetTarget()
    {
        return m_target;
    }

    /// <summary>
    /// Set the target of the enemy.
    /// </summary>
    /// <param name="_target"></param>
    public void SetTarget(ref Transform _target)
    {
        m_target = _target;
    }

    /// <summary>
    /// Get the direction the enemy is currently looking towards.
    /// </summary>
    /// <returns>The current direction (-1 to 1)</returns>
    public float GetDirection()
    {
        return m_direction;
    }

    /// <summary>
    /// Set the direction the enemy will be facing.
    /// </summary>
    /// <param name="_direction">New direction(-1 to 1)</param>
    public void SetDirection(float _direction)
    {
        m_direction = _direction;
    }

    /// <summary>
    /// Check in front of the enemy for ground, if the enemy does not find ground, return said state.
    /// </summary>
    /// <returns>If there's ground in front of the enemy.</returns>
    public bool CheckForward()
    {
        return Physics2D.Raycast(transform.position,
                                 Vector2.right * m_direction,
                                 m_forwardCheckDistance,
                                 ~m_enemyLayer);
        
    }

    public bool CheckDown()
    {
        // get the point where the raycast will start depending on where the entity is looking towards
        Vector2 origin = transform.position;
        origin.x += m_forwardCheckDistance * m_direction;
        return Physics2D.Raycast(origin, Vector2.down, m_forwardCheckDistance, m_groundLayer);
    }

    void SetUpStateMachine()
    {
        m_stateMachine = new StateMachine();
        m_stateMachine.Owner = this.gameObject;
        // Adding states to the state machine
        m_stateMachine.AddState(new EnemyMoveState(), "Move");
        m_stateMachine.AddState(new EnemyAttackState(), "Attack");
        // Setting initial state
        m_stateMachine.ChangeState("Move");
    }

    /// <summary>
    /// Get the time the attack state should last.
    /// </summary>
    /// <returns></returns>
    public float GetAttackDuration() { return m_attackDuration; }

    /// <summary>
    /// Get the time the attack will take from start to hit.
    /// </summary>
    /// <returns></returns>
    public float GetAttackDelay() { return m_attackDelay; }

    /// <summary>
    /// Get the elapsed attack time.
    /// </summary>
    /// <returns></returns>
    public float GetAttackTime() { return m_attackTimer; }

    /// <summary>
    /// Count the attack time.
    /// </summary>
    public void TickAttackTime() { m_attackTimer += Time.deltaTime; }

    /// <summary>
    /// Reset the attack timer.
    /// </summary>
    public void ResetAttackTime() { m_attackTimer = 0.0f; }

    private Rigidbody2D m_rb = null;
    private Transform m_target = null;
    [HideInInspector] public bool m_isGrounded = false;

    [Header("Attack parameters")]
    [SerializeField] private float m_attackRange = 1.0f;
    [SerializeField] private float m_attackDelay = 0.5f;
    [SerializeField] private float m_attackDuration = 1.0f;
    [SerializeField] private float m_attackTimer = 0.0f;
    
    [Header("Movement parameters")]
    private float m_direction = 1f;
    [SerializeField] private float m_speed = 1.0f;
    [SerializeField] private float m_forwardCheckDistance = 1.0f;


    [Header("Grounded parameters")]
    [SerializeField] private Transform m_feet;
    [SerializeField] private float m_groundCheckDist = 0.1f;
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private LayerMask m_enemyLayer;

    private StateMachine m_stateMachine;
}
