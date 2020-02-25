using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    BoxCollider2D bCollider;
    [Tooltip("The respawn system to call when colliding")]
    [SerializeField] RespawnSystem respawner = null;

    void Awake()
    {
        bCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Killer") //If the object collides with a collider tagged killer, the object will respawn
            respawner.Respawn();
    }
}
