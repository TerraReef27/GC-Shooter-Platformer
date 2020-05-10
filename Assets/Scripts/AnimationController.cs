using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator = null; //Reference to animator
    //private PlayerMovement playerMove = null; //Reference to PlayerMover script
    private PlayerController player;
    private SpriteRenderer spriteRend = null; //Reference to the sprite renderer

    private int facing = 1;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        //playerMove = FindObjectOfType<PlayerMovement>();
        player = GetComponent<PlayerController>();
    }


    void Update()
    {
        animator.SetFloat("horizontalMove", player.animationXMovement); //Set the X and Y values in the animator to be the movement values
        //animator.SetFloat("verticalMove", player.Movement.y);

        if (player.animationXMovement != 0) //Set the direction the sprite is facing to the last player input in the X direction
        {
            facing = Mathf.FloorToInt(player.animationXMovement);
            if (facing > 0)
                spriteRend.flipX = false;
            else
                spriteRend.flipX = true;
        }
    }
    
}
