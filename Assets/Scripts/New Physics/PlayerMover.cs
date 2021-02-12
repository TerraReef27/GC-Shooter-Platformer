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

    private float gravity;
    private float minJumpVelocity;
    private float maxJumpVelocity;
    private float moveSmoothing;

    private PhysicsController physicsController;
    private Vector3 velocity;
    
    private float horizontalInput;
    private float targetMoveSpeed;

    void Awake()
    {
        physicsController = GetComponent<PhysicsController>();
    }

    void Start()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(jumpTime, 2); //Based off of the equation deltaMove = initialVelocity * time + (acceleration * time^2)/2
        maxJumpVelocity = Mathf.Abs(gravity) * jumpTime; //Based off of the equation finalVelocity = initialVelocity + acceleration * time
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity + minJumpHeight)); //based off the euqation minJumpForce = sqrt(2 * gravity * minJumpHeight)
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
    }

    void FixedUpdate()
    {
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetMoveSpeed, ref moveSmoothing, (physicsController.Info.isBelow) ? groundAcceleration : airAcceleration); //Accelerate with smoothing according to if the character is grounded or not 

        if(horizontalInput != 0 && Mathf.Abs(velocity.x) <= moveSpeed)
        {
            if(Mathf.Sign(velocity.x) != Mathf.Sign(horizontalInput))
            {
                velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, (physicsController.Info.isBelow) ? groundAcceleration*2 : airAcceleration*2);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, (physicsController.Info.isBelow) ? groundAcceleration : airAcceleration);
            }
            /*
            if (physicsController.Info.isBelow)
            {
                if (Mathf.Sign(velocity.x) != Mathf.Sign(horizontalInput))
                    velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, (groundDecleration + groundAcceleration) * Time.fixedDeltaTime);
                else
                    velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, groundDecleration * Time.fixedDeltaTime);
            }
            else
                velocity.x = Mathf.MoveTowards(velocity.x, moveSpeed * horizontalInput, airAcceleration * Time.fixedDeltaTime);
            */
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, (physicsController.Info.isBelow) ? groundDecleration : airDeceleration);
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
}
