using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float speed;
    public float nextWaypointDistance = 1f;
    [Tooltip("Is grounded check. Automatic Field.")]
    [SerializeField] private bool isGrounded;
    [Tooltip("Enemy is stacked on another enemy.")]
    [SerializeField] private bool isStacked;
    [SerializeField] private EnemyContainer enemyContainer;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
        Debug.Log("Start");
    }
    void UpdatePath()
    {
        if(seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }
    void OnPathComplete(Path p )
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach (Enemy enemy in enemyContainer.enemyArray)
        {
            if (path == null)
                return;
            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force;
            force = direction * enemy.movementSpeed * Time.deltaTime;
            //Debug.Log("Intial force" + force);
            #region Force Movement Documentation
            /*
             *  0,0,0
             *  0,0,1
             *  0,1,0
             *  0,1,1
             *  1,0,0
             *  1,0,1
             *  1,1,0
             *  1,1,1
             */
            /*             Instances                            Effects
             *#1 Isn't Grounded, Can't Fly, Can't Jump            Gravity
             *#2 Isn't Grounded, Can't Fly, Can Jump              Gravity
             *#3 Isn't Grounded, Can Fly, Can't Jump              force = direction
             *#4 Isn't Grounded, Can Fly, Can Jump                N/A
             *#5 Is Grounded, Can't Fly, Can't Jump               force.y = 0
             *#6 Is Grounded, Can't Fly, Can Jump                 if(direction.y>0){force.y = direction.y}
             *#7 Is Grounded, Can Fly, Can't Jump                 if(direction.y>0){force.y = direction.y}
             *#8 Is Grounded, Can Fly, Can Jump                   N/A
             */
            #endregion

            if (!isGrounded && enemy.canFly)//#3
            {
                force = direction * enemy.movementSpeed * Time.deltaTime;
            }
            else if (isGrounded && !enemy.canFly && !enemy.canJump)//#5
            {
                force.y = 0;
            }
            else if (isGrounded && !enemy.canFly && enemy.canJump)//#6
            {
                if (direction.y > 0)
                    force.y = 20f;
                else
                    force.y = 0;
                //Debug.Log(force);
            }
            else if(isGrounded && enemy.canFly)//#7
            {
                if (direction.y > 0)
                    force.y = direction.y;
                else
                    force.y = 0;
            }

            #region Old Force Editing Code
            //else if (!enemy.canFly && !enemy.isGrounded) //Cant Fly and isn't grounded, Don't move upward (Jump).
            //{
            //    force.y = 0;
            //}
            //else if (!enemy.canFly && enemy.canJump && enemy.isGrounded && direction.y > 0)  //Cant Fly, can jump and is grounded, (Jump)
            //{
            //    force.y = direction.y * enemy.movementSpeed * Time.deltaTime;
            //}
            //else
            //    force = direction * enemy.movementSpeed * Time.deltaTime;
            #endregion
            
            Debug.Log(force.y);
            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
        Vector2 stackedForce = new Vector2(0f, 0f);
        if (isStacked)
        {
            if (Random.Range(0, 1) == 0)
                stackedForce.x = 10;
            else
                stackedForce.x = -10;
        }
        rb.AddForce(stackedForce);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(Collider2D.Equals("BoxCollider2D"))
        isGrounded = true;
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyAI>() && collision.gameObject.GetComponentInChildren<CircleCollider2D>())
            isStacked = true;
        else
            isStacked = false;
    }
}
