using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyingEye : BaseEnemy
{
    private enum FlyingMovementState { fly, plunge, attack, takeHit, deathFall, deathTouchGround }
    private FlyingMovementState flyingState;

    [Header("Flying Eye")]
    public float mana = 1f;
    public float speedHealMana = 0.0005f;
    public Slider manaSlider;
    public float plungeSpeed = 4f;
    public bool isPlunge = false;

    protected override void Start()
    {
        base.Start();
        tempSpeed = speed;
        manaSlider.maxValue = mana;
    }

    protected override void Update()
    {
        hpCanvas.transform.position = transform.position;

        if(inDetectionRange && mana >= 1)
        {
            speed = plungeSpeed;
            isPlunge = true;
            if(Vector2.Distance(transform.position, playerTranforms.position) < attackRange)
            {
                speed = tempSpeed - 1;
                isAttacking = true;
                isPlunge = false;
            }
        }
        else
        {
            isPlunge = false;
            healthMana();
            if (!isWating && Vector2.Distance(points[currentPoint].transform.position, transform.position) < .1f)
            {
                StartCoroutine(PointCaculate());
            }
        }

        UpdateAnimationState();
    }

    protected override void UpdateAnimationState()
    {
        flyingState = FlyingMovementState.fly;

        if(rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }

        if (isPlunge)
        {
            flyingState = FlyingMovementState.plunge;
        }

        if(isAttacking)
        {
            flyingState = FlyingMovementState.attack;
        }

        if(isTakeHit)
        {
            flyingState = FlyingMovementState.takeHit;
        }

        if(isDeath)
        {
            flyingState = FlyingMovementState.deathFall;
        }

        animator.SetInteger("state", (int)flyingState);
    }

    public override void Attack()
    {
        base.Attack();
        MinusMana();
        DirectionPointCaculate();
        speed = plungeSpeed;
    }

    public override void TakeHit(int directionHit, float pushoutValue)
    {
        base.TakeHit(directionHit, pushoutValue);
        isAttacking = false;
        isPlunge = false;
        MinusMana();
        DirectionPointCaculate();
        speed = plungeSpeed;
    }

    public void MinusMana()
    {
        mana = 0;
        manaSlider.value = mana;
    }

    public void healthMana()
    {
        if (mana >= 1)
            return;
        mana += speedHealMana;
        manaSlider.value = mana;
    }

    protected override void CheckPlayer()
    {
        if (mana < 1)
        {
            inDetectionRange = false;
            return;
        }
            
        base.CheckPlayer();
    }
}
