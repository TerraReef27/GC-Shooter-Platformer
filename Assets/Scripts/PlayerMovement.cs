using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb = null;

    private Vector2 movement; //The direction the player is inputing

    [Tooltip("Amount of velocity to the object's jump")]
    [SerializeField] private float jumpVelocity = 10f;
    [Tooltip("Amount of force for object's input movement")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("The maximum force that the object can achive using inputs")]
    [SerializeField] private float maxMoveSpeed = 10f;
    [Tooltip("How much the gravity is multiplied by when the object is falling")]
    [SerializeField] private float gravityMultiplyer = 2.5f;
    private float baseGravity;

    [Tooltip("The layermask that the object will recognize as ground")]
    [SerializeField] LayerMask layer;
    private bool isGrounded; //Boolean to check if the object is touching the ground
    [Tooltip("The size of the box below the object that checks if it is touching the ground")]
    [SerializeField] private float groundCheckSize = 1f;
    [Tooltip("The amount less than the sprite that the ground check will register. Reduce to prevent jumping up walls")]
    [SerializeField] private float groundCheckXSizeReduction = .3f;

    [Tooltip("The amout the the object's input control is multiplied by in the air")]
    [SerializeField] private float airSpeedMultiplier = .5f;

    private bool inJump = false; //Check to see if the player is jumping or not
    [Tooltip("The time that the player can get extra height on their jump")]
    [SerializeField] private float extraJumpTime = .2f;
    private float currentJumpTime = .2f; //Time to check for extra height while jumping

    private RespawnSystem respawn = null; //Reference to the respawn system in the scene

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        respawn = FindObjectOfType<RespawnSystem>();
        respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn; ; //Subscribe object to the OnPlayerRespawn event
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Sets position to respawn point and velocity to 0
    {
        transform.position = respawnPos;
        rb.velocity = Vector2.zero;
    }

    void Start()
    {
        baseGravity = rb.gravityScale; //Get the starting gravity so that it can be reset if needed
    }

    void Update()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), 0); //Set the movement vector to the horizontal inputs

        HandleJumping();

        if (Input.GetKeyDown(KeyCode.Q))
            respawn.Respawn();
    }

    void FixedUpdate()
    {
        //Check if the player is touching the ground. The surface it is overlaping must be on the "Ground" layer mask layer
        isGrounded = Physics2D.OverlapArea(new Vector2(transform.position.x - .5f + groundCheckXSizeReduction, transform.position.y - .5f), new Vector2(transform.position.x + .5f - groundCheckXSizeReduction, transform.position.y - .5f - groundCheckSize), layer);

        if (rb.velocity.y < 0) //Checks if the player is falling. If they are, increase they speed at which they fall for a smoother feel
            rb.gravityScale = gravityMultiplyer;
        else
            rb.gravityScale = baseGravity;

        rb.drag = moveSpeed / maxMoveSpeed; //Adds drag to the rigidbody so that it will not grow exponetially fast
        if(isGrounded)

            rb.AddForce(movement.normalized * moveSpeed * Time.fixedDeltaTime); //Applies a force relative to the world coordinates in the direction of the current input
        else
            rb.AddForce(movement.normalized * moveSpeed * airSpeedMultiplier * Time.fixedDeltaTime); //If in the air, limit mobility
            
    }

    private void HandleJumping()
    {
        //LEGACY (here for safe keeping): rb.AddForce(Vector2.up * jumpForce); //Makes the player jump directly up. Jump height is relative to the jumpForce value

        if (Input.GetButtonDown("Jump") && isGrounded) //Applyes velocity once the jump button is pressed. Also prepares for the rest of the method with booleans
        {
            rb.velocity += Vector2.up * jumpVelocity;
            inJump = true;
            currentJumpTime = extraJumpTime;
        }
        if (Input.GetButton("Jump") && inJump) //If the jump button is held, the player will jump higher. The player can jump as high as the the extraJumpTime var allows
        {
            if (currentJumpTime > 0)
            {
                rb.velocity += Vector2.up * jumpVelocity;
                currentJumpTime -= Time.deltaTime;
            }
            else
                inJump = false;
        }
    }
}
