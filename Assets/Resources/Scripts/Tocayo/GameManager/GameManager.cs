using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(m_endUIPrefab);
        m_times = new List<float>(new float[m_levelNames.Count]);
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
    }

    /// <summary>
    /// Pause the level and activate the UI.
    /// </summary>
    public void EndLevel()
    {
        Time.timeScale = 0.0f;
        m_endUIPrefab.SetActive(true);
    }

    // Reset the currently selected level.
    public void ResetLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Change the current level.
    /// </summary>
    public void NextLevel()
    {
        m_endUIPrefab.SetActive(false);
        Time.timeScale = 1.0f;
        // if the time can be counted
        if (m_levelIndex <= m_times.Count - 1) {
            m_times[m_levelIndex] = m_timer;
            m_timer = 0.0f;
        }
        ++m_levelIndex;

        // if the index is out of range.
        if (m_levelIndex > m_levelNames.Count - 1) {
            // to do: load final score scene.
            return;
        }

        string lvlName = m_levelNames[m_levelIndex];
        SceneManager.LoadScene(lvlName);
    }

    /// <summary>
    /// In case the player leaves the main game loop completely.
    /// </summary>
    public void ExitGameLoop() // to do: give a more comprehensive name.
    {
        m_levelIndex = 0;
        m_timer = 0.0f;
    }

    private int m_levelIndex = 0;
    [SerializeField] private float m_timer;
    [SerializeField] private List<float> m_times;
    [SerializeField] private List<string> m_levelNames;

    [Header("UI Elements")]
    [SerializeField] private GameObject m_endUIPrefab;
}
