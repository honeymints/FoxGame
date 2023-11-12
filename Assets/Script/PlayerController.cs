using UnityEngine;
using TMPro;
using System;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    //Start Variables
    private Rigidbody2D rb;
    private Animator anim;
    private enum State { idle, running, jumping, falling, hurt, climb};
    private State state = State.idle;
    private Collider2D coll;
    
    private float Hdirection;

    #region Climb booleans

    private bool isNearLadder = false;
    private bool climbHeld = false;
    private bool hasStartedClimbing = false;

    #endregion

    #region Climb parameters

    private float vertical;
    private Transform ladders;
    private float climbSpeed = 0.3f;

    #endregion

    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    //Inspector Variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int coins=0;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private float hurtForce = 3f;
    public float health { get; private set; } = 3f;
    
    public static event Action OnPlayerDeath; //setting event

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
        if (dialogueUI.IsOpen) return;

        if (state != State.hurt)
        {
            Movement();
        }

        anim.SetInteger("state", (int)state); //sets animation on enumerator state
        VelocityState();

        climbHeld = (isNearLadder && Input.GetButton("Climb")) ? true : false;
        vertical = Input.GetAxisRaw("Vertical") * climbSpeed;
        
        CheckIfPlayerStartedClimb();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Interactable != null)
            {
                Interactable.Interact(this);
            }
        }
    }

    private void FixedUpdate()
    {
        Climbing();
    }

    private void CheckIfPlayerStartedClimb()
    {
        if (climbHeld)
        {
            if (!hasStartedClimbing)
            {
                hasStartedClimbing = true;
            }
        }
    }
    

    private void Movement()
    {
        Hdirection = Input.GetAxisRaw("Horizontal");

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

    private void Climbing()
    {
        if (hasStartedClimbing && !climbHeld)
        {
            if (Hdirection<0 || Hdirection>0)
            {
                StopClimb();
            }
        }
        else if (hasStartedClimbing && climbHeld)
        {
            float height = GetComponent<SpriteRenderer>().size.y;
            float topLadderHandler = Half(ladders.transform.GetChild(0).position.y+height);
            float bottomLadderHandler = Half(ladders.transform.GetChild(1).position.y+height);
            float posY = Half(transform.position.y);
            float verticalY = Math.Abs(posY)+Math.Abs(vertical);

            Debug.Log("here is " + height + " " + bottomLadderHandler + " " + topLadderHandler + " " + transform.position.y);
            if(verticalY>=topLadderHandler || verticalY<bottomLadderHandler)
            {
                
                StopClimb();
            }
            else if(verticalY<topLadderHandler || verticalY>=bottomLadderHandler)
            {
                if (!transform.position.x.Equals(ladders.position.x))
                {
                    Debug.Log("change the player's position to ladders");
                    transform.position = new Vector3(ladders.position.x, transform.position.y, transform.position.z);
                }
                rb.bodyType = RigidbodyType2D.Kinematic;
                Vector3 forwardDirection = new Vector3(0, verticalY, 0);
                Vector3 newPos = Vector3.zero;
                if(vertical>0)
                {
                    newPos = transform.position + forwardDirection * Time.deltaTime*3;
                    Debug.Log("when player is going up " + verticalY);
                }
                else if(vertical<0)
                {
                    newPos = transform.position - forwardDirection * Time.deltaTime*3;
                    Debug.Log("when player is going down " + verticalY);
                }
                
                if (newPos != Vector3.zero)
                {
                    rb.MovePosition(newPos);
                    
                }
            }
        }
        
    }

    private float Half(float value)
    {
        return Mathf.Floor(value) + 0.5f;
        
    }

    private void StopClimb()
    {
        if (hasStartedClimbing)
        {
            hasStartedClimbing = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            transform.position = new Vector3(transform.position.x, Half(transform.position.y),transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Collectable"))
        {
            Destroy(col.gameObject);
                coins += 1;
            coinText.text = coins.ToString();
        }

        if (col.CompareTag("Ladders"))
        {
            isNearLadder = true;
            this.ladders= col.transform;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladders"))
        {
            isNearLadder = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
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
        if (other.gameObject.CompareTag("Trap"))
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

        else if (Mathf.Abs(rb.velocity.x) > 1f)
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
