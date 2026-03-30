using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] int Vida = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Damage"))
        {
            Debug.Log("Vida: " + (Vida-=1).ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vida <= 0)
        {
            Destroy(gameObject);
        }
    }
}
