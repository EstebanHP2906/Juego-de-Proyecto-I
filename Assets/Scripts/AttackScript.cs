using UnityEngine;

public class AttackScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
            Debug.Log("Golpe");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
