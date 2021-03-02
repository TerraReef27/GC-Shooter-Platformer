using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator = null; //Reference to animator
    //private PlayerMovement playerMove = null; //Reference to PlayerMover script
    //private PlayerController player;
    private PlayerMover playerMove = null;
    private SpriteRenderer spriteRend = null; //Reference to the sprite renderer

    private int facing = 1;
    private int wasFacing = 1;
    private bool isGrounded = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        //playerMove = FindObjectOfType<PlayerMovement>();
        //player = GetComponent<PlayerController>();
        playerMove = GetComponent<PlayerMover>();
    }


    void Update()
    {
        isGrounded = playerMove.grounded;

        animator.SetFloat("horizontalMove", playerMove.Velocity.x); //Set the X and Y values in the animator to be the movement values
        animator.SetBool("isGrounded", isGrounded);
        //animator.SetFloat("verticalMove", player.Movement.y);

        if (playerMove.Velocity.x != 0) //Set the direction the sprite is facing to the last player input in the X direction
        {
            facing = Mathf.FloorToInt(playerMove.HorizontalInput);
            Debug.Log(Mathf.FloorToInt(playerMove.HorizontalInput));
            if (facing > 0 && wasFacing < 0)
            {
                wasFacing = 1;
                Vector3 scale = this.transform.localScale;
                this.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                //spriteRend.flipX = false;
            }
            else if (facing < 0 && wasFacing > 0)
            {
                wasFacing = -1;
                Vector3 scale = this.transform.localScale;
                this.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                //spriteRend.flipX = true
            }
        }

        if (playerMove.state == PlayerMover.playerState.sliding)
        {
            animator.SetBool("isSliding", true);
        }
        else
        {
            animator.SetBool("isSliding", false);
        }
    }
    
}
