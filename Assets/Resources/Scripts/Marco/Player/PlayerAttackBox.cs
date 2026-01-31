using UnityEngine;

public class PlayerAttackBox : MonoBehaviour
{
    private float lifetime = 0.2f;
    private float timer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= lifetime)
        {
            Destroy(this.gameObject);
        }
    }
}
