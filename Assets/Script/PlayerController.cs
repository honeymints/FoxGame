 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
public class PlayerController : MonoBehaviour
{

    //Start Variables
    private Rigidbody2D rb;
    private Animator anim;
    private enum State { idle, running, jumping, falling, hurt };
    private State state = State.idle;
    private Collider2D coll;
    

    //Inspector Variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int coins=0;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private float hurtForce = 3f;
    public float health { get; private set; } = 3f;



    public  static event Action OnPlayerDeath; //setting event

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

    }


    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -8f)
        {
            transform.position = new Vector3(-1f, 0.65f, 0);
        }
        if (state != State.hurt)
        {
            Movement();
        }
        VelocityState();
        anim.SetInteger("state", (int)state); //sets animation on enumerator state
    }
   
   


    private void Movement()
    {
        float Hdirection = Input.GetAxisRaw("Horizontal");

        if (Hdirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (Hdirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);

        }

        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag== "Collectable")
        {
            Destroy(collision.gameObject);
                coins += 1;
            coinText.text = coins.ToString();
            
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if(state == State.falling) 
            {
                enemy.JumpedOn();

                Jump();
            }
            else 
            {
                state = State.hurt;
                HandleHealth();


               if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy is to my right and i should move to the left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                    rb.velocity = new Vector2(rb.velocity.x, 7);
                }
                else
                {
                    //enemy is to my left and i should move to the right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                    rb.velocity = new Vector2(rb.velocity.x, 7);
                }
            }
        }
        if (other.gameObject.tag=="Trap")
        {
            
            Jump();
            state = State.hurt;
            HandleHealth();
        }

    }
    private void Death()
    {
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        coll.enabled = false;
    }
     
   
    public void HandleHealth()
    {
        
        health -= 1;
        if (health<=0)
        {
            OnPlayerDeath?.Invoke();
            Death();
        }
       
        //dealing with health, and updating health ui
  
        
    }
    
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }
    private void VelocityState()
    {
        
        if (state==State.idle && !(coll.IsTouchingLayers(ground)))
        {
            state = State.falling;
        }
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }

        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }

        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            
            state = State.idle;
            rb.velocity = Vector3.zero;
        }

    }

}
