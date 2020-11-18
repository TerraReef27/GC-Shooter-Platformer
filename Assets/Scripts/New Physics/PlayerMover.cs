using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsController))]
public class PlayerMover : MonoBehaviour
{
    private PhysicsController physicsController;
    private Vector3 velocity;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float moveSpeed = 5f;

    private float horizontalInput;

    void Awake()
    {
        physicsController = GetComponent<PhysicsController>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            velocity.x += moveSpeed * horizontalInput * Time.deltaTime;
        }

        velocity.y += gravity * Time.deltaTime;
        physicsController.Move(velocity * Time.deltaTime);
    }
}
