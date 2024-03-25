using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breath : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private enum BreathState { start, fire, end }
    private BreathState state;

    public DemonBoss demonBoss;
    public bool isFire = false;
    public bool isEndBreath = false;
    public float speed;
    public float damageRange = 4f;
    public float timeFire = 4f;
    public float timeRepeatFire = 0.5f;
    public float lifeTime = 3f; 

    public Vector2 moveDirection;

    private float countTimeFire = 0;
    private float countLifeTime = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.velocity = moveDirection * speed;
    }

    private void Update()
    {
        Life();

        UpdateBreathState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Ground"))
        {
            isFire = true;

            if(collision.CompareTag("Ground"))
            {
                rb.velocity = Vector2.zero; // stop move
            }
                
            //damage
            StartCoroutine(ApplyDamage());
        }
    }

    private void UpdateBreathState()
    {
        if(rb.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }

        //Fire
        if (isFire)
        {
            state = BreathState.fire;
        }

        //End fire
        if(isEndBreath)
        {
            state = BreathState.end;
        }

        animator.SetInteger("state", (int)state);
    }

    private void EndOfFrameEndFire()
    { 

        Destroy(this.gameObject);
    }

    private void Damage()
    {
        countTimeFire += 0.5f;

        if(countTimeFire >= timeFire)
        {
            isFire = false;
            isEndBreath = true;
            demonBoss.EndOfAttack();
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, damageRange);
        if(colliders.Length > 0)
        {
            foreach(Collider2D collider in colliders)
            {
                if(collider.CompareTag("Player"))
                {
                    PlayerManager.instance.TakeHit();
                }
            }
        }
        
    }

    private IEnumerator ApplyDamage()
    {
        while(isFire)
        {
            Damage();

            yield return new WaitForSeconds(timeRepeatFire);
        }
    }

    private void Life()
    {
        if(isFire)
            return;
            
        countLifeTime += Time.deltaTime;

        if(countLifeTime >= lifeTime)
        {
            isFire = true;
            StartCoroutine(ApplyDamage());
        }
    }
}
