using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGame : MonoBehaviour
{
    private AudioSource audioSource;
   
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
       
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            audioSource.Play();
        }
    }

   
}
