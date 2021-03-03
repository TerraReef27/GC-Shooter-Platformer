using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float minJumpHeight = 1f;
    [SerializeField] private float maxJumpHeight = 2f;
    [SerializeField] private float jumpTime = 0.3f;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundAcceleration = .2f;
    [SerializeField] private float airAcceleration = .1f;
    [SerializeField] private float airDeceleration = .1f;
    [SerializeField] private float groundDecleration = .2f;
    [SerializeField] private float slideDeceleration = .01f;
    [SerializeField] private float wallSlideSpeed = 1f;
    [SerializeField] private Vector2 wallJump = Vector2.zero;
    private int wallDirection;
    private float gravity;
    private float minJumpVelocity;
    private float maxJumpVelocity;

    private PhysicsController physics;
    private Vector3 velocity;
    public Vector3 Velocity { get { return velocity; } private set { velocity = Velocity; } }
    
    private float horizontalInput;
    public float HorizontalInput { get { return horizontalInput; } private set { horizontalInput = HorizontalInput; } }
    private float targetMoveSpeed;

    public enum playerState { neutral, sliding, airborne, wallSliding };
    public playerState state = playerState.neutral;
    public bool grounded;

    private RespawnSystem respawn = null;

    void Awake()
    {
        physics = GetComponent<PhysicsController>();

        respawn = FindObjectOfType<RespawnSystem>();
        respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn; //Subscribe object to the OnPlayerRespawn event
    }

    void Start()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(jumpTime, 2); //Based off of the equation deltaMove = initialVelocity * time + (acceleration * time^2)/2
        maxJumpVelocity = Mathf.Abs(gravity) * jumpTime; //Based off of the equation finalVelocity = initialVelocity + acceleration * time
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity + minJumpHeight)); //based off the euqation minJumpForce = sqrt(2 * gravity * minJumpHeight)

        state = playerState.neutral;
    }

    void Update()
    {
        grounded = physics.Info.isBelow;
        wallDirection = (physics.Info.isLeft) ? -1 : 1;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        targetMoveSpeed = moveSpeed * horizontalInput;

        HandleWallSlide();
        HandleJumping();
        HandleInput();

        if (state == playerState.airborne && physics.Info.isBelow)
            state = playerState.neutral;
    }

    void FixedUpdate()
    {
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetMoveSpeed, ref moveSmoothing, (physicsController.Info.isBelow) ? groundAcceleration : airAcceleration); //Accelerate with smoothing according to if the character is grounded or not 

        if(horizontalInput != 0 && Mathf.Abs(velocity.x) <= moveSpeed && state != playerState.sliding)
        {
            if(Mathf.Sign(velocity.x) != Mathf.Sign(horizontalInput))
            {
                velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, (physics.Info.isBelow) ? groundAcceleration*2 : airAcceleration*2);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, (physics.Info.isBelow) ? groundAcceleration : airAcceleration);
            }
        }
        else
        {
            if(state != playerState.sliding)
                velocity.x = Mathf.MoveTowards(velocity.x, 0, (physics.Info.isBelow) ? groundDecleration : airDeceleration);
            else
                velocity.x = Mathf.MoveTowards(velocity.x, 0, (physics.Info.isBelow) ? slideDeceleration : airDeceleration);
        }
        
        velocity.y += gravity * Time.fixedDeltaTime;
        physics.Move(velocity * Time.fixedDeltaTime, false);

        if (physics.Info.isBelow || physics.Info.isAbove)
        {
            velocity.y = 0;
        }
        if (physics.Info.isLeft || physics.Info.isRight)
        {
            velocity.x = 0;
        }
    }

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Slide") && physics.Info.isBelow && state != playerState.sliding) //&& Mathf.Abs(velocity.x) > 1) //Slide
        {
            state = playerState.sliding;
            GetComponentInChildren<ColliderController>().SwitchActive();
            physics.ChangeActiveCollider();
        }
        if(state == playerState.sliding && (Input.GetButtonUp("Slide") || !physics.Info.isBelow))
        {
            state = playerState.neutral;
            GetComponentInChildren<ColliderController>().SwitchActive();
            physics.ChangeActiveCollider();
        }

        if (Input.GetAxis("Vertical") < 0f)
        {
            physics.FallThroughPlatform();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            respawn.Respawn();
        }
    }

    private void HandleJumping()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if(state == playerState.wallSliding)
            {
                AddForce(new Vector2(wallJump.x * -wallDirection, wallJump.y));
                //velocity.x = wallJump.x * wallDirection;
                //velocity.y = wallJump.y;
            }
            if (physics.Info.isBelow)
            {
                velocity.y = maxJumpVelocity;
            }    
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > minJumpVelocity)
                velocity.y = minJumpVelocity;
        }
    }

    private void HandleWallSlide()
    {
        if (((physics.Info.isLeft && horizontalInput < -.1) || (physics.Info.isRight && horizontalInput > .1)) && !physics.Info.isBelow)
        {
            state = playerState.wallSliding;
            if (velocity.y < -wallSlideSpeed)
                velocity.y = -wallSlideSpeed;
        }
        else if (state == playerState.wallSliding)
        {
            if (physics.Info.isBelow)
                state = playerState.neutral;
            else
                state = playerState.airborne;
        }
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Sets position to respawn point and velocity to 0
    {
        transform.position = respawnPos;
        velocity = Vector2.zero;
    }
}
