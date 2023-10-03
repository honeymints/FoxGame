using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kangaroo : Enemy
{
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float jumpLength;
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask ground;
     

    private bool facingLeft = true;
    private Collider2D coll;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //transiiton from build up to fall
        
        if (anim.GetBool("buildUp"))
        {
            if (rb.velocity.y < .1)
            {
                anim.SetBool("falling", true);
                anim.SetBool("buildUp", false);
            }
        }
        //transition from fall to idle
        if (coll.IsTouchingLayers(ground) && anim.GetBool("falling"))
        {
            anim.SetBool("falling", false);
        }
    }
    private void Move()
    {
        if (facingLeft)
        {
            //test to see if we beyond left Cap

            if (transform.position.x > leftCap)
            {
                //make sure if object facing right direction
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector2(1, 1);
                }
                if (coll.IsTouchingLayers(ground))
                {
                    anim.SetBool("buildUp", true);
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    
                }

                //test to see if i am on the ground if so, jump
            }
            //if it is not we want to face right
            else
            {
                facingLeft = false;
            }

        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }
                if (coll.IsTouchingLayers(ground))
                {
                    anim.SetBool("buildUp", true);
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }
  
}
