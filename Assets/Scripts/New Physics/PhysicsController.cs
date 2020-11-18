using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BoxCollider2D))]
public class PhysicsController : MonoBehaviour
{
    private float skinSize = 0.01f;
    private int numHorizontalRays = 5;
    private int numVerticalRays = 3;
    
    float horizontalRaySpace, verticalRaySpace;

    struct RayOrigins { public Vector2 topLeft, topRight, bottomLeft, bottomRight; }
    private RayOrigins rayOrigins;
    [SerializeField] LayerMask collisionMask;

    private BoxCollider2D mainCollider = new BoxCollider2D();

    void Awake()
    {
        mainCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        CalculateSpacing();
    }
    
    public void Move(Vector3 moveVelocity)
    {
        UpdateRayOrigin();
        
        if(moveVelocity.x != 0)
            HandleHorizontalCollisions(ref moveVelocity);
        if(moveVelocity.y != 0)
            HandleVerticalCollisions(ref moveVelocity);

        transform.Translate(moveVelocity);
    }

    private void HandleHorizontalCollisions(ref Vector3 moveVelocity)
    {
        int direction = Math.Sign(moveVelocity.x);
        float length = Mathf.Abs(moveVelocity.x) + skinSize;

        for (int i = 0; i < numHorizontalRays; i++)
        {
            Vector2 origin = (direction == -1) ? rayOrigins.bottomLeft : rayOrigins.bottomRight; //Determine which rays to check
            origin += Vector2.up * (horizontalRaySpace * i); //Check the rays for the projected movement
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, length, collisionMask); //Generate a hit to see if the ray collided with anything on the collisionMask

            Debug.DrawRay(origin, direction * Vector2.right * length, Color.green);

            if (hit) //If the ray collides, change the collision so that it stops at the collision point
            {
                moveVelocity.x = (hit.distance - skinSize) * direction;
                length = hit.distance;
            }
        }
    }
    private void HandleVerticalCollisions(ref Vector3 moveVelocity)
    {
        int direction = Math.Sign(moveVelocity.y);
        float length = Mathf.Abs(moveVelocity.y) + skinSize;

        for(int i=0; i<numVerticalRays; i++)
        {
            Vector2 origin = (direction == -1) ? rayOrigins.bottomLeft : rayOrigins.topLeft; //Determine which rays to check
            origin += Vector2.right * (verticalRaySpace * i + moveVelocity.x); //Check the rays for the projected movement
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * direction, length, collisionMask); //Generate a hit to see if the ray collided with anything on the collisionMask

            Debug.DrawRay(origin, direction * Vector2.up * length, Color.green);

            if (hit) //If the ray collides, change the collision so that it stops at the collision point
            {
                moveVelocity.y = (hit.distance - skinSize) * direction;
                length = hit.distance;
            }
        }
    }
    
    private void UpdateRayOrigin()
    {
        Bounds bounds = mainCollider.bounds;
        bounds.Expand(-skinSize*2); //Add the skin to offset the bounds inwards

        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y); //Set the bounds of the collider
        rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private void CalculateSpacing()
    {
        Bounds bounds = mainCollider.bounds;
        bounds.Expand(-skinSize*2); //Add the skin to offset the bounds

        numHorizontalRays = Mathf.Clamp(numHorizontalRays, 2, int.MaxValue); //Set the number of rays. Must be at least two
        numVerticalRays = Mathf.Clamp(numVerticalRays, 2, int.MaxValue);

        horizontalRaySpace = bounds.size.y / (numHorizontalRays - 1); //Get the distance between the individual rays
        verticalRaySpace = bounds.size.x / (numVerticalRays - 1);
    }
}
