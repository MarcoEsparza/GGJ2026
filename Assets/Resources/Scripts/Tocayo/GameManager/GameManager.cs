using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Change the current level.
    /// </summary>
    public void
    NextLevel()
    {
        // if the index is out of range.
        if (m_levelIndex > m_levelNames.Count - 1) {
            // to do: load final score scene.
            return;
        }

        string lvlName = m_levelNames[m_levelIndex];

        SceneManager.LoadScene(lvlName);
        ++m_levelIndex;
    }

    /// <summary>
    /// In case the player leaves the main game loop completely.
    /// </summary>
    public void
    ExitGameLoop() // to do: give a more comprehensive name.
    {
        m_levelIndex = 0;
    }

    private int m_levelIndex = 0;

    [SerializeField] private List<string> m_levelNames;
}
