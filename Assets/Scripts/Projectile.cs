using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        StartCoroutine(Despawn());
    }

    public void Shot(float speed, Vector2 direction)
    {
        rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Projectile")
        {
            Destroy(gameObject);
        }
    }
}
