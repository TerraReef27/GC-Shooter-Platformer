using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveAccelerationRate = 1f;
    [SerializeField] private float moveDecelerationRate = 3f;

    [SerializeField] private float gravity;
    [SerializeField] private Vector2 velocity;
    private float moveInputDirection;
    private Vector3 moveTo;

    private BoxCollider2D coll = null;

    private bool isGrounded = false;

    [SerializeField] private float jumpVelocity = 1f;
    [SerializeField] private float jumpTime = .3f;
    private float currentJumpTime = 0f;
    private bool inJump = false;
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        //physicsTransform.position = transform.position;
    }
    
    void Update()
    {
        moveInputDirection = Input.GetAxisRaw("Horizontal");

        if(moveInputDirection != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * moveInputDirection, moveAccelerationRate * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, moveDecelerationRate * Time.deltaTime);
        }

        if(!isGrounded)
            velocity.y += -gravity * Time.deltaTime;
        
        CollisionCheck();
        HandleJumping();
        moveTo = velocity * Time.deltaTime;

        transform.Translate(moveTo);
        //transform.position = physicsTransform.position;
        Debug.DrawRay(transform.position, velocity, Color.red);
        //Debug.DrawRay(transform.position, moveTo, Color.green);
    }

    private void CollisionCheck()
    {
        Collider2D[] collisions = Physics2D.OverlapBoxAll(transform.position, coll.size, 0f);

        foreach (Collider2D collision in collisions)
        {
            if (collision.gameObject != gameObject)
            {
                ColliderDistance2D collisionDistance = collision.Distance(coll);

                if (collisionDistance.isOverlapped)
                {
                    if(collisionDistance.pointA.x - collisionDistance.pointB.x < 0 && moveTo.x > 0)
                    {
                        velocity.x = 0;
                    }
                    else if(collisionDistance.pointA.x - collisionDistance.pointB.x > 0 && moveTo.x < 0)
                    {
                        velocity.x = 0;
                    }

                    if(collisionDistance.pointA.y - collisionDistance.pointB.y > 0 && moveTo.y < 0)
                    {
                        velocity.y = 0;
                        isGrounded = true;
                    }
                    else
                    {
                        isGrounded = false;
                    }
                    if (collisionDistance.pointA.y - collisionDistance.pointB.y < 0 && moveTo.y > 0)
                    {
                        velocity.y = 0;
                    }
                }
            }
        }
    }

    private void HandleJumping()
    {
        //LEGACY (here for safe keeping): rb.AddForce(Vector2.up * jumpForce); //Makes the player jump directly up. Jump height is relative to the jumpForce value

        if (Input.GetButtonDown("Jump") && isGrounded) //Applyes velocity once the jump button is pressed. Also prepares for the rest of the method with booleans
        {
            velocity += Vector2.up * jumpVelocity;
            inJump = true;
            currentJumpTime = jumpTime;
        }
        if (Input.GetButton("Jump") && inJump) //If the jump button is held, the player will jump higher. The player can jump as high as the the extraJumpTime var allows
        {
            if (currentJumpTime > 0)
            {
                velocity += Vector2.up * jumpVelocity;
                currentJumpTime -= Time.deltaTime;
            }
            else
                inJump = false;
        }
    }
}
