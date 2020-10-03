using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstantValues;


public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb;
    [HideInInspector]
    public float timeScale;
    public GameObject AttackUp;
    public GameObject AttackForward;
    public GameObject Attackdown;
    public GameObject SpawnPoint;
    public int lives;
    [HideInInspector]
    public bool DisableInput;
    public FadeManager fadeManager;
    public CinemachineCameraShaker cameraShaker;
    public HeartsHealthVisual healthManager;
    public float SuperAmount;
    public GameObject preChargeParticles;
    

    private float moveSpeed = 15;
    private float jumpHeight = 25;
    private float kickbackIntensity = 40;
    private int kickbackDelay = 3;
    private float fallSpeedThreshold = -2.00f;
    private float slowmoDuration = 20;
    private float shakingDuration = 1.0f;
    private float currentSpecialAmount;
    public float MaxSuperAMount = 100;
    private float HitSuperAmount = 10;
    private float MaxChargeThreshold = 40;
    private float SuperHealthCost = 20;
    private float ChargeRate = 0.4f;
    

    private bool isVerticalPressed;
    private bool facingRight = true;
    private bool JumpPressed;
    private bool JumpReleased;
    private float xaxis;
    private bool kickbacked;
    private int _kickbackDelay;
    private bool isSlowTime;
    private float _slowmoDuration;
    private bool IsVulnerable;
    private bool isDead;
    private bool IsChangingSpecial;
    [SerializeField]
    private float ChargeThreshold;
    private float superBuffer;
    private bool IsIdle;

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
        IsVulnerable = true;
        healthManager.drawLives(lives);
        SuperAmount = 0f;

    }

    private void Update()
    {
        SuperAmount = Mathf.Clamp(SuperAmount, 0, MaxSuperAMount);
        if (isDead && !animator.GetCurrentAnimatorStateInfo(0).IsName("die"))
        {
            //play death animation
            cameraShaker.ShakeCamera(shakingDuration);
            animator.SetTrigger("TriggerDeath");
            return;
        }
        if (DisableInput)
        {
            Time.timeScale = 1;
            return;
        }


        if (SuperAmount > 0)
        {
            if ( IsIdle && Input.GetButton("Charge"))
            {
                ChargeThreshold += 1;
                preChargeParticles.SetActive(true);
            }
            if (ChargeThreshold >= MaxChargeThreshold)
            {
                preChargeParticles.SetActive(false);
                IsChangingSpecial = true;
                animator.SetBool("isCharging", true);
                if (IsChangingSpecial)
                {
                    SuperAmount -= ChargeRate;
                    superBuffer += ChargeRate;

                    if (superBuffer >= SuperHealthCost)
                    {
                        //Add life after we have spent enough super (SuperHealthCost)
                        healthManager.Addlife(1);
                        //Reset buffer
                        superBuffer = 0;
                    }

                }
            }
        }

        if (Input.GetButtonUp("Charge") || (SuperAmount <= 0 && superBuffer <=0 ) || !IsIdle)
        {
            preChargeParticles.SetActive(false);
            if (superBuffer != 0)
            {
                SuperAmount += superBuffer;
                superBuffer = 0;
            }
            ChargeThreshold = 0;
            animator.SetBool("isCharging", false);
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

    private bool IsAnimating(string animation)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

    void FixedUpdate()
    {

        if (DisableInput)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if ((rb.velocity.y > 0 ||  rb.velocity.x > 0) || (rb.velocity.y < fallSpeedThreshold || rb.velocity.x < 0))
        {
            IsIdle = false;
        }
        else { IsIdle = true; }
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

    public void increaseSuper() {
        SuperAmount += HitSuperAmount;
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
        if (!IsVulnerable)
        {
            return;
        }
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();


        if (CollisionTag != null)
        {
            if (CollisionTag.HasTag(TAG_ENEMY))
            {
                healthManager.RemoveLife(1.0f);
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
                healthManager.RemoveLife(0.5f);
                cameraShaker.ShakeCamera(shakingDuration);
                //TODO play special pop animation and slow time
                animator.SetTrigger("TriggerSpike");
            }

            if (healthManager.CheckIsDead())
            {
                if (isDead)
                {
                    return;
                }
                isDead = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsVulnerable)
        {
            return;
        }
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();
        if (CollisionTag != null)
        {
            if (CollisionTag.HasTag(TAG_CHECKPOINT))
            {
                SpawnPoint = collision.gameObject;
            }

            if (CollisionTag.HasTag(TAG_ENEMY_ATTACK))
            {
                healthManager.RemoveLife(1.0f);
                isSlowTime = true;
                animator.SetBool("isInvulnerable", true);
                if (transform.position.x < collision.transform.parent.transform.position.x)
                {
                    kickbackPlayer(-1);
                }
                else
                {
                    kickbackPlayer(1);
                }

            }
            if (healthManager.CheckIsDead())
            {
                if (isDead)
                {
                    return;
                }
                isDead = true;
            }
        }

    }

    private void setPlayerInvulnerable() {
        Physics2D.IgnoreLayerCollision(LAY_ENEMY, LAY_PLAYER, true);
        IsVulnerable = false;

    }

    private void setPlayerVulnerable()
    {
        Physics2D.IgnoreLayerCollision(LAY_ENEMY, LAY_PLAYER, false);
        animator.SetBool("isInvulnerable", false);
        IsVulnerable = true;
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
