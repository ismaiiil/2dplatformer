using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantValues;

public class KnightAttackDetector : MonoBehaviour
{
    public EnemyKnightAi knightAiScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Get tag of the current trigger that entered the knight collider
        var CollisionTag = collision.gameObject.GetComponent<CustomTag>();

        if (CollisionTag != null)
        {
            if (CollisionTag.HasTag(TAG_PLAYER))
            {
                knightAiScript.TriggerAttack();
            }
        }
    }
}
