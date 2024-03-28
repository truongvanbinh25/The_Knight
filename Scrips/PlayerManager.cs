using Assets.Scrips;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Transactions;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance; 

    private Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2d;
    public Collider2D _collider2d { get => collider2d; }
    private bool isDisable = false;

    private enum MovementState { idle, running, jumping, falling, crouching, crouchwalk, attack, crouchAttack, takeHit, deadth, slide }
    private MovementState state;

    [Space(20)]

    [Header("Horizontal")]    
    public float deadZone = 0.01f;
    public float newDeadZone = 0.0001f;
    private float horizontal = 0f;

    [Header("Life")]
    public int numberOfHeath = 7;
    public bool isDeath = false;
    public bool takeHit = false;
    public bool trigAniDie = false;

    [Header("Attack")]
    private bool isAttacking = false;
    public float attackRange = 1f;

    [Header("Run")]
    public float runSpeed = 2f;
    public bool isOnPlatform = false;
    public Rigidbody2D platformRb;

    [Header("Fall")]   
    public float fallMultiplier = 0;

    [Header("Jump")]
    public float jumpMultiplier = 0;
    public float jumpTime = 0.4f;
    public float jumpForce = 10f;
    private float jumpCounter = 0;
    public bool isJumping = false;

    [Header("Crouch")]
    public float crouchSpeed = 1f;
    public bool isCrouching = false;

    [Header("Slide")]
    public bool isSliding = false;
    public float slideSpeed = 5f;
    public float movementSliding = 0f;
    private Vector2 vecGravity;
    private int direction = 1;

    [Header("Box Collider 2D")]
    public BoxCollider2D idleCollider;
    public BoxCollider2D crouchCollider;

    [Header("Canvas")]
    public PlayerLifeCanvas playerLifeCanvas;

    [Header("Effects")]
    public GameObject dashEffect;
    public GameObject smokeEffect;
    private SpriteRenderer dashSR;
    public bool isSmoke = false;

    //Setting
    private float speed;
    [HideInInspector] public float tempRunSpeed;
    [HideInInspector] public float tempCrouchSpeed;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
        vecGravity = new Vector2(0, -Physics.gravity.y);

        playerLifeCanvas = PlayerLifeCanvas.instante;
        tempRunSpeed = runSpeed;
        tempCrouchSpeed = crouchSpeed;

        //Effects
        dashSR = dashEffect.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeath || isDisable)
            return;

        OutMap();

        GetHorizontal();

        Attacking();

        Jumping();

        Crouching();    

        Sliding();

        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        if(isSliding)
        {
            Vector2 movement = new Vector2(movementSliding * speed * 0.5f, rb.velocity.y);
            rb.velocity = movement;
            return;
        }

        //Đối với người chơi khi di chuyển trên platform sẽ cộng thêm rb.velocity.x của platform
        if (isOnPlatform)
        {
            Vector2 movement = new Vector2(horizontal * speed + platformRb.velocity.x, rb.velocity.y);
            rb.velocity = movement;
        }
        else
        {
            Vector2 movement = new Vector2(horizontal * speed, rb.velocity.y);
            rb.velocity = movement;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(2, 0, 0) * direction, new Vector2(2, 2.2f));
    }

    private void GetHorizontal()
    {
        ////Run
        //horizontal = Input.GetAxis("Horizontal"); //not fit to sliding
        if(Input.GetKeyDown(KeyCode.D) && isGrounded() || Input.GetKeyDown(KeyCode.A) && isGrounded())
        {
            dashEffect.SetActive(true);
        }


        if(Input.GetKey(KeyCode.D))
        {
            if (horizontal <= 0.7f)
            {
                horizontal += deadZone;
            }
            else
            {
                horizontal += newDeadZone;
                if(horizontal >= 1)
                {
                    horizontal = 1;
                }    
            }    
        }
        if(Input.GetKeyUp(KeyCode.D))
        {
            horizontal = 0;
        }


        if (Input.GetKey(KeyCode.A))
        {
            if (horizontal >= -0.7f)
            { 
                horizontal -= deadZone;
            }
            else
            {
                horizontal -= newDeadZone;
                if(horizontal <= -1)
                {
                    horizontal = -1;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            horizontal = 0;
        }
    }

    private void UpdateAnimationState()
    {   
        //Run
        if (horizontal > 0)
        {
            state = MovementState.running;
            spriteRenderer.flipX = false;
            dashSR.flipX = false; //Effect
            direction = 1;
        }
        else if (horizontal < 0)
        {
            state = MovementState.running;
            spriteRenderer.flipX = true;
            dashSR.flipX = true; //Effect
            direction = -1;
        }
        else
        {
            state = MovementState.idle;
        }

        if (!isOnPlatform)
        {
            //Jump
            if(rb.velocity.y > .1f)
            {
                state = MovementState.jumping;
                isCrouching = false;
            }
            else if(rb.velocity.y < -.1f)
            {
                rb.velocity += Vector2.down * fallMultiplier * Time.deltaTime;
                state = MovementState.falling;
                if(isGrounded())
                {
                    //Khi người chơi chạm đất thì sẽ điều chỉnh vị trí của smoke, x sẽ là vị trí của người chơi, y sẽ lấy vị trí người chơi trừ cho dài của box colider sẽ ra vị trí dưới chân
                    smokeEffect.SetActive(true); 
                    smokeEffect.transform.position = new Vector3(transform.position.x, transform.position.y - collider2d.bounds.extents.y, 0);
                }
                isCrouching = false;
            }
        }   
        else
        {
            //Jump
            if (rb.velocity.y > 2f)
            {
                state = MovementState.jumping;
                isCrouching = false;
            }
            else if (rb.velocity.y < -2.1f)
            {
                rb.velocity += Vector2.down * fallMultiplier * Time.deltaTime;
                state = MovementState.falling;
                if (isGrounded())
                {
                    //Khi người chơi chạm đất thì sẽ điều chỉnh vị trí của smoke, x sẽ là vị trí của người chơi, y sẽ lấy vị trí người chơi trừ cho dài của box colider sẽ ra vị trí dưới chân
                    smokeEffect.SetActive(true);
                    smokeEffect.transform.position = new Vector3(transform.position.x, transform.position.y - collider2d.bounds.extents.y, 0);
                }
                isCrouching = false;
            }
        }

        //take hit
        if (takeHit)
        {
            state = MovementState.takeHit;
        }

        //crouch
        if (isCrouching)
        {
            idleCollider.enabled = false;
            crouchCollider.enabled = true;
            speed = crouchSpeed;

            //crouch walk
            if (horizontal > 0)
            {
                state = MovementState.crouchwalk;
            }
            else if (horizontal < 0)
            {
                state = MovementState.crouchwalk;
            }
            else
            {
                state = MovementState.crouching;
            }

            //crouch attack
            if (isAttacking)
            {
                state = MovementState.crouchAttack;
            }
        }
        else
        {
            //Khi đang ngồi mà chuyển ra tư thế chạy thì cần phải đổi collider thì idle ngay lập tức
            //để tránh tính trạng nhảy không được do kiểm tra Ground
            //tại vì collider ban đầu được Get là idle nên khi ngồi sẽ không nhảy đc
            idleCollider.enabled = true;
            crouchCollider.enabled = false;
            speed = runSpeed;

            if (isAttacking)
            {
                state = MovementState.attack;
            }
        }


        if(isSliding)
        {
            state = MovementState.slide;
        }

        //death
        if (trigAniDie)
        {
            state = MovementState.deadth;
        }

        animator.SetInteger("state", (int)state);
    }

    private void Attacking()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            isAttacking = true;
        }
    }

    private void EndOfAttacking()
    {
        isAttacking = false;
        ProcessingEnemy();
    }


    public void TakeHit()
    {
        if (numberOfHeath <= 0)
        {
            Die();
            return;
        }

        //Canvas
        numberOfHeath--;
        Destroy(playerLifeCanvas.heathSegmentsList[numberOfHeath]);
        playerLifeCanvas.heathSegmentsList.RemoveAt(numberOfHeath);
        

        playerLifeCanvas.borderAni.SetInteger("state", 1);
        takeHit = true;
    }

    private void EndOfFrameTakeHit()
    {
        takeHit = false;
    }

    private void Die()
    {
        trigAniDie = true;
        this.tag = "Untagged";
    }
    private void EndOfFrameDie()
    {
        trigAniDie = false;
        isDeath = true;
    }

    private void Jumping()
    {
        //Nếu người chơi nhấn thả thì sẽ nhảy với giá trị jumpForce
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Lấy thành phần vận tốc 
            jumpCounter = 0;
        }

        //Nếu người chơi nhấn giữ thì sẽ nhảy với giá trị cộng thêm là jumpMultiplier
        if (rb.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;

            if (jumpCounter > jumpTime)
                isJumping = false;

            rb.velocity += vecGravity * jumpMultiplier * Time.deltaTime;

        }

        //GetKeyUp: kiểm tra có thả nút Space sau khi nhấn không, nếu không thì nhảy cao, ngược lại thấp
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    private void Crouching()
    {
        if (Input.GetKey(KeyCode.S))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }
    }

    private void Sliding()
    {
        if(Input.GetKeyDown(KeyCode.S) && isGrounded() && !isSliding)
        {
            horizontal = 0;
            if(rb.velocity.x > runSpeed - 0.35f)
            {
                isSliding = true;
                movementSliding = rb.velocity.x;
            }
            else if(rb.velocity.x < -(runSpeed - 0.35f))
            {
                isSliding = true;
                movementSliding = rb.velocity.x;
            }
        }

        if(isSliding)
        {
            if(movementSliding > 0)
            {
                movementSliding -= Time.deltaTime * slideSpeed;
                if(movementSliding <= 0)
                {
                    isSliding = false;
                    horizontal = 0;
                }
            }
            else
            {
                movementSliding += Time.deltaTime * slideSpeed;
                if(movementSliding >= 0)
                {
                    isSliding = false;
                    horizontal = 0;
                }
            }
        }
    }

    private void ProcessingEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(2, 0, 0) * direction, new Vector2(2, 2.2f), 0);

        if(colliders.Length > 0)
        {
            foreach (Collider2D collider in colliders)
            {
                if(collider.CompareTag("Enemy"))
                {
                    BaseEnemy enemy = collider.GetComponent<BaseEnemy>(); //Khi đối tượng trong phạm vi tấn công sẽ thực hiện tấn công
                    if(!enemy.isShield)
                    {
                        enemy.TakeHit(direction);
                    }
                }
            }
        }
    }

    private bool isGrounded() // Kiểm tra va chạm với nền 
    {
        return Physics2D.OverlapBox(collider2d.bounds.center, collider2d.bounds.size, 0f, LayerMask.GetMask("Ground"));
    }

    private void OutMap()
    {
        if(rb.velocity.y < -40)
            Die();
    }    

    public void DisableMovement()
    {
        isDisable = true;
        speed = 0;
        animator.SetInteger("state", 0);

    }

    public void EnableMovement()
    {
        isDisable = false;
    }
}
