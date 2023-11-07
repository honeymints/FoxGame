using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Enemy 
{

    protected virtual void Start()
    {
        base.Start();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health > 0)
        {
            anim.SetTrigger("Damage");

        }
        else if (health <= 0)
        {
            Death();
        }
    }
    private void Death()
    {
        Destroy(gameObject);
    }
}
