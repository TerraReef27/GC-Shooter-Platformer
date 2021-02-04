using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RaycastController
{
    /*
    public float speed = 1f;
    public Vector3 moveFrom, moveTo;
    private float startTime;
    private float totalDistance;
    */
    [SerializeField] LayerMask movableMask;
    [SerializeField] Vector3 move;

    public override void Start()
    {
        base.Start();
        /*
        startTime = Time.time;
        totalDistance = Vector3.Distance(moveFrom, moveTo);
        */
    }

    void Update()
    {

        UpdateRayOrigin();
        
        Vector3 velocity = move * Time.deltaTime;
        MoveObjects(velocity);
        transform.Translate(velocity);

        /*
        float distanceTraveled = (Time.time - startTime) * speed;
        float currentMovement = distanceTraveled / totalDistance;
        transform.position = Vector3.Lerp(moveFrom, moveTo, currentMovement);
        */
    }

    private void MoveObjects(Vector3 velocity)
    {
        HashSet<Transform> movedObjects = new HashSet<Transform>(); //use a hash table to find out which objects have already been adjusted

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        if (velocity.y != 0) //Manage objects when moving vertically
        {
            float rayLength = Mathf.Abs(velocity.y) + skinSize;

            for (int i = 0; i < numVerticalRays; i++)
            {
                Vector2 origin = (directionY == -1) ? rayOrigins.bottomLeft : rayOrigins.topLeft; //Determine which rays to check
                origin += Vector2.right * (verticalRaySpace * i); //Check the rays for the projected movement
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * directionY, rayLength, movableMask); //Generate a hit to see if the ray collided with anything on the collisionMask
                Debug.DrawRay(origin, Vector3.up * rayLength * directionY, Color.red);
                if (hit)
                {
                    if (!movedObjects.Contains(hit.transform))
                    {
                        movedObjects.Add(hit.transform);
                        float moveX = (directionY == 1) ? velocity.x : 0;
                        float moveY = velocity.y - (hit.distance - skinSize) * directionY;

                        hit.transform.Translate(new Vector3(moveX, moveY));
                    }
                }
            }
        }

        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinSize;

            for (int i = 0; i < numHorizontalRays; i++)
            {
                Vector2 origin = (directionX == -1) ? rayOrigins.bottomLeft : rayOrigins.bottomRight; //Determine which rays to check
                origin += Vector2.up * (horizontalRaySpace * i); //Check the rays for the projected movement
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * directionX, rayLength, movableMask); //Generate a hit to see if the ray collided with anything on the collisionMask

                Debug.DrawRay(origin, directionX * Vector2.right * rayLength, Color.red);

                if (hit)
                {
                    if (!movedObjects.Contains(hit.transform))
                    {
                        movedObjects.Add(hit.transform);
                        float moveX = velocity.x - (hit.distance - skinSize) * directionX;
                        float moveY = 0;

                        hit.transform.Translate(new Vector3(moveX, moveY));
                    }
                }
            }
        }
    }
}
