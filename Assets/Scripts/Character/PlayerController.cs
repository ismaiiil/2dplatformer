using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantValues;


public class PlayerController : MonoBehaviour
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
    public float slowmoDuration;



    private bool isVerticalPressed;
    private bool facingRight = true;
    private bool JumpPressed;
    private bool JumpReleased;
    private float xaxis;
    private bool kickbacked;
    private int _kickbackDelay;
    private bool isSlowTime;
    private float _slowmoDuration;



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
        _slowmoDuration = slowmoDuration;

    }

    private void Update()
    {
        if (isSlowTime)
        {
            Time.timeScale = 0.1f;
            if (slowmoDuration > 0)
            {
                slowmoDuration -= 1;
            }
            else {
                isSlowTime = false;
                Time.timeScale = 1;
                slowmoDuration = _slowmoDuration;
            }

        }
        //Set axis to the direction pressed in the input
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

        //If Jump button is pressed, verifu if not jumping or falling and animate appropriately
        //Bools are often used to transfer input from Update to fixedupdate to avoid glitches
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
            //Directional attack
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
        
        //Apply movespeed to player when move button is pressed
        //While being kickbacked the user cannot input any motion
        if (!kickbacked)
        {
            //Handle Input in Update to move player based on Input
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
        
        //Handle Jump from Input
        if (JumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            JumpPressed = false;
        }

        //Used to make longer presses cause higher jumps
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

        //when velocity is zero Falling anim not played
        if (rb.velocity.y == 0)
        {
            animator.SetBool("isFalling", false);            
        }

        
    }
    /*
     *1 right
     *-1 left
     *0 Up
     * THis method is used to make the player kickback
     * This method is called from outside by the Attack Collision Trigger boxes
         */
    public void kickbackPlayer(int side)
    {
        //Based on the side from which the player is relative to the source, kickback is triggered
        if (!kickbacked && side == 1 || side == -1) {
            rb.velocity = new Vector2(side * kickbackIntensity, rb.velocity.y);
            kickbacked = true;
        }
        else if(side ==  0)
        {
            //if kickback comes from the bottom, the player is sent into the air by just a little bit
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight/1.5f);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();


        if (CollisionTag != null)
        {
            if (CollisionTag.HasTag(TAG_ENEMY))
            {
                Debug.Log("Enemy collided");
                isSlowTime = true;

                if (transform.position.x < collision.transform.position.x)
                {
                    kickbackPlayer(-1);
                }
                else{
                    kickbackPlayer(1);
                }
                
            }
        }
    }

    //Flip player rendering state and postion
    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
