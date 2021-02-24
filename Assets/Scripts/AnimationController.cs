using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator = null; //Reference to animator
    //private PlayerMovement playerMove = null; //Reference to PlayerMover script
    //private PlayerController player;
    private PlayerMover playerMove = null;
    private SpriteRenderer spriteRend = null; //Reference to the sprite renderer

    private int facing = 1;

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
        animator.SetFloat("horizontalMove", playerMove.Velocity.x); //Set the X and Y values in the animator to be the movement values
        //animator.SetFloat("verticalMove", player.Movement.y);

        if (playerMove.Velocity.x != 0) //Set the direction the sprite is facing to the last player input in the X direction
        {
            facing = Mathf.FloorToInt(playerMove.Velocity.x);
            if (facing > 0)
                spriteRend.flipX = false;
            else
                spriteRend.flipX = true;
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
