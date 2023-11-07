using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;
    public int health = 100;


    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void JumpedOn()
    {
        anim.SetTrigger("Death");
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
     
        if(health > 0)
        {
            anim.SetTrigger("Damage");

        }
        else if (health <= 0)
        {
            anim.SetTrigger("Death");

            Invoke("Death()", 2); 
        }
    }
    private void Death()
    {
        Destroy(gameObject);
    }
}
