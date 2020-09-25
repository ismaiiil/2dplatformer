using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantValues;

public class AttacksScript : MonoBehaviour
{

    public GameObject PlayerRef;

    private MovementController movementController;
    // Start is called before the first frame update
    void Start()
    {
        movementController = PlayerRef.gameObject.GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();


        if (CollisionTag != null)
        {
            if (CollisionTag.HasTag(TAG_ENEMY)) {
                if (CollisionTag.HasTag(TAG_KICKBACK))
                {
                    if (name == OBJ_ATK_DWN)
                    {
                        if ((movementController.animator.GetBool("isFalling") || movementController.animator.GetBool("isJumping")))
                        {
                            movementController.kickbackPlayer(0);
                        }
                    }
                    else if(name == OBJ_ATK_FWD)
                    {
                        if (PlayerRef.transform.position.x > collision.transform.position.x)
                        {
                            movementController.kickbackPlayer(1);
                        }
                        else
                        {
                            movementController.kickbackPlayer(-1);
                        }
                    }
                }
                
            }
            
        }

    }

    private void FixedUpdate()
    {
        if (PlayerRef != null) {
            
        }

    }

}
