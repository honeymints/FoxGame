using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    
    public Transform cam;
    public float relativeMove = .3f;
    public bool lockY = false;
    // Start is called before the first frame update
 

    // Update is called once per frame
    void Update()
    {
        
        if (lockY)
        {
            transform.position = new Vector2(cam.position.x * relativeMove, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(cam.position.x * relativeMove, cam.position.y * relativeMove);
        }
    }

}