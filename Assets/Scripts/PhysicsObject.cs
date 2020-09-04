using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    protected Vector2 velocity;
    [Tooltip("How much more should gravity be applied to this over the default gravity amount")]
    [SerializeField] private float gravityMultiplyer = 1f;
    [Tooltip("How much faster or slower should object decelerate on ground")]
    [SerializeField] protected float frictionMultiplier = 1f;

    protected bool grounded;
    public bool Grounded { get { return grounded; } private set { grounded = value; } }
    private float minGroundNormalY = .75f; //The min amount of slope until considered too steep
    private Vector2 groundNormal;
    protected Vector2 targetVelocity; //Where we want the object to move
    private const float minMoveDist = .001f; //If the object does not move farther than this, do not check collisions (Prevents collision checks when standing still)
    private const float shellRadius = .01f; //Radius that is used for when a collision happens to check in front of itself
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hits = new RaycastHit2D[16]; //Array to store collisions
    private List<RaycastHit2D> hitsList = new List<RaycastHit2D>(16); //List to act off of collisions
    private Rigidbody2D rb; //Rigidbody2D to use in order to change position. Must be kinematic

    private RespawnSystem respawn = null; //Reference to the respawn system in the scene

    private CollisionResponses collisionResponses = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        respawn = FindObjectOfType<RespawnSystem>();
        respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn; //Subscribe object to the OnPlayerRespawn event
        collisionResponses = GetComponent<CollisionResponses>();
    }

    void Start()
    {
        contactFilter.useTriggers = false; //Dissallow object to respond to trigger set colliders
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer)); //Set up the collisions to follow the Unity collision matrix
        contactFilter.useLayerMask = true; //Enable the use of layer masks
    }

    void Update()
    {
        ComputeVelocity();
        velocity += targetVelocity;
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Sets position to respawn point and velocity to 0
    {
        transform.position = respawnPos;
        velocity = Vector2.zero;
    }

    protected virtual void ComputeVelocity()    
    { }

    void FixedUpdate()
    {
        //velocity += targetVelocity;

        velocity += gravityMultiplyer * Physics2D.gravity * Time.fixedDeltaTime; //Apply gravity to the object

        if (grounded) //Apply friction to the object
            velocity.x *= frictionMultiplier;

        grounded = false; //Make grounded false until proven otherwise

        Vector2 futureMovement = velocity * Time.fixedDeltaTime; //Get the vector that the gameobject will move towards in the next frame

        Vector2 moveOnGround = new Vector2(groundNormal.y, -groundNormal.x); //Get the normal on ground to check for how to move if sloped
        Vector2 moveTo = moveOnGround * futureMovement.x; //Find the future X positon of the object

        Move(moveTo, false); //Move for x direction

        moveTo = Vector2.up * futureMovement.y; //Find the future Y positon of the object

        Move(moveTo, true); //Move for y direction
    }

    private void Move(Vector2 moveTo, bool isYMovement)
    {
        float distance = moveTo.magnitude;
        if (distance > minMoveDist)
        {
            int count = rb.Cast(moveTo, contactFilter, hits, distance + shellRadius); //Use Cast() to get a list of RaycastHit2Ds which will be checked for collisions

            hitsList.Clear(); //Clear the list to get rid of old contacts
            for(int i = 0; i < count; i++) //Add each contact to the list
            {
                hitsList.Add(hits[i]);
            }

            for(int i = 0; i < hitsList.Count; i++)
            {
                Vector2 currentNormal = hitsList[i].normal; //Get the normal of the collision
                if (collisionResponses)
                {
                    //Ignore collision if tag is on the ignore list
                    if (!collisionResponses.CheckIfIgnore(hitsList[i].collider.tag))
                    {
                        continue;
                    }
                    //Kill the player if it interacts with a killer tagged collider 
                    else if (!collisionResponses.CheckIfKill(hitsList[i].collider.tag))
                    {
                        respawn.Respawn();
                    }
                }

                if(currentNormal.y > minGroundNormalY) //Check if collision is considered "ground" based off the minGroundNormalY
                {
                    grounded = true;
                    if(isYMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal); //Get the dot product of the velocity and currentNormal
                if(projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDist = hitsList[i].distance - shellRadius;
                distance = modifiedDist < distance ? modifiedDist : distance;
            }
        }
        rb.position += moveTo.normalized * distance; //Move the rigidbody and gameObject to the future position
    }
}
