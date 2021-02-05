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
    private List<ObjectData> objectList;
    private Dictionary<Transform, PhysicsController> objectDictionary = new Dictionary<Transform, PhysicsController>();

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

    void FixedUpdate()
    {
        UpdateRayOrigin();
        
        Vector3 velocity = move * Time.fixedDeltaTime;
        CalculateObjectMovement(velocity);

        MoveObjects(true);
        transform.Translate(velocity);
        MoveObjects(false);
        /*
        float distanceTraveled = (Time.time - startTime) * speed;
        float currentMovement = distanceTraveled / totalDistance;
        transform.position = Vector3.Lerp(moveFrom, moveTo, currentMovement);
        */
    }

    struct ObjectData
    {
        public Transform transform;
        public Vector3 velocity;
        public bool isMovedBefore;
        public bool onPlatform;

        public ObjectData(Transform pTransform, Vector3 pVelocity, bool pMoveBefore, bool pOnPlatform)
        {
            transform = pTransform;
            velocity = pVelocity;
            isMovedBefore = pMoveBefore;
            onPlatform = pOnPlatform;
        }
    }

    private void MoveObjects(bool isBeforePlatformMove)
    {
        foreach(ObjectData obj in objectList)
        {
            if(!objectDictionary.ContainsKey(obj.transform))
            {
                objectDictionary.Add(obj.transform, obj.transform.GetComponent<PhysicsController>());
            }

            if (obj.isMovedBefore == isBeforePlatformMove)
            {
                objectDictionary[obj.transform].Move(obj.velocity, obj.onPlatform);
            }
        }
    }

    private void CalculateObjectMovement(Vector3 velocity)
    {
        objectList = new List<ObjectData>();
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

                        //hit.transform.Translate(new Vector3(moveX, moveY));
                        objectList.Add(new ObjectData(hit.transform, new Vector3(moveX, moveY), (directionY) == 1, true));
                    }
                }
            }
        }

        if (velocity.x != 0) //Push object horizontally
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
                        float moveY = -skinSize; //Ensure that the object will still check for downwards collision

                        //hit.transform.Translate(new Vector3(moveX, moveY));
                        objectList.Add(new ObjectData(hit.transform, new Vector3(moveX, moveY), false, true));
                    }
                }
            }
        }

        if (directionY == -1 || velocity.y == 0 && velocity.x != 0) //Adjust objects if platform is moving down or only horizontally
        {
            float rayLength = mainCollider.bounds.size.x;

            Vector2 origin = rayOrigins.topLeft + (Vector2.up * skinSize * 2);
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.right, rayLength, movableMask); //Generate a hit to see if the ray collided with anything on the collisionMask

            Debug.DrawRay(origin, Vector3.right * rayLength, Color.cyan);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit)
                {
                    float moveX = velocity.x;
                    float moveY = velocity.y;

                    //hit.transform.Translate(new Vector3(moveX, moveY));
                    objectList.Add(new ObjectData(hit.transform, new Vector3(moveX, moveY), true, false));
                }
            }
            
        }
    }
}
