using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpTime = 0.3f;
    [SerializeField] private float moveSpeed = 5f;

    private float gravity;
    private float jumpVelocity;

    private PhysicsController physicsController;
    private Vector3 velocity;
    
    private float horizontalInput;

    void Awake()
    {
        physicsController = GetComponent<PhysicsController>();
    }

    void Start()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(jumpTime, 2); //Based off of the equation deltaMove = initialVelocity * time + (acceleration * time^2)/2
        jumpVelocity = Mathf.Abs(gravity) * jumpTime; //Based off of the equation finalVelocity = initialVelocity + acceleration * time
    }

    void Update()
    {
        if(physicsController.Info.isBelow || physicsController.Info.isAbove)
        {
            velocity.y = 0;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            Debug.Log(physicsController.Info);
            velocity.x += moveSpeed * horizontalInput * Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && physicsController.Info.isBelow)
        {
            Debug.Log("Jumping");
            velocity.y = jumpVelocity;
        }

        velocity.y += gravity * Time.deltaTime;
        physicsController.Move(velocity * Time.deltaTime);
    }
}
