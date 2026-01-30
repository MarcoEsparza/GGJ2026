using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    // temporary state machine for simple enemy AI
    enum STATE
    {
        PATROL,
        ATTACK
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_detectCollider = GetComponent<CircleCollider2D>();
        m_detectCollider.radius = m_sightRange;
    }

    // Update is called once per frame
    void Update()
    {
        machineState();
    }

    /// <summary>
    /// Swap between the enemy states as needed
    /// </summary>
    private void machineState()
    {
        m_state = STATE.PATROL;
        m_isGrounded = CheckGround();

        if (m_isGrounded && !CheckForward()) {
            m_direction *= -1f;
        }

        if (m_target) { m_state = STATE.ATTACK; }
        
        MoveForward();
    }

    /// <summary>
    /// Check if the target is within sight range.
    /// </summary>
    /// <returns>If the entity can see the target.</returns>
    // private bool Sight()
    // {
    //     OnTriggerEnter2D(m_detectCollider);
    // }

    // adds the target when the player enters the sight range.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
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
    /// Check if the entity is grounded.
    /// </summary>
    /// <returns>Wether the entity is grounded or not.</returns>
    private bool CheckGround()
    {
        return Physics2D.Raycast(m_feet.position,
                                 Vector2.down,
                                 m_groundCheckDist,
                                 m_groundLayer);
    }

    // to do: find in the future a better way to implement this movement
    private void MoveForward()
    {
        m_rb.linearVelocity = new Vector2(m_speed * m_direction, m_rb.linearVelocityY);
    }

    /// <summary>
    /// Check in front of the enemy for ground, if the enemy does not find ground, return said state.
    /// </summary>
    /// <returns>If there's ground in front of the enemy.</returns>
    private bool CheckForward()
    {
        // get the point where the raycast will start depending on where the entity is looking towards
        Vector2 origin = transform.position;
        origin.x += m_forwardCheckDistance * m_direction;

        m_mrStart.transform.position = origin;
        m_mrEnd.transform.position = origin + (Vector2.down * m_groundCheckDist);

        return Physics2D.Raycast(origin,
                                 Vector2.down,
                                 m_downCheckDistance,
                                 m_groundLayer);
    }

    // organize this later on.

    private Rigidbody2D m_rb;
    private Transform m_target;
    private CircleCollider2D m_detectCollider;
    private bool m_isGrounded = false;
    private float m_direction = 1f;

    [SerializeField] private float m_sightRange;
    [SerializeField] private STATE m_state = STATE.PATROL;

    [Header("Movement parameters")]
    [SerializeField] private float m_speed = 1.0f;
    [SerializeField] private float m_forwardCheckDistance;
    [SerializeField] private float m_downCheckDistance;

    [Header("Grounded parameters")]
    [SerializeField] private Transform m_feet;
    [SerializeField] private float m_groundCheckDist;
    [SerializeField] private LayerMask m_groundLayer; // temporary variable for ground testing.

    // VISUAL TESTING
    [SerializeField] private GameObject m_mrStart;
    [SerializeField] private GameObject m_mrEnd;
}
