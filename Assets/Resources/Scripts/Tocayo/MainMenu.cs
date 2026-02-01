using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Main function to start the game.
    /// </summary>
    public void StartGame()
    {
      UnityEngine.SceneManagement.SceneManager.LoadScene(m_startLevel);
    }

    /// <summary>
    /// Button behaviour to exit the game.
    /// </summary>
    public void ExitGame()
    {
      Application.Quit();
    }

    /// <summary>
    /// Button behaviour to open the settings menu.
    /// </summary>
    /// <param name="_active">Settings menu state.</param>
    public void OpenSettings()
    {
      OpenSettingsMenu(true);
    }

    /// <summary>
    /// Returns the user to the main menu screen.
    /// </summary>
    public void ReturnToMenu()
    {
      OpenSettingsMenu(false);
    }

    public void OpenSettingsMenu(bool _active)
    {
      m_mainMenu.SetActive(!_active);
      m_settingsMenu.SetActive(_active);
    }

    [Header("UI Element groups")]
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_settingsMenu;

    [Header("Whats the name of the first level to load.")]
    [SerializeField] private string m_startLevel;
}
