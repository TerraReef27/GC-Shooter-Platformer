using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator = null; //Reference to animator
    private PlayerMovement playerMove = null; //Reference to PlayerMover script
    private SpriteRenderer spriteRend = null; //Reference to the sprite renderer

    private int facing = 1;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        playerMove = FindObjectOfType<PlayerMovement>();
    }


    void Update()
    {
        animator.SetFloat("horizontalMove", playerMove.Movement.x); //Set the X and Y values in the animator to be the movement values
        animator.SetFloat("verticalMove", playerMove.Movement.y);

        if (playerMove.Movement.x != 0) //Set the direction the sprite is facing to the last player input in the X direction
        {
            facing = Mathf.FloorToInt(playerMove.Movement.x);
            if (facing > 0)
                spriteRend.flipX = false;
            else
                spriteRend.flipX = true;
        }
    }
    
}
