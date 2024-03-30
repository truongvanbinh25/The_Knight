using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : BaseEnemy
{
    [Header("Skeleton")]
    public float shieldTime = 10f;
    public float shieldSpeed = 1f;
    
    public Slider manaSlider;
    public float speedHealMana = 0.0003f;
    
    protected override void Start()
    {
        base.Start();
        manaSlider.maxValue = shieldTime;
    }

    protected override void Update()
    {
        base.Update();
        if(inDetectionRange)
        {
            if(Vector2.Distance(transform.position, playerTranforms.position) < 5f && Vector2.Distance(transform.position, playerTranforms.position) > 1.5f && shieldTime > 0)
            {
                isShield = true;
                speed = shieldSpeed;
                MinusMana();
            }
            else
            {
                isShield = false;
            }
        }
        else
        {
            HealMana();
        }
        
    }

    protected override void FixedUpdate()
    {
        if (!isGrounded())
        {
            return;
        }
        base.FixedUpdate();
    }

    private void MinusMana()
    {
        shieldTime -= Time.deltaTime;
        manaSlider.value = shieldTime;
    }    

    private void HealMana()
    {
        shieldTime += speedHealMana;
        manaSlider.value = shieldTime;
    }    

    private void EndOfFrameShield()
    {
        //isShield = false;
    }

    //Check if ahead is ground then wayback another point
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            StartCoroutine(PointCaculate());
        }
    }
}
