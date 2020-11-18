using System;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    #region Variables
    [Tooltip("The top run speed the player can obtain")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("The rate that the player can accelerate when running")]
    [SerializeField] private float runAccelerationRate = 1f;
    //[Tooltip("The rate that the player slows down when not inputing movement")]
    //[SerializeField] private float baseRunDecelerationRate = 10f;
    [Tooltip("The rate that the player slows down when sliding")]
    [SerializeField] private float slideDeceleration = 1f;
    //private float runDecelerationRate = .5f; //Current deceleration
    [Tooltip("How much less the movement inputs affect the player when in the air")]
    [SerializeField] private float airMovementMultiplyer = .5f;

    [SerializeField] private float jumpVelocity = 1f;
    [SerializeField] private float jumpTime = .5f;
    private float currentJumpTime = 0f;
    private bool inJump;

    private float moveInputDirection = 0f;
    private bool isGrounded = true;

    public enum playerState { normal, sliding, airborne };
    public playerState state = playerState.normal;

    private Vector2 addVelocity = Vector2.zero;

    private Rigidbody2D rb;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        moveInputDirection = Input.GetAxisRaw("Horizontal");
        
        HandleInputMovement();
        HandleJumping();
    }

    void FixedUpdate()
    {
        rb.velocity += addVelocity;
        addVelocity = Vector2.zero;
    }

    private void HandleInputMovement()
    {
        if (moveInputDirection != 0 && Mathf.Abs(rb.velocity.x) < moveSpeed)
        {
            if (state == playerState.normal)
            {
                //if(Math.Sign(rb.velocity.x) != Math.Sign(moveInputDirection))
                addVelocity.x += moveInputDirection * runAccelerationRate * Time.deltaTime; //Mathf.MoveTowards(rb.velocity.x, moveSpeed * moveInputDirection, (runDecelerationRate + runAccelerationRate) * Time.fixedDeltaTime); 
                //addVelocity.x += Mathf.MoveTowards(rb.velocity.x, moveSpeed, moveInputDirection * runAccelerationRate * Time.deltaTime);
                //else
                //addVelocity.x = Mathf.MoveTowards(rb.velocity.x, moveSpeed * moveInputDirection, runAccelerationRate * Time.fixedDeltaTime);
            }
            else if(state == playerState.airborne)
            {
                addVelocity.x += moveInputDirection * runAccelerationRate   * airMovementMultiplyer * Time.deltaTime;
            }
        }
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) //Applyes velocity once the jump button is pressed. Also prepares for the rest of the method with booleans
        {
            addVelocity.y += jumpVelocity * Time.deltaTime;
            inJump = true;
            currentJumpTime = jumpTime - Time.deltaTime;
        }
        if (Input.GetButton("Jump") && inJump) //If the jump button is held, the player will jump higher. The player can jump as high as the the extraJumpTime var allows
        {
            if (currentJumpTime > 0)
            {
                if ((currentJumpTime - Time.deltaTime) < 0)
                    addVelocity.y += jumpVelocity * (currentJumpTime % Time.deltaTime);
                else
                    addVelocity.y += jumpVelocity * Time.deltaTime;

                currentJumpTime -= Time.deltaTime;
            }
            else
                inJump = false;
        }
    }
}
