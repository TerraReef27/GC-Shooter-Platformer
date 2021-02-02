using System;
using UnityEngine;

[RequireComponent(typeof (BoxCollider2D))]
public class PhysicsController : MonoBehaviour
{
    #region Variables
    private float skinSize = 0.0125f;
    private int numHorizontalRays = 5;
    private int numVerticalRays = 3;
    
    float horizontalRaySpace, verticalRaySpace;

    struct RayOrigins { public Vector2 topLeft, topRight, bottomLeft, bottomRight; }
    private RayOrigins rayOrigins;
    public struct CollisionInfo
    {
        public bool isAbove, isBelow, isLeft, isRight;
        public bool isClimbingSlope, isDecendingSlope;
        public float slopeAngle, oldSlopeAngle;

        public void ResetInfo()
        {
            isAbove = isBelow = isLeft = isRight = false;
            isClimbingSlope = isDecendingSlope = false;
            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }
    }
    private CollisionInfo info;
    public CollisionInfo Info { get { return info; } }
    [SerializeField] LayerMask collisionMask;

    [SerializeField] private int maxClimbAngle = 45;
    [SerializeField] private int maxDecendAngle = 80;

    private BoxCollider2D mainCollider = new BoxCollider2D();
    #endregion

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
        info.ResetInfo();
        UpdateRayOrigin();

        if (moveVelocity.y < 0)
            DecendSlope(ref moveVelocity);
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
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && angle <= maxClimbAngle) //Only check slope if its the first raycast and change velocity if on a shallow enough slope
                {
                    float distanceToSlope = 0;
                    if (angle != info.oldSlopeAngle) //Check if walking on a new slope
                    {
                        distanceToSlope = hit.distance - skinSize;
                        moveVelocity.x -= distanceToSlope * direction;
                    }
                    ClimbSlope(ref moveVelocity, angle);
                    moveVelocity.x += distanceToSlope * direction; //Add back on removed velocity from moving on slope
                }
                if (!info.isClimbingSlope || angle > maxClimbAngle)
                {
                    moveVelocity.x = (hit.distance - skinSize) * direction;
                    length = hit.distance;

                    if(info.isClimbingSlope)
                    {
                        moveVelocity.y = Mathf.Tan(info.slopeAngle * Mathf.Rad2Deg) * moveVelocity.x; //Use unchanged angle to adjust y value
                    }

                    info.isRight = direction == 1;
                    info.isLeft = direction == -1;
                }
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

                if (info.isClimbingSlope)
                    moveVelocity.x = moveVelocity.y / Mathf.Tan(info.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveVelocity.x);

                info.isAbove = direction == 1;
                info.isBelow = direction == -1;
            }
        }

        if(info.isClimbingSlope) //Check to see if there is a change to a new slope. If there is adjust the velocity accordingly
        {
            float directionX = Mathf.Sign(moveVelocity.x);
            length = Mathf.Abs(moveVelocity.x) + skinSize;
            Vector2 origin = ((directionX == -1) ? rayOrigins.bottomLeft : rayOrigins.bottomRight) + Vector2.up * moveVelocity.y;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * directionX, length, collisionMask);

            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != info.slopeAngle)
                {
                    moveVelocity.x = (hit.distance - skinSize) * directionX;
                    info.slopeAngle = slopeAngle;
                }
            }
        }
    }
    
    private void ClimbSlope(ref Vector3 moveVelocity, float angle) //Use basic trig to calculate the new xy values according to our slope angle
    {
        float moveDist = Math.Abs(moveVelocity.x);
        float climbY = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDist;

        if (moveVelocity.y <= climbY) //Check if jumping so that the y is not changed
        {
            moveVelocity.y = climbY;
            moveVelocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(moveVelocity.x);
            info.isBelow = true;
            info.isClimbingSlope = true;
            info.slopeAngle = angle;
        }
    }
    
    private void DecendSlope(ref Vector3 moveVelocity)
    {
        float xDirection = Mathf.Sign(moveVelocity.x);
        Vector2 origin = (xDirection == -1) ? rayOrigins.bottomRight : rayOrigins.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up, Mathf.Infinity, collisionMask);
        if(hit) 
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxDecendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == xDirection)
                {
                    if (hit.distance - skinSize <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveVelocity.x)) //Check if we will be close enough to the slope to move down it
                    {
                        float moveDistance = Mathf.Abs(moveVelocity.x);
                        float decendY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveVelocity.x);
                        moveVelocity.y -= decendY;

                        info.slopeAngle = slopeAngle;
                        info.isDecendingSlope = true;
                        info.isBelow = true;

                        Debug.Log("Decending");
                    }
                }
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
