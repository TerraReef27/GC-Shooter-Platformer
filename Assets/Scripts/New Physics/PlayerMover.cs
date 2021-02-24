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

    private float gravity;
    private float minJumpVelocity;
    private float maxJumpVelocity;

    private PhysicsController physicsController;
    private Vector3 velocity;
    public Vector3 Velocity { get { return velocity; } private set { velocity = Velocity; } }
    
    private float horizontalInput;
    private float targetMoveSpeed;

    public enum playerState { neutral, sliding, airborne };
    public playerState state = playerState.neutral;

    private RespawnSystem respawn = null;

    void Awake()
    {
        physicsController = GetComponent<PhysicsController>();

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
        horizontalInput = Input.GetAxisRaw("Horizontal");
        targetMoveSpeed = moveSpeed * horizontalInput;

        if(Input.GetButtonDown("Jump") && physicsController.Info.isBelow)
        {
            velocity.y = maxJumpVelocity;
        }
        if(Input.GetButtonUp("Jump"))
        {
            if (velocity.y > minJumpVelocity)
                velocity.y = minJumpVelocity;
        }
        if(Input.GetAxis("Vertical") < 0f)
        {
            physicsController.FallThroughPlatform();
        }

        HandleInput();
    }

    void FixedUpdate()
    {
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetMoveSpeed, ref moveSmoothing, (physicsController.Info.isBelow) ? groundAcceleration : airAcceleration); //Accelerate with smoothing according to if the character is grounded or not 

        if(horizontalInput != 0 && Mathf.Abs(velocity.x) <= moveSpeed && state != playerState.sliding)
        {
            if(Mathf.Sign(velocity.x) != Mathf.Sign(horizontalInput))
            {
                velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, (physicsController.Info.isBelow) ? groundAcceleration*2 : airAcceleration*2);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, (physicsController.Info.isBelow) ? groundAcceleration : airAcceleration);
            }
        }
        else
        {
            if(state != playerState.sliding)
                velocity.x = Mathf.MoveTowards(velocity.x, 0, (physicsController.Info.isBelow) ? groundDecleration : airDeceleration);
            else
                velocity.x = Mathf.MoveTowards(velocity.x, 0, (physicsController.Info.isBelow) ? slideDeceleration : airDeceleration);
        }
        
        velocity.y += gravity * Time.fixedDeltaTime;
        physicsController.Move(velocity * Time.fixedDeltaTime, false);

        if (physicsController.Info.isBelow || physicsController.Info.isAbove)
        {
            velocity.y = 0;
        }
        if (physicsController.Info.isLeft || physicsController.Info.isRight)
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
        if (Input.GetButtonDown("Slide") && physicsController.Info.isBelow && Mathf.Abs(velocity.x) > 1) //Slide
        {
            state = playerState.sliding;
        }
        if(state == playerState.sliding && (Input.GetButtonUp("Slide") || !physicsController.Info.isBelow))
        {
            state = playerState.neutral;
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            respawn.Respawn();
        }
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Sets position to respawn point and velocity to 0
    {
        transform.position = respawnPos;
        velocity = Vector2.zero;
    }
}
