using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    private Rigidbody2D rb;
    public float timeScale;
    public CinemachineVirtualCamera cinemachine;
    public float XOffset;
    public GameObject AttackUp;
    public GameObject AttackForward;
    public GameObject Attackdown;
    public float AttackDelay;
    public int framerate;

    private bool isVerticalPressed;
    private bool facingRight = true;
    private Animator animator;
    private Animation animation;
    private CinemachineFramingTransposer cameraComp;
    private bool JumpPressed;
    private float xaxis;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = framerate;
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        animation = gameObject.GetComponent<Animation>();
        cameraComp = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();

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
        
        rb.velocity = new Vector2(xaxis, rb.velocity.y);
        if (JumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            JumpPressed = false;
        }

        //falling y is negative
        if (rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }

        if (rb.velocity.y == 0)
        {
            animator.SetBool("isFalling", false);            
        }


    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        cameraComp.m_TrackedObjectOffset.x = -XOffset;
        XOffset = -XOffset;
    }
}
