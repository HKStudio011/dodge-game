using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExplosionsController : MonoBehaviour
{
    public float destroyTime;
    private GameController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller =GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (controller.IsGameOver) Destroy(gameObject);
        Destroy(gameObject, destroyTime);        
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.IsGameOver) Destroy(gameObject);
    }

}
