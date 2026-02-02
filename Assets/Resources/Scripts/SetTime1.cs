using UnityEngine;

public class SetTime1 : MonoBehaviour
{
    public static SetTime1 Instance { get; private set; }

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
    void Start()
    {
    }

    private void Update()
    {
        Time.timeScale = 1.0f;
    }

}
