using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstantValues;

public class EnemyKnightAi : MonoBehaviour
{

    public float walkingSpeed;
    public float patrolDuration;
    public float standByDuration;
    public float aggroDistance;
    public float attackRange;
    public float rayOffset;
    public GameObject Character;
    public float kickbackIntensity;
    public int kickbackDelay;
    public int lifes = 7;
    public GameObject whiteBlood;

    private Rigidbody2D rb;
    private float _patrolDuration;
    private float _standByDuration;
    private bool facingRight = true;
    private Animator animator;
    private RaycastHit2D rightLedge;
    private RaycastHit2D leftLedge;
    private bool isChasing;
    private float _walkingSpeed;
    private float minAggroDistance = 1.0f;
    private bool kickbacked;
    private int _kickbackDelay;
    private bool disableAI;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        _patrolDuration = patrolDuration;
        _standByDuration = standByDuration;
        _walkingSpeed = walkingSpeed;
        _kickbackDelay = kickbackDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (disableAI == true)
        {
            return;
        }
        //Check if player is in the range (Circle of aggroDistance)
        if ((transform.position.x - aggroDistance < Character.transform.position.x) && (transform.position.x + aggroDistance > Character.transform.position.x) &&
            (transform.position.y - aggroDistance < Character.transform.position.y) && (transform.position.y + aggroDistance > Character.transform.position.y))
        {
            isChasing = true;
        }
        else {
            isChasing = false;
        }
    }

    private void FixedUpdate()
    {
        if (disableAI == true)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        //AI code isnt run while the enemy is kickbacked
        if (kickbacked)
        {
            kickbackDelay -= 1;
            if (kickbackDelay == 0)
            {
                kickbacked = false;
                kickbackDelay = _kickbackDelay;
            }
            return;
        }

        //Idle "bug patrol" mode while there is no player in the aggroDistance
        if (!isChasing)
        {
            if (rb.velocity.x != 0)
            {
                //Check ledge before the turning the enemy back and patrol
                if (facingRight)
                {
                    rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayOffset, transform.position.y), Vector2.down, 3f);
                    Debug.DrawRay(new Vector2(transform.position.x + rayOffset, transform.position.y), new Vector2(0, -3), Color.red);
                    if (rightLedge.collider == null)
                    {
                        patrolDuration = 0;
                    }
                }
                else
                {
                    leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayOffset, transform.position.y - 2), Vector2.down, 3f);
                    Debug.DrawRay(new Vector2(transform.position.x - rayOffset, transform.position.y), new Vector2(0, -3), Color.red);
                    if (leftLedge.collider == null)
                    {
                        patrolDuration = 0;
                    }
                }
            }
            //while the enemy is in patrol mode keep moving
            if (patrolDuration > 0)
            {

                rb.velocity = new Vector2(walkingSpeed, rb.velocity.y);
                patrolDuration -= 1;
                animator.SetBool("isWalking", true);
            }
            else
            {
                //when patrol time is finished, standby mode is triggered and the enemy idles and does nothing
                if (standByDuration > 0)
                {
                    standByDuration -= 1;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animator.SetBool("isWalking", false);

                }
                else
                {
                    //Reset all vars when standby times ends
                    standByDuration = _standByDuration;
                    patrolDuration = _patrolDuration;
                    walkingSpeed = -walkingSpeed;
                    if (walkingSpeed > 0 && !facingRight || walkingSpeed < 0 && facingRight)
                    {
                        Flip();
                    }
                    
                    animator.SetBool("isWalking", true);
                }
            }
        }
        //The enemy is in chase mode and will track the user position
        else {
            //If enemy is on the right side of the player, run right

            
            if (transform.position.x < Character.transform.position.x)
            {
                if (facingRight)
                {
                    rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayOffset, transform.position.y), Vector2.down, 3f);
                    Debug.DrawRay(new Vector2(transform.position.x + rayOffset, transform.position.y), new Vector2(0, -3), Color.red);
                    if (rightLedge.collider == null)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        animator.SetBool("isWalking", false);
                        return;
                    }
                }

                rb.velocity = new Vector2(_walkingSpeed, rb.velocity.y);
                animator.SetBool("isWalking", true);
                if (!facingRight)
                {
                    Flip();
                }
            //if enemy is just under player, enemy stays idle, to avoid glitches on movment, have a min aggro distance, to prevent immediate fliping and glitch
            }else if(transform.position.x < Character.transform.position.x + minAggroDistance && transform.position.x > Character.transform.position.x - minAggroDistance)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.SetBool("isWalking", false);
            }
            //if enemy is on the left side of the player, run left
            else {
                if (!facingRight)
                {
                    leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayOffset, transform.position.y - 2), Vector2.down, 3f);
                    Debug.DrawRay(new Vector2(transform.position.x - rayOffset, transform.position.y), new Vector2(0, -3), Color.red);
                    if (leftLedge.collider == null)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        animator.SetBool("isWalking", false);
                        return;
                    }
                }
                rb.velocity = new Vector2(-_walkingSpeed, rb.velocity.y);
                animator.SetBool("isWalking", true);
                if (facingRight)
                {
                    Flip();
                }
            }
            patrolDuration = 0;
        }       
    }

    //handle triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (disableAI == true)
        {
            return;
        }
        //Get tag of the current trigger that entered the knight collider
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();

        if (CollisionTag != null)
        {
            //TAG_PLAYER_ATK is the collider for the sword attack objects activated on animation
            //kickback enemy if the nemy is hit by the sword
            if (CollisionTag.HasTag(TAG_PLAYER_ATK))
            {
                lifes -= 1;
                animator.SetBool("isHurt", true);
                Instantiate(whiteBlood, transform.position, Quaternion.identity);
                if (lifes == 0)
                {
                    animator.SetTrigger("TriggerDie");
                    disableAI = true;
                    var _tags = GetComponent<CustomTag>();
                    _tags.ClearTags();
                }
                if (transform.position.x > Character.transform.position.x)
                {
                    kickback(1);
                }
                else
                {
                    kickback(-1);
                }
            }
        }
    }

    public void isHurtFalse()
    {
        animator.SetBool("isHurt", false);
    }

    public void enemyDestroy()
    {
        Destroy(gameObject);
    }
    public void TriggerAttack()
    {
        animator.SetTrigger("triggerAttack");
    }

    public void kickback(int side)
    {
        rb.velocity = new Vector2(side * kickbackIntensity, rb.velocity.y);
        kickbacked = true;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, aggroDistance);
    }

    void Flip()
    {
        facingRight = !facingRight;
        //change the walking direction velocity
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
