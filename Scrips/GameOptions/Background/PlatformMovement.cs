using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public GameObject[] waypoints;
    public int currentWaypoint = 0;

    public float speed = 2f;

    private PlayerManager playerMovement;
    private Rigidbody2D rb;
    private Vector3 moveDirection;

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //Lấy hướng đầu tiên
        DirectionCalculate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(waypoints[currentWaypoint].transform.position, transform.position) < .1f)
        {
            currentWaypoint++;
            if(currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }

            //Thực hiện cập nhật lại hướng đến waypoint tiếp theo
            DirectionCalculate();
        }

    }

    //Cập nhật vận tốc
    private void FixedUpdate()
    {
        rb.velocity = moveDirection * speed;
    }

    //Lấy hướng của waypoints đến platform và chuẩn hóa
    private void DirectionCalculate()
    {
        moveDirection = (waypoints[currentWaypoint].transform.position - transform.position).normalized;
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement.isOnPlatform = true;
            playerMovement.platformRb = rb;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //collision.gameObject.transform.SetParent(null);

            playerMovement.isOnPlatform = false;
        }
    }
}
