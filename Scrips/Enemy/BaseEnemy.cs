﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scrips;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BaseEnemy : MonoBehaviour, IEnemy
{
    protected Rigidbody2D rb;
    protected BoxCollider2D boxCol;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected PlayerManager playerManager;
    protected List<Transform> points;

    protected enum MovementState { idle, run, attack, takeHit, death, shield }
    protected MovementState state;

    [Header("General")]
    public float DetectionRange = 6f;
    public float attackRange = 3f;
    public bool isDeath = false; //Kích hoạt animation Death
    public float speed = 2f;
    public float waitTime = 2.5f;
    public GameObject pointsParent;
    public BoxCollider2D _boxCol { get => boxCol; }

    protected float tempSpeed;

    [Space(20)]
    [Header("Health")]
    public GameObject hpCanvas;
    public Slider hpSlider;
    public Vector3 offset;

    [Header("Shared")]
    public float numberOfHeath = 3;
    public bool isAttacking = false; // This key to check the animation attack
    public bool inDetectionRange = false;
    public bool isWating = false; //Dùng để kích hoạt Coroutine thực hiện 1 lần duy nhất khi đến waypoint
    public Transform playerTranforms;

    public bool isTakeHit = false; //Kích hoạt animation TakeHit
    
    protected int currentPoint = 0;
    protected Vector2 moveDirection = new Vector2();

    [Header("Special")]
    public bool canItFly = false;

    protected bool isShield = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerManager = PlayerManager.instance;

        //Components
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        //Slider
        hpSlider.maxValue = numberOfHeath;
        hpSlider.value = numberOfHeath;

        InvokeRepeating("CheckPlayer", 1f, 1f);
        
        tempSpeed = speed;

        //Points
        points = new List<Transform>();
        GetPoints();
        if(points.Count > 0)
        {
            DirectionPointCaculate();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        hpCanvas.transform.position = transform.position + offset;
        
        if(!isGrounded() && !canItFly)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            //Set points follow player if player fall
            pointsParent.transform.position = new Vector3(pointsParent.transform.position.x, transform.position.y, 0);

            return;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }

        //Nếu người chơi trong phạm vi tấn công
        if (inDetectionRange)
        {
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

                //Tính toán lại hướng 
                DirectionPlayerCaculate();
            }
        }
        else
        {
            if (!isWating && Vector2.Distance(points[currentPoint].transform.position, transform.position) < .4f)
            {
                StartCoroutine(PointCaculate());
            }
        }

        UpdateAnimationState();
    }

    protected virtual void FixedUpdate()
    {
        rb.velocity = moveDirection * speed;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }

    protected void GetPoints()
    {
        if(pointsParent != null)
        {
            foreach (Transform tf in pointsParent.transform)
            {
                points.Add(tf);
            }
        }
    }

    //Chuyển tiếp điểm
    protected IEnumerator PointCaculate()
    {
        isWating = true;
        speed = 0;
        yield return new WaitForSeconds(waitTime);
        
        currentPoint++;
        if (currentPoint >= points.Count)
        {
            currentPoint = 0;
        }
        DirectionPointCaculate();
        isWating = false;
        speed = tempSpeed;
    }   
    
    //Tính hướng đến điểm
    protected void DirectionPointCaculate()
    {
        moveDirection = (points[currentPoint].transform.position - transform.position).normalized;
    }

    //Tính hướng đến player
    protected void DirectionPlayerCaculate()
    {
        //If enemy walker follow player and go beyond the boundary then set points follow enemy
        if(!canItFly)
        {
            pointsParent.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        
        //Change direction to player
        moveDirection = (playerTranforms.transform.position - transform.position).normalized;
    }

    protected virtual void UpdateAnimationState()
    {
        if (rb.velocity.x > 0)
        {
            state = MovementState.run;
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            state = MovementState.run;
            spriteRenderer.flipX = true;
        }
        else if (rb.velocity.y == 0)
        {
            state = MovementState.idle;
        }

        if (isAttacking)
        {
            state = MovementState.attack;
        }    

        if (isShield)
        {
            state = MovementState.shield;
        }

        if(isTakeHit)
        {
            state = MovementState.takeHit;
        }

        if (isDeath)
        {
            state = MovementState.death;
        }


        animator.SetInteger("state", (int)state);
    }

    public void Move()
    {
        
    }

    protected void EndOfTheFrameAttack()
    {
        isAttacking = false;
        speed = tempSpeed; //Trả lại speed

        Attack();
    }

    public virtual void Attack()
    {
        playerManager.TakeHit();
    }

    protected void EndOfFrameTakeHit()
    {
        isTakeHit = false; //Kết thúc animation
           
        //Trừ máu
        UpdateHealth();
    }

    public virtual void TakeHit(int directionHit, float pushoutValue)
    {
        if(!isShield) //Nếu đang bật khiêng thì bỏ qua
        {
            isTakeHit = true; //Kích hoạt animation
            transform.position += new Vector3(pushoutValue * directionHit, 0, 0); //Khi người chơi tấn công thì sẽ lùi 1 đoạn là 1.5 về hướng quay của người chơi
        }
    }

    protected virtual void UpdateHealth()
    {
        if(numberOfHeath > 0)
        {
            numberOfHeath--;
            hpSlider.value--;
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        isDeath = true;
    }

    protected virtual void EndOfFrameDie()
    {
        Destroy(transform.parent.gameObject);
    }

    protected virtual void CheckPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, DetectionRange);

        bool check = false;

        foreach(Collider2D collider in colliders)
        {
            //Nếu người chơi trong phạm vi tấn công thì tính hướng của người chơi
            if(collider.CompareTag("Player"))
            {
                inDetectionRange = true;
                playerTranforms = collider.GetComponent<Transform>();
                DirectionPlayerCaculate();
                check = true;
                break;
            }
        }  
        
        if(!check)
        {
            inDetectionRange = false;
            playerTranforms = null;
            DirectionPointCaculate();
        }
    }

    protected virtual bool isGrounded() // Kiểm tra va chạm với nền 
    {
        return Physics2D.OverlapBox(boxCol.bounds.center, boxCol.bounds.size, 0f, LayerMask.GetMask("Ground"));
    }

}
