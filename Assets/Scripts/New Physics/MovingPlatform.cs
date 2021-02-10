using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RaycastController
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float waitTime = .5f;
    [SerializeField] private Vector3[] localPoints;
    [SerializeField] private float lineSize = 1;
    [Range(0, 4)]
    [SerializeField] private float smoothingValue = 0f;

    private Vector3 moveFrom, moveTo;
    private float startTime;
    private float totalDistance;
    private Vector3[] globalPoints;
    private int currentPathLocation = 0;
    private float progressToNextPoint = 0;
    private float currentWaitTime;

    private List<ObjectData> objectList;
    private Dictionary<Transform, PhysicsController> objectDictionary = new Dictionary<Transform, PhysicsController>();

    [SerializeField] LayerMask movableMask;

    //[SerializeField] Vector3 move;

    public override void Start()
    {
        base.Start();

        globalPoints = new Vector3[localPoints.Length];
        for(int i=0; i<globalPoints.Length; i++)
        {
            globalPoints[i] = localPoints[i] + transform.position;
        }

        startTime = Time.time;
        totalDistance = Vector3.Distance(moveFrom, moveTo);
    }

    void FixedUpdate()
    {
        UpdateRayOrigin();
        
        Vector3 velocity = CalculateBetweenAlongPoints();
        CalculateObjectMovement(velocity);

        MoveObjects(true);
        transform.Translate(velocity);
        MoveObjects(false);
    }

    private Vector3 CalculateBetweenAlongPoints()
    {
        if (Time.time < currentWaitTime)
            return Vector3.zero;

        float distanceToBetweenPath = Vector3.Distance(globalPoints[currentPathLocation], globalPoints[currentPathLocation + 1]);
        progressToNextPoint += Time.fixedDeltaTime * speed / distanceToBetweenPath;
        progressToNextPoint = Mathf.Clamp01(progressToNextPoint);
        float smoothedProgressToNextPoint = Smooth(progressToNextPoint);

        Vector3 platformPos = Vector3.Lerp(globalPoints[currentPathLocation], globalPoints[currentPathLocation + 1], smoothedProgressToNextPoint);

        if(progressToNextPoint >= 1)
        {
            currentPathLocation++;
            progressToNextPoint = 0;

            if(currentPathLocation >= globalPoints.Length - 1)
            {
                currentPathLocation = 0;
                System.Array.Reverse(globalPoints);
            }
            currentWaitTime = Time.time + waitTime;
        }

        return platformPos - transform.position;
    }

    private float Smooth(float x)
    {
        float a = smoothingValue + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
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
                        Debug.Log("Object is on ground (Vertical)");
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
                        objectList.Add(new ObjectData(hit.transform, new Vector3(moveX, moveY), true, false));
                    }
                }
            }
        }

        if (directionY == -1 || velocity.y == 0 && velocity.x != 0) //Adjust objects if platform is moving down or only horizontally
        {
            float rayLength = mainCollider.bounds.size.x;

            Vector2 origin = rayOrigins.topLeft + (Vector2.up * skinSize);
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.right, rayLength, movableMask); //Generate a hit to see if the ray collided with anything on the collisionMask

            Debug.DrawRay(origin, Vector3.right * rayLength, Color.cyan);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit)
                {
                    Debug.Log("Object is on ground");
                    float moveX = velocity.x;
                    float moveY = velocity.y;

                    //hit.transform.Translate(new Vector3(moveX, moveY));
                    objectList.Add(new ObjectData(hit.transform, new Vector3(moveX, moveY), false, true));
                }
            }
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

    private void OnDrawGizmos()
    {
        if(localPoints != null)
        {
            Gizmos.color = Color.green;
            for(int i=0; i<localPoints.Length; i++)
            {
                Vector3 pointPos = (Application.isPlaying) ? globalPoints[i] : localPoints[i] + transform.position;
                Gizmos.DrawLine(pointPos - Vector3.up * lineSize, pointPos + Vector3.up * lineSize);
                Gizmos.DrawLine(pointPos - Vector3.left * lineSize, pointPos + Vector3.left * lineSize);
            }
        }
    }
}
