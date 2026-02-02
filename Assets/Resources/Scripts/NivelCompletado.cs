using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class NivelCompletado : MonoBehaviour
{
    [SerializeField] Button m_nextLevel;
    [SerializeField] Button m_restartLevel;
    [SerializeField] TMP_Text m_TimeText;
    private float m_timerValue = 0f;

    [SerializeField] GameObject m_panel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_nextLevel.onClick.AddListener(GameManager.Instance.NextLevel);
        m_restartLevel.onClick.AddListener(GameManager.Instance.ResetLevel);
    }

    private void Update()
    {
        if (GameManager.Instance.LevelFinish)
        {
            activate();
        }
    }
    private void activate()
    {
        m_timerValue = GameManager.Instance.SetTimeLevelFinish;
        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(m_timerValue / 60); // Total minutes
        int seconds = Mathf.FloorToInt(m_timerValue % 60); // Seconds portion

        // Format the string with leading zeros
        // The "00" format specifier ensures a two-digit number
        m_TimeText.text = "Tiempo: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        //m_TimeText.text = "Time: " + GameManager.Instance.SetTimeLevelFinish;
    }

}
