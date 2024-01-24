using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombsController : MonoBehaviour
{
    private Vector3 tagret;
    public float moveSpeed;
    public float destroyTime;
    public GameObject explosoin;
    private GameController controller;

    public Vector3 Tagret { get => tagret; set => tagret = value; }

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (controller.IsGameOver) Destroy(gameObject);
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((transform.position - Tagret) * moveSpeed * Time.deltaTime * -1);
        if (controller.IsGameOver) Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (!controller.IsGameOver)
            Instantiate(explosoin, transform.position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            Destroy(gameObject);
        }

    }
}
