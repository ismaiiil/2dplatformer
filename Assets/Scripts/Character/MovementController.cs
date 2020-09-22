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

    private bool isVerticalPressed;
    private bool facingRight = true;
    private Animator animator;
    private Animation animation;
    private CinemachineFramingTransposer cameraComp;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        animation = gameObject.GetComponent<Animation>();
        cameraComp = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();

    }

    void FixedUpdate()
    {
        //TODO REMOVE testing animations
        Time.timeScale = timeScale; 
        //Apply movespeed to player when move button is pressed
        float xaxis = Input.GetAxisRaw("Horizontal") * moveSpeed;
        rb.velocity = new Vector2(xaxis, rb.velocity.y);

        //animation handling for running
        if (rb.velocity.x != 0)
        {
            animator.SetBool("isRunning",true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        //flip player based on running direction
        if (xaxis > 0 && !facingRight)
        {
            Flip();
            
        }
        else if ( xaxis < 0 && facingRight) {
            Flip();
        }

        //handle jump button
        if (Input.GetButtonDown("Jump")
            //|| Input.GetAxisRaw("Vertical") > 0 //TODO this is is used to debug in Teamviewer, REMOVE LATER
            ) {
            if (rb.velocity.y == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", true);
            }

        }



        //falling y is negative
        if (rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }

        //not jumping or falling, static
        if (rb.velocity.y == 0)
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
        }

    }

    private void Update()
    {

        //handle fire button
        if (Input.GetButtonDown("Fire1"))
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                AttackUp.SetActive(true);

            }
            else if (Input.GetAxisRaw("Vertical") < 0 && ( animator.GetBool("isFalling") || animator.GetBool("isJumping")))
            {
                Attackdown.SetActive(true);
            }
            else
            {
                AttackForward.SetActive(true);
            }
        }

        if (AttackForward.activeSelf || AttackUp.activeSelf || Attackdown.activeSelf)
        {
            StartCoroutine(DelayedInactivate(0.10f));
        }
    }

    IEnumerator DelayedInactivate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AttackUp.SetActive(false);
        AttackForward.SetActive(false);
        Attackdown.SetActive(false);
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
