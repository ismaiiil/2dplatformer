using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public float kickbackIntensity;
    public int kickbackDelay;
    public float fallSpeedThreshold;
    public float slowmoDuration;
    public GameObject SpawnPoint;
    public int lives = 7;
    public bool DisableInput;
    public FadeManager fadeManager;
    public CinemachineCameraShaker cameraShaker;
    public float shakingDuration = 1.0f;

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
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        _kickbackDelay = kickbackDelay;
        _slowmoDuration = slowmoDuration;
        fadeManager = GameObject.Find("FadeManager").GetComponent<FadeManager>();
        Physics2D.IgnoreLayerCollision(LAY_ENEMY, LAY_PLAYER, false);

    }

    private void Update()
    {

        if (DisableInput)
        {
            Time.timeScale = 1;
            return;
        }
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
        if (DisableInput)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
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
        else if (side == 0)
        {
            //if kickback comes from the bottom, the player is sent into the air by just a little bit
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight / 1.5f);
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
                lives -= 1;
                isSlowTime = true;
                animator.SetBool("isInvulnerable", true);
                if (transform.position.x < collision.transform.position.x)
                {
                    kickbackPlayer(-1);
                }
                else {
                    kickbackPlayer(1);
                }

            }

            if (CollisionTag.HasTag(TAG_SPIKED))
            {
                Debug.Log("spike collided");
                lives -= 1;
                cameraShaker.ShakeCamera(shakingDuration);
                //TODO play special pop animation and slow time
                animator.SetTrigger("TriggerSpike");
            }

            if (lives == 0)
            {
                //play death animation
                cameraShaker.ShakeCamera(shakingDuration);
                animator.SetTrigger("TriggerDeath");
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();
        if (CollisionTag != null)
        {
            if (CollisionTag.HasTag(TAG_CHECKPOINT))
            {
                SpawnPoint = collision.gameObject;
            }
        }

    }

    private void setPlayerInvulnerable() {
        Physics2D.IgnoreLayerCollision(LAY_ENEMY, LAY_PLAYER, true);

    }

    private void setPlayerVulnerable()
    {
        Physics2D.IgnoreLayerCollision(LAY_ENEMY, LAY_PLAYER, false);
        animator.SetBool("isInvulnerable", false);
    }

    private void MoveToCheckpoint() {
        UnFreeze();
        DisableInput = false;
        transform.position = SpawnPoint.transform.position;
    }

    private void FreezePostion() {

        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    private void UnFreeze() {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FadeToBlack() {
        fadeManager.fadeToBlack = true;
    }

    private void FadeToTrans() {
        fadeManager.fadeToTrans = true;
    }
    //Flip player rendering state and postion
    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
