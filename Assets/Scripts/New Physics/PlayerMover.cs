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
        velocity.x = Mathf.SmoothDamp(velocity.x, targetMoveSpeed, ref moveSmoothing, (physicsController.Info.isBelow) ? groundAcceleration : airAcceleration); //Accelerate with smoothing according to if the character is grounded or not
        
        velocity.y += gravity * Time.fixedDeltaTime;
        physicsController.Move(velocity * Time.fixedDeltaTime, false);

        if (physicsController.Info.isBelow || physicsController.Info.isAbove)
        {
            velocity.y = 0;
        }
    }
}
