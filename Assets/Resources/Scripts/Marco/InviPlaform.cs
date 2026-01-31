using UnityEngine;

public enum PlatformType
{
    Type1 = 0,
    Type2 = 1
}

public class InviPlaform : MonoBehaviour
{
    //[SerializeField] private bool m_isVisible = false;
    [SerializeField] private float m_lowerAlpha = 0.3f;
    [SerializeField] private PlatformType m_platformType = PlatformType.Type1;

    private Collider2D m_collider;
    private SpriteRenderer m_spriteRenderer;
    private PlatformType m_selectedPlatform = PlatformType.Type1;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        if (m_collider == null)
        {
            Debug.LogError("Collider2D component is missing on InviPlaform GameObject.");
        }
        if (m_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on InviPlaform GameObject.");
        }
        m_spriteRenderer.enabled = false;
        m_collider.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerController.axolotlMaskOn += ToogleVisibility;
        PlayerController.selectPlatform += TooglePlatformType;

        if(m_platformType != m_selectedPlatform)
        {
            m_spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, m_lowerAlpha);
            m_collider.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToogleVisibility(bool visible)
    {
        m_spriteRenderer.enabled = visible;

        if (visible && m_platformType == m_selectedPlatform)
        {
            m_collider.enabled = true;
        }
        else
        {
            m_collider.enabled = false;
        }
    }

    public void TooglePlatformType(PlatformType type)
    {
        m_selectedPlatform = type;

        if (m_selectedPlatform == m_platformType)
        {
            m_spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            m_collider.enabled = true;
        }
        else
        {
            m_spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, m_lowerAlpha);
            m_collider.enabled = false;
        }
    }
}
