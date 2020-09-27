using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementController : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed;
    public float jumpHeight;
    private Rigidbody2D rb;
    public float timeScale;
    public GameObject AttackUp;
    public GameObject AttackForward;
    public GameObject Attackdown;
    public int framerate;
    public float kickbackIntensity;
    public int kickbackDelay;
    public float fallSpeedThreshold;



    private bool isVerticalPressed;
    private bool facingRight = true;
    private bool JumpPressed;
    private bool JumpReleased;
    private float xaxis;
    private bool kickbacked;
    private int _kickbackDelay;
    

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = framerate;
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        _kickbackDelay = kickbackDelay;

    }

    private void Update()
    {
        
        xaxis = Input.GetAxisRaw("Horizontal") * moveSpeed;

        //animation handling for running
        if (xaxis != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (Input.GetButtonDown("Jump") && (!animator.GetBool("isFalling") && !animator.GetBool("isJumping")))
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", true);
            JumpPressed = true;
        }
        //Check if character is jumping and stop
        if (Input.GetButtonUp("Jump") && animator.GetBool("isJumping"))
        {
            JumpReleased = true;
        }
        //flip player based on running direction
        if (xaxis > 0 && !facingRight)
        {
            Flip();

        }
        else if (xaxis < 0 && facingRight)
        {
            Flip();
        }
        
        //handle fire button
        if (Input.GetButtonDown("Fire1"))
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                animator.SetTrigger("AttackUpTrigger");

            }
            else if (Input.GetAxisRaw("Vertical") < 0 && (animator.GetBool("isFalling") || animator.GetBool("isJumping")))
            {
                animator.SetTrigger("AttackDownTrigger");
            }
            else
            {
                animator.SetTrigger("AttackTrigger");
            }
        }
    }

    void FixedUpdate()
    {
        
        //TODO REMOVE testing animations
        Time.timeScale = timeScale;
        //Apply movespeed to player when move button is pressed

        if (!kickbacked)
        {
            rb.velocity = new Vector2(xaxis, rb.velocity.y);
        }
        else {
            kickbackDelay -= 1;
            if (kickbackDelay == 0)
            {
                kickbacked = false;
                kickbackDelay = _kickbackDelay;
            }
        }
        

        if (JumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            JumpPressed = false;
        }

        if (JumpReleased)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            JumpReleased = false;
        }
        
        //falling y is negative
        if (rb.velocity.y < fallSpeedThreshold)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }

        if (rb.velocity.y == 0)
        {
            animator.SetBool("isFalling", false);            
        }

        
    }
    /*
     *1 right
     *-1 left
     *0 Up
         */
    public void kickbackPlayer(int side)
    {
        if (!kickbacked && side == 1 || side == -1) {
            rb.velocity = new Vector2(side * kickbackIntensity, rb.velocity.y);
            kickbacked = true;
        }
        else if(side ==  0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight/1.5f);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }


    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
