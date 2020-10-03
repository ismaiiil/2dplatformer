using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantValues;

public class AttacksScript : MonoBehaviour
{

    public GameObject PlayerRef;
    private PlayerController PlayerController;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController = PlayerRef.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //get the tags of the tboxes that entered this object that is animated in the sword hit
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();


        if (CollisionTag != null)
        {
            //Check if the collided object is an enemy and is kickbackable
            if (CollisionTag.HasTag(TAG_ENEMY)) {                
                if (CollisionTag.HasTag(TAG_KICKBACK))
                {                   
                    //else we kcik him back based on the collider position
                    if(name == OBJ_ATK_FWD)
                    {
                        if (PlayerRef.transform.position.x > collision.transform.position.x)
                        {
                            PlayerController.kickbackPlayer(1);
                        }
                        else
                        {
                            PlayerController.kickbackPlayer(-1);
                        }
                    }

                    //If the current object (out of 3 directions, up, forward and below) has name DOWN we kick the player with a mini jump
                    if (name == OBJ_ATK_DWN)
                    {
                        if ((PlayerController.animator.GetBool("isFalling") || PlayerController.animator.GetBool("isJumping")))
                        {
                            PlayerController.kickbackPlayer(0);
                        }
                    }
                }
            }

            if (CollisionTag.HasTag(TAG_SPIKED))
            {
                //If the current object (out of 3 directions, up, forward and below) has name DOWN we kick the player with a mini jump
                if (name == OBJ_ATK_DWN)
                {
                    if ((PlayerController.animator.GetBool("isFalling") || PlayerController.animator.GetBool("isJumping")))
                    {
                        PlayerController.kickbackPlayer(0);
                    }
                }
            }
            
        }

    }

    private void FixedUpdate()
    {

    }

}
