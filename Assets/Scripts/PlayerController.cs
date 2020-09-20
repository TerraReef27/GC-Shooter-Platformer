using System;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Tooltip("The top run speed the player can obtain")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("The rate that the player can accelerate when running")]
    [SerializeField] private float runAccelerationRate = 1f;
    [Tooltip("The rate that the player slows down when not inputing movement")]
    [SerializeField] private float baseRunDecelerationRate = 10f;
    [Tooltip("The rate that the player slows down when sliding")]
    [SerializeField] private float slideDeceleration = 1f;
    private float runDecelerationRate = .5f; //Current deceleration
    [Tooltip("How much less the movement inputs affect the player when in the air")]
    [SerializeField] private float airMovementMultiplyer = .5f;

    [SerializeField] private CapsuleCollider2D playerCollider = null;
    [SerializeField] private BoxCollider2D slideCollider = null;

    private Vector2 outsideMoveInfluence; //Force coming from outside influences

    [SerializeField] private float jumpVelocity = 1f;
    [SerializeField] private float jumpTime = .5f;
    private float currentJumpTime = 0f;
    private bool inJump;

    public float animationXMovement; //Seperate variable so changes do not affect movement

    public enum playerState { neutral, sliding, airborne };
    public playerState state = playerState.neutral;

    protected override void ComputeVelocity()
    {
        Vector2 moveTo = Vector2.zero;

        float moveInputDirection = Input.GetAxisRaw("Horizontal");
        animationXMovement = moveInputDirection; //Used for the animator to determine input direction

        if (moveInputDirection != 0 && state != playerState.sliding) //Move by values when inputing on the x-axis (change acceleration when in the air vs on the ground)
        {
            if (grounded)
                moveTo.x = Mathf.MoveTowards(moveTo.x, moveSpeed * moveInputDirection, runAccelerationRate * Time.deltaTime);
            else
                moveTo.x = Mathf.MoveTowards(moveTo.x * airMovementMultiplyer, moveSpeed * moveInputDirection, runAccelerationRate * Time.deltaTime);
        }
        else if(grounded) //When not inputing on ground, slow by decelerationRate
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, runDecelerationRate * Time.deltaTime);
        }

        HandleJumping();
        HandleInput();

        ApplyForce(ref moveTo, ref outsideMoveInfluence);

        targetVelocity = moveTo;
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && grounded) //Applyes velocity once the jump button is pressed. Also prepares for the rest of the method with booleans
        {
            velocity.y += jumpVelocity * Time.deltaTime;
            inJump = true;
            currentJumpTime = jumpTime;
        }
        if (Input.GetButton("Jump") && inJump) //If the jump button is held, the player will jump higher. The player can jump as high as the the extraJumpTime var allows
        {
            if (currentJumpTime > 0)
            {
                velocity.y += jumpVelocity * Time.deltaTime;
                currentJumpTime -= Time.deltaTime;
            }
            else
                inJump = false;
        }
    }
    
    public void AddForce(Vector2 newForce) //Used to add a force to the object from outside of the script
    {
        outsideMoveInfluence += newForce;
        return;
    }

    private void ApplyForce(ref Vector2 applyTo, ref Vector2 applicant) //Applies the outside forces to this object
    {
        applyTo += applicant;
        applicant = Vector2.zero;
        return;
    }

    private void HandleInput() //Recives and reacts to inputs
    {
        if(Input.GetButton("Slide") && grounded) //Slide
        {
            runDecelerationRate = slideDeceleration;
            state = playerState.sliding;
            //ToggleActiveHitbox(slideCollider, playerCollider);
        }
        else if(runDecelerationRate <= 1 && state == playerState.sliding)
        {
            //ToggleActiveHitbox(playerCollider, slideCollider);

            runDecelerationRate = baseRunDecelerationRate;
            state = playerState.neutral;             
        }
    }

    /*
     * TODO: Fix this!
    private void ToggleActiveHitbox(Collider2D activate, Collider2D disable)
    {
        
        float activateSize = activate.bounds.size.y;
        float disableSize = disable.bounds.size.y;
        float offset = (activateSize - disableSize)/2;
        Debug.Log(offset);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + offset, gameObject.transform.position.z);
        disable.enabled = false;
        activate.enabled = true;
    }
    */
}
