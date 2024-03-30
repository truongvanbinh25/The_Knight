using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : BaseEnemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (!isGrounded())
            return;
        base.FixedUpdate();
    }

    //Check if ahead is ground then wayback another point
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            StartCoroutine(PointCaculate());
        }
    }
}
