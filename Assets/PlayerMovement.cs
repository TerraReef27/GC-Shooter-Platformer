using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb = null;

    private Vector3 movement; //The direction the player is inputing

    [Tooltip("Amount of force to the object's jump")]
    [SerializeField] private float jumpForce = 10f;
    [Tooltip("Amount of force for object's input movement")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("How much the gravity is multiplied by when the object is falling")]
    [SerializeField] private float gravityMultiplyer = 2.5f;
    private float baseGravity;

    [Tooltip("The layermask that the object will recognize as ground")]
    [SerializeField] LayerMask layer;
    private bool isGounded;
    [Tooltip("The size of the box below the object that checks if it is touching the ground")]
    [SerializeField] private float groundCheckSize = 1f;

    [Tooltip("The amout the the object's input control is multiplied by in the air")]
    [SerializeField] private float airSpeedMultiplier = .5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        baseGravity = rb.gravityScale; //Get the starting gravity so that it can be reset if needed
    }

    void Update()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), 0); //Set the movement vector to the horizontal inputs

        if (Input.GetButtonDown("Jump") && isGounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        //Check if the player is touching the ground. The surface it is overlaping must be on the "Ground" layer mask layer
        isGounded = Physics2D.OverlapArea(new Vector2(transform.position.x - .3f, transform.position.y - .5f), new Vector2(transform.position.x + .3f, transform.position.y - .5f - groundCheckSize), layer);

        if (rb.velocity.y < 0) //Checks if the player is falling. If they are, increase they speed at which they fall for a smoother feel
        {
            //Possibly change to simply adjust rigidbody's gravity
            //rb.AddForce(Vector2.up * Physics2D.gravity.y * (gravityMultiplyer - 1) * Time.fixedDeltaTime);
            //rb.gravityScale = gravityMultiplyer;
            gravityMultiplyer *= 2f;
        }
        else
            rb.gravityScale = baseGravity;

        //possible new move system
        //rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);

        //rb.AddForce(-transform.up * gravityMultiplyer);

        
        if(isGounded)
            rb.AddForce(movement.normalized * moveSpeed * Time.fixedDeltaTime); //Applies a force relative to the world coordinates in the direction of the current input
        else
            rb.AddForce(movement.normalized * moveSpeed * airSpeedMultiplier * Time.fixedDeltaTime); //If in the air, limit mobility
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce); //Makes the player jump directly up. Jump height is relative to the jumpForce value
    }
}
