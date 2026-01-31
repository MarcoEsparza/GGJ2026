using UnityEngine;

public class LevelStructure : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_score = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckVictory()) {
            OnExit();
            GameManager.Instance.NextLevel();
        }
    }

    /// <summary>
    /// Check if the player is near the victory point.
    /// </summary>
    /// <returns>If the victory point is in range.</returns>
    public bool CheckVictory()
    {
        if (!m_endPoint || !m_player) {
            return false;
        }
        // could be changed to something else.
        return Vector3.Distance(m_player.transform.position,
                                m_endPoint.transform.position) < m_distanceVictoryCheck;
    }

    /// <summary>
    /// When the player leaves the level.
    /// </summary>
    public void OnExit()
    {
        // json for scores
    }

    /// <summary>
    /// Add points to the current level score.
    /// </summary>
    /// <param name="_score">Score to be added.</param>
    public void AddScore(float _score)
    {
        m_score += _score;
    }

    /// <summary>
    /// Rewrite the current score of the level.
    /// </summary>
    /// <param name="_score">New score.</param>
    public void SetScore(float _score)
    {
        m_score = _score;
    }

    /// <summary>
    /// Get the current level score.
    /// </summary>
    /// <returns></returns>
    public float GetScore()
    {
        return m_score;
    }

    [SerializeField] private float m_score;
    [SerializeField] private Transform m_endPoint;
    [SerializeField] private GameObject m_player; // to do: placeholder
    [SerializeField] private float m_distanceVictoryCheck = 1.0f;
}
