using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnightAi : MonoBehaviour
{

    public float walkingSpeed;
    public float patrolDuration;
    public float standByDuration;
    public float aggroDistance;
    public float rayOffset;
    public GameObject Character;

    private Rigidbody2D rb;
    private float _patrolDuration;
    private float _standByDuration;
    private bool facingRight = true;
    private Animator animator;
    private RaycastHit2D rightLedge;
    private RaycastHit2D leftLedge;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        _patrolDuration = patrolDuration;
        _standByDuration = standByDuration;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {


        if (rb.velocity.x != 0)
        {
            if (facingRight)
            {
                rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayOffset, transform.position.y), Vector2.down, 3f);
                Debug.DrawRay(new Vector2(transform.position.x + rayOffset, transform.position.y), new Vector2(0,-3), Color.red);
                if (rightLedge.collider == null)
                {
                    Flip();
                }                
            }
            else {
                leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayOffset, transform.position.y-2), Vector2.down, 3f);
                Debug.DrawRay(new Vector2(transform.position.x - rayOffset, transform.position.y), new Vector2(0, -3), Color.red);
                if (leftLedge.collider == null)
                {
                    Flip();
                }
            }
        }

        if (patrolDuration > 0)
        {
            rb.velocity = new Vector2(walkingSpeed, rb.velocity.y);
            patrolDuration -= 1;
            animator.SetBool("isWalking", true);
        }
        else
        {
            if (standByDuration > 0)
            {
                standByDuration -= 1;
                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.SetBool("isWalking", false);

            }
            else {
                standByDuration = _standByDuration;
                patrolDuration = _patrolDuration;
                Flip();
                animator.SetBool("isWalking", true);
            }            
        }




    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, aggroDistance);
    }

    void Flip()
    {
        facingRight = !facingRight;
        walkingSpeed = -walkingSpeed;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
