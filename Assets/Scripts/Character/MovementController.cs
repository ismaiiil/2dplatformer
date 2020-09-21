using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    private Rigidbody2D rb;
    public float timeScale;
    private bool isVerticalPressed;
    private bool facingRight = true;
    private Animator animator;
    private Animation animation;
    

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        animation = gameObject.GetComponent<Animation>();

    }

    void FixedUpdate()
    {
        Time.timeScale = timeScale; 
        //Movement
        float xaxis = Input.GetAxisRaw("Horizontal") * moveSpeed;
        rb.velocity = new Vector2(xaxis, rb.velocity.y);

        if (rb.velocity.x != 0)
        {
            animator.SetBool("isRunning",true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (xaxis > 0 && !facingRight)
        {
            Flip();
        }
        else if ( xaxis < 0 && facingRight) {
            Flip();
        }

        if (Input.GetButtonDown("Jump")) {

            if (rb.velocity.y == 0 && !isVerticalPressed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                animator.SetBool("isJumping", true);
                animation.Play("jump");
            }
            
            isVerticalPressed = true;
        }
        else
        {
            isVerticalPressed = false;
        }

        if (rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }

        if (rb.velocity.y == 0)
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
        }

    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
