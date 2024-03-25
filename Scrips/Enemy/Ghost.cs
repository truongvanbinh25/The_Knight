using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ghost : BaseEnemy
{
    //Movement state
    //0: idle
    //1: shriek
    //2: Vanish
    //Ghost chỉ đuổi theo người chơi, không có waypoints

    private GameManager gameManager;
    private enum GhostMovementState { idle, shriek, vanish, apear };
    private GhostMovementState ghostState;

    [Header("Ghost")]
    public bool isApear = false;

    [Header("Effect")]
    private bool effectHasStop = false;
    public GameObject slowEffectGO;

    // Start is called before the first frame update
    protected override void Start()
    {
        gameManager = GameManager.instance;

        base.Start();

        //Stop invoke because ghost doesn't need it
        CancelInvoke("CheckPlayer");

        //Direction to player
        DirectionToPlayer();

        //Effects
        slowEffectGO = gameManager.slowEffectPool.GetObjectFromPool();
    }

    // Update is called once per frame
    protected override void Update()
    {
        hpCanvas.transform.position = transform.position + offset;

        //Nếu va chạm với người chơi
        if (Vector2.Distance(playerTranforms.transform.position, transform.position) < attackRange)
        {
            //Thực hiện việc tấn công
            isAttacking = true; //Kích hoạt animation
            speed = 0;
        }
        else
        {
            isAttacking = false; //Kích hoạt animation
            speed = tempSpeed;

            if (!effectHasStop)
            {
                effectHasStop = true;

                StopSlowEffect();
            }

            //Tính toán lại hướng 
            DirectionPlayerCaculate();
        }

        UpdateAnimationState();
    }

    protected override void UpdateAnimationState()
    {
        if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            ghostState = GhostMovementState.idle;
        }

        if (isAttacking)
        {
            ghostState = GhostMovementState.shriek; //state: 1;
            Attack();
            SideEffects();
        }

        if(isDeath)
        {
            ghostState = GhostMovementState.vanish;
        }

        if(isApear)
        {
            ghostState = GhostMovementState.apear;
        }

        if (isTakeHit)
        {
            EndOfFrameTakeHit();
        }

        animator.SetInteger("state", (int)ghostState);
    }

    private void DirectionToPlayer()
    {
        playerTranforms = playerManager.transform;
    }

    public override void Attack()
    {
        if(!effectHasStop)
        {
            SlowEffects();
        }
    }

    public void SlowEffects()
    {
        if(slowEffectGO == null)
        {
            slowEffectGO = gameManager.slowEffectPool.GetObjectFromPool();
        }

        slowEffectGO.SetActive(true);
        playerManager.runSpeed = playerManager.tempRunSpeed * 0.5f;
        playerManager.crouchSpeed = playerManager.tempCrouchSpeed * 0.5f;
    }

    public void SideEffects() //Minus HP until this die
    {
        //Effect has countinute when the player in attack range
        effectHasStop = false;

        if (numberOfHeath <= 0)
        {
            Die();
        }

        slowEffectGO.transform.position = playerManager.transform.position;
        numberOfHeath -= Time.deltaTime;
        hpSlider.value = numberOfHeath;
    }

    public void StopSlowEffect()
    {
        //Check if it not null
        if (slowEffectGO != null)
        {
            playerManager.runSpeed = playerManager.tempRunSpeed;
            playerManager.crouchSpeed = playerManager.tempCrouchSpeed;
        }
    }

    protected override void EndOfFrameDie()
    {
        StopSlowEffect();
        gameManager.slowEffectPool.ReturnGameObjetToPool(slowEffectGO);
        base.EndOfFrameDie();
    }
}
