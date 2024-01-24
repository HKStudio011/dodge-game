using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContronller : MonoBehaviour
{
    private bool isDead;
    private Vector3 mousePos;
    public float minX,minY,maxX,maxY,moveSpeed;

    public bool IsDead { get => isDead; set => isDead = value; }

    // Start is called before the first frame update
    void Start()
    {
        IsDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        if (Input.GetMouseButton(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(Mathf.Clamp(mousePos.x, minX, maxX), Mathf.Clamp(mousePos.y, minY, maxY), 0);
        }
        transform.position = Vector3.Lerp(transform.position, mousePos, moveSpeed * Time.deltaTime);
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        IsDead = true;
    }
    
}
