using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_detectCollider = GetComponent<CircleCollider2D>();
        m_detectCollider.radius = m_sightRange;
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
            print("found player");
            m_target = collision.transform;
        }
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
    public bool InLineOfSight()
    {
        // if there is no target to seek, ignore.
        if (!m_target) {
            return false;
        }

        // shoot the raycast towards the target.
        Vector2 direction = (m_target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                                             direction,
                                             m_sightRange,
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
    public Transform
    GetTarget()
    {
        return m_target;
    }

    /// <summary>
    /// Set the target of the enemy.
    /// </summary>
    /// <param name="_target"></param>
    public void
    SetTarget(ref Transform _target)
    {
        m_target = _target;
    }

    /// <summary>
    /// Get the direction the enemy is currently looking towards.
    /// </summary>
    /// <returns>The current direction (-1 to 1)</returns>
    public float
    GetDirection()
    {
        return m_direction;
    }

    /// <summary>
    /// Set the direction the enemy will be facing.
    /// </summary>
    /// <param name="_direction">New direction(-1 to 1)</param>
    public void
    SetDirection(float _direction)
    {
        m_direction = _direction;
    }

    /// <summary>
    /// Check in front of the enemy for ground, if the enemy does not find ground, return said state.
    /// </summary>
    /// <returns>If there's ground in front of the enemy.</returns>
    public bool CheckForward()
    {
        // get the point where the raycast will start depending on where the entity is looking towards
        Vector2 origin = transform.position;
        origin.x += m_forwardCheckDistance * m_direction;

        return Physics2D.Raycast(origin,
                                 Vector2.down,
                                 m_downCheckDistance,
                                 m_groundLayer);
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

    private Rigidbody2D m_rb = null;
    private Transform m_target = null;
    private CircleCollider2D m_detectCollider = null;
    [HideInInspector] public bool m_isGrounded = false;
    private float m_direction = 1f;

    [SerializeField] private float m_sightRange;
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private float m_forwardCheckDistance = 1f;
    [SerializeField] private float m_downCheckDistance = 1f;


    [Header("Grounded parameters")]
    [SerializeField] private Transform m_feet;
    [SerializeField] private float m_groundCheckDist;
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private LayerMask m_enemyLayer;

    private StateMachine m_stateMachine;
}
