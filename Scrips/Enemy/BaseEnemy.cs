using System.Collections;
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
    public float speed = 2f;
    public float waitTime = 2.5f;
    public GameObject pointsParent;

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
    protected bool isDeath = false; //Kích hoạt animation Death
    protected int currentPoint = 0;
    protected Vector2 moveDirection;

    [Header("Skeleton")]
    public bool isShield = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerManager = PlayerManager.instance;

        hpSlider.maxValue = numberOfHeath;
        hpSlider.value = numberOfHeath;

        InvokeRepeating("CheckPlayer", 1f, 1f);
        
        tempSpeed = speed;

        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

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

        //Nếu người chơi trong phạm vi tấn công
        if(inDetectionRange)
        {
            //Nếu va chạm với người chơi
            if(Vector2.Distance(playerTranforms.transform.position, transform.position) < attackRange)
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
            if (!isWating && Vector2.Distance(points[currentPoint].transform.position, transform.position) < .1f)
            {
                StartCoroutine(PointCaculate());
            }
        }
        
        Debug.Log(Vector2.Angle(transform.position, points[currentPoint].position));

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

    public virtual void TakeHit(int directionHit)
    {
        isTakeHit = true; //Kích hoạt animation
        transform.position += new Vector3(1f * directionHit, 0, 0); //Khi người chơi tấn công thì sẽ lùi 1 đoạn là 1.5 về hướng quay của người chơi
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
}
