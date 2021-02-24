using System;
using UnityEngine;

public class PhysicsController : RaycastController
{
    #region Variables
    private CollisionInfo info;
    public CollisionInfo Info { get { return info; } }

    [SerializeField] private int maxClimbAngle = 45;
    [SerializeField] private int maxDecendAngle = 80;
    private Vector2 playerInput = Vector2.zero;

    NewCollisionResponses collisionResponses = null;
    #endregion

    public override void Awake()
    {
        base.Awake();
        if (GetComponent<NewCollisionResponses>())
            collisionResponses = GetComponent<NewCollisionResponses>();
        else
            collisionResponses = null;
    }

    public override void Start()
    {
        base.Start();
    }

    public void Move(Vector2 moveVelocity, bool isOnPlatform)
    {
        Move(moveVelocity, isOnPlatform, Vector2.zero);
    }

    public void Move(Vector2 moveVelocity, bool isOnPlatform, Vector2 input)
    {
        playerInput = input;

        info.ResetInfo();
        UpdateRayOrigin();

        if (moveVelocity.y < 0)
            DecendSlope(ref moveVelocity);
        if(moveVelocity.x != 0)
            HandleHorizontalCollisions(ref moveVelocity);
        if(moveVelocity.y != 0)
            HandleVerticalCollisions(ref moveVelocity);

        transform.Translate(moveVelocity);

        if (isOnPlatform)
            info.isBelow = true;

    }

    private void HandleHorizontalCollisions(ref Vector2 moveVelocity)
    {
        int direction = Math.Sign(moveVelocity.x);
        float length = Mathf.Abs(moveVelocity.x) + skinSize;

        for (int i = 0; i < numHorizontalRays; i++)
        {
            Vector2 origin = (direction == -1) ? rayOrigins.bottomLeft : rayOrigins.bottomRight; //Determine which rays to check
            origin += Vector2.up * (horizontalRaySpace * i); //Check the rays for the projected movement
            RaycastHit2D solidHit = Physics2D.Raycast(origin, Vector2.right * direction, length, solidColisionMask); //Generate a hit to see if the ray collided with anything on the collisionMask
            RaycastHit2D interactHit = Physics2D.Raycast(origin, Vector2.right * direction, length, interactionMask);
            Debug.DrawRay(origin, direction * Vector2.right * length, Color.green);

            if(interactHit)
            {
                collisionResponses.CheckReaction(interactHit.collider);
            }

            if (solidHit) //If the ray collides, change the collision so that it stops at the collision point
            {
                if (solidHit.distance == 0 || solidHit.collider.tag == "OneWayPlatform") //Make it so objects can move inside of collider
                    continue;

                float angle = Vector2.Angle(solidHit.normal, Vector2.up);
                if (i == 0 && angle <= maxClimbAngle) //Only check slope if its the first raycast and change velocity if on a shallow enough slope
                {
                    float distanceToSlope = 0;
                    if (angle != info.oldSlopeAngle) //Check if walking on a new slope
                    {
                        distanceToSlope = solidHit.distance - skinSize;
                        moveVelocity.x -= distanceToSlope * direction;
                    }
                    ClimbSlope(ref moveVelocity, angle);
                    moveVelocity.x += distanceToSlope * direction; //Add back on removed velocity from moving on slope
                }
                if (!info.isClimbingSlope || angle > maxClimbAngle)
                {
                    moveVelocity.x = (solidHit.distance - skinSize) * direction;
                    length = solidHit.distance;

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
    private void HandleVerticalCollisions(ref Vector2 moveVelocity)
    {
        int direction = Math.Sign(moveVelocity.y);
        float length = Mathf.Abs(moveVelocity.y) + skinSize;

        for(int i=0; i<numVerticalRays; i++)
        {
            Vector2 origin = (direction == -1) ? rayOrigins.bottomLeft : rayOrigins.topLeft; //Determine which rays to check
            origin += Vector2.right * (verticalRaySpace * i + moveVelocity.x); //Check the rays for the projected movement
            RaycastHit2D solidHit = Physics2D.Raycast(origin, Vector2.up * direction, length, solidColisionMask); //Generate a hit to see if the ray collided with anything on the collisionMask
            RaycastHit2D interactHit = Physics2D.Raycast(origin, Vector2.up * direction, length, interactionMask);

            Debug.DrawRay(origin, direction * Vector2.up * length, Color.green);

            if (interactHit)
            {                
                collisionResponses.CheckReaction(interactHit.collider);
            }

            if (solidHit) //If the ray collides, change the collision so that it stops at the collision point
            {
                if (solidHit.collider.tag == "OneWayPlatform")
                {
                    if(moveVelocity.y > 0 || solidHit.distance == 0 || solidHit.collider == info.fallThrough)
                    {
                        if(solidHit.collider == info.fallThrough)
                            Debug.Log("passing through top");
                        if (solidHit.distance == 0)
                            Debug.Log("in collider");
                        if (moveVelocity.y > 0)
                            Debug.Log("passing through bottom");
                        continue;
                    }
                }
                if (solidHit.collider != info.fallThrough)
                {
                    info.fallThrough = null;
                }

                moveVelocity.y = (solidHit.distance - skinSize) * direction;
                length = solidHit.distance;

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
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * directionX, length, solidColisionMask);

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
    
    private void ClimbSlope(ref Vector2 moveVelocity, float angle) //Use basic trig to calculate the new xy values according to our slope angle
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
    private void DecendSlope(ref Vector2 moveVelocity)
    {
        float xDirection = Mathf.Sign(moveVelocity.x);
        Vector2 origin = (xDirection == -1) ? rayOrigins.bottomRight : rayOrigins.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up, Mathf.Infinity, solidColisionMask);
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

    public void FallThroughPlatform()
    {
        for (int i = 0; i < numVerticalRays; i++)
        {
            Vector2 origin = rayOrigins.bottomLeft;
            origin += Vector2.right * (verticalRaySpace * i); //Check the rays for the projected movement
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * -1, skinSize*2, solidColisionMask); //Generate a hit to see if the ray collided with anything on the collisionMask

            if (hit) //If the ray collides, change the collision so that it stops at the collision point
            {
                if (hit.collider.tag == "OneWayPlatform")
                {
                    info.fallThrough = hit.collider;
                }
            }
        }
    }
}
