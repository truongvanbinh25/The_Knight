using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class DemonBoss : BaseEnemy
{
    private enum DemonMovementState { idle, prepareAttack, attack, takeHit, death }
    private DemonMovementState d_state;
    private GameManager gameManager;

    [Header("Demon Boss")]

    [Header("Breath")]
    public GameObject breathPrefab;
    public float breathSpeed = 4f;

    [Header("Canvas")]
    public GameObject infoCanvas;
    public GameObject fills;
    public Slider manaSlider;

    [Header("Fire Totem")]
    public FireTotem fireTotem;

    [Header("Setting")]
    public float mana = 1f;
    public float speedHealMana = 0.0001f;
    public bool isRepareAttack = false;
    public float cooldownTimeRepareAttack = 3f;
    public Transform restPoint;

    [Header("Death")]
    public float timeDeath = 3f;

    [Header("Effects")]
    public GameObject explotionEffect;

    [Header("Necromancer")]
    public GameObject necromancerPrefab;

    private bool isDisable = false;
    private float tempHP;
    private float tempMana;
    private Stack<RectTransform> quenueFills; //Peek: chỉ lấy phần tử ra không xóa, Pop: lấy phần tử ra và xóa
    private RectTransform currentFill;
    private bool getPlayerDirection = false; // Just get once time if has player around
    private Vector2 playerDirection;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        gameManager = GameManager.instance;

        tempSpeed = speed;

        //HP
        tempHP = numberOfHeath;

        //Fills HP
        GetChildHPRectTransforms();
        currentFill = quenueFills.Pop();
        hpSlider.fillRect = currentFill;
        hpSlider.maxValue = numberOfHeath;

        //Mana
        tempMana = mana;
        manaSlider.maxValue = mana;
        manaSlider.value = mana;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(!isDisable)
        {
            if (inDetectionRange && mana >= 1)
            {
                //Nếu va chạm với người chơi
                if (Vector2.Distance(playerTranforms.transform.position, transform.position) < attackRange)
                {
                    //Thực hiện việc tấn công
                    isRepareAttack = true;

                    if(!getPlayerDirection)
                    {
                        getPlayerDirection = true;
                        playerDirection = (playerTranforms.position - transform.position).normalized;
                    }
                }
            }
            else
            {
                if (mana < 0.1f) //If mana doesn't full
                {
                    if (Vector2.Distance(restPoint.position, transform.position) < .1f) // If in the reset point
                    {
                        speed = 0;
                        spriteRenderer.flipX = false;

                        //Healmana
                        StartCoroutine(HealFullMana()); 
                    }
                    else
                    {
                        DirectionRestPointCaculate();
                    }
                }
                else//If mana is full then find the player to kill
                {
                    if (!isWating && Vector2.Distance(points[currentPoint].transform.position, transform.position) < .1f)
                    {
                        StartCoroutine(PointCaculate());
                    }
                }
            }
        }

        UpdateAnimationState();
    }

    private void GetChildHPRectTransforms()
    {
        quenueFills = new Stack<RectTransform>();

        // Lặp qua tất cả các con trong fills và lấy RectTransform của chúng
        foreach (Transform child in fills.transform)
        {
            RectTransform rectTransform = child.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                quenueFills.Push(rectTransform);
            }
        }
    }

    private void DirectionRestPointCaculate()
    {
        moveDirection = (restPoint.position - transform.position).normalized;
    }

    protected override void UpdateAnimationState() //please call it on Update() :)))))
    {
        d_state = DemonMovementState.idle;

        if(rb.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if(rb.velocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }

        if (isTakeHit)
        {
            d_state = DemonMovementState.takeHit;
        }

        if (isRepareAttack)
        {
            speed = 0;
            isTakeHit = false;
            d_state = DemonMovementState.prepareAttack;
        }

        if(isAttacking)
        {
            d_state = DemonMovementState.attack;
        }

        if(isDeath)
        {
            d_state = DemonMovementState.death;
            speed = 0;
        }

        animator.SetInteger("state", (int)d_state);
    }

    protected override void UpdateHealth()
    {
        UpdateFillHP();
        base.UpdateHealth();
    }

    private void UpdateFillHP()
    {
        if(numberOfHeath <= 0)
        {
            if(quenueFills.Count > 0)
            {
                //HP
                numberOfHeath = tempHP; //Thiết lập lại máu

                //Fill
                currentFill = quenueFills.Pop(); // Lấy phần tử tiếp theo
                hpSlider.fillRect = currentFill; //Gán phần tử tiếp theo
                hpSlider.maxValue = numberOfHeath;
                hpSlider.value = numberOfHeath;
                return;
            }
        }
    }

    private IEnumerator HealFullMana()
    {
        while(mana < tempMana)
        {
            fireTotem.hasDemonOn = true;
            HealMana();

            if (mana >= tempMana) //if mana full then speed recovery
            {
                speed = tempSpeed;
            }

            yield return new WaitForSeconds(0.2f);
        }

        fireTotem.hasDemonOn = false;
    }

    private void HealMana()
    {
        mana += speedHealMana;
        manaSlider.value = mana;
    }

    private void MinusMana()
    {
        mana -= 1;
        manaSlider.value = mana;
    }

    protected override void CheckPlayer()
    {
        if (mana <= 0.1f)
            return;
        base.CheckPlayer();
    }

    private void EndOfFramePrepareAttack()
    {
        StartCoroutine(DemonAttack());
    }

    public void EndOfAttack() //activated in breath
    {
        isAttacking = false;
        isRepareAttack = false;
        getPlayerDirection = false;
        speed = tempSpeed;
    }

    protected override void EndOfFrameDie()
    {
        fireTotem.isDeadth = true;
        
        explotionEffect.SetActive(true);
        explotionEffect.transform.position = transform.position;
    }

    public override void TakeHit(int directionHit, float pushoutValue)
    {
        if (isAttacking || isRepareAttack)
        {
            return;
        }
        isTakeHit = true;
    }

    private IEnumerator DemonAttack()
    {
        yield return new WaitForSeconds(cooldownTimeRepareAttack);
        isAttacking = true;
        MinusMana();
        CreateBreathFire();
    }

    private void CreateBreathFire()
    {
        Breath breath = Instantiate(breathPrefab, transform.position, Quaternion.identity).GetComponent<Breath>();
        breath.demonBoss = this;
        breath.speed = breathSpeed; //speed
        breath.moveDirection = playerDirection; //direction
    }

    public void DisableMovement()
    {
        isDisable = true;
        speed = 0;
        infoCanvas.SetActive(false);
    }

    public void EnableMovement()
    {
        isDisable = false;
        speed = tempSpeed;
        infoCanvas.SetActive(true);
    }
}
