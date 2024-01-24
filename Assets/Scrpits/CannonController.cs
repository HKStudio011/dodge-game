using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{


    public GameObject bomb;
    public float minBombTime;
    public float maxBombTime;
    public float throughBombTime;

    private Animator animator;
    private GameObject player;
    private float bombTime;
    private float lastBomdTime;
    private AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        UpdateBombTime();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastBomdTime + bombTime - throughBombTime)
        {
            animator.ResetTrigger("Fire");
            sound.Stop();
        }
        if (Time.time > lastBomdTime + bombTime)
        {
            ThroughBomb();
        }
        
    }

    void UpdateBombTime()
    {
        lastBomdTime = Time.time;
        bombTime = Random.Range(minBombTime, maxBombTime + 1);
    }
    void ThroughBomb()
    {
        animator.SetTrigger("Fire");
        sound.Play();
        Vector3 vector;
        if (gameObject.GetComponent<SpriteRenderer>().flipX)
            vector = new Vector3(transform.position.x - 2, transform.position.y + 1, 0);
        else vector = new Vector3(transform.position.x + 2, transform.position.y + 1, 0);
        GameObject bombTemp= Instantiate(bomb, vector, Quaternion.identity);
        bombTemp.GetComponent<BombsController>().Tagret = player.transform.position;
        UpdateBombTime();
    }
}
