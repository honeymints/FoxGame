using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaddersHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        Vector3 newPos = transform.GetChild(0).position;
        float height = GetComponent<SpriteRenderer>().size.y;
        float width = GetComponent<SpriteRenderer>().size.x;
        float topPosY=transform.GetChild(0).position.y + height / 2;
        float bottomPosY=transform.GetChild(1).position.y - height / 2;
        transform.GetChild(0).position = new Vector3(newPos.x, topPosY, newPos.z);
        transform.GetChild(1).position = new Vector3(newPos.x, bottomPosY, newPos.z);
        GetComponent<BoxCollider2D>().size = new Vector2(width, height);
        GetComponent<BoxCollider2D>().offset=Vector2.zero;

    }
}
