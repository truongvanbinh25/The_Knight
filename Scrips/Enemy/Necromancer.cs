using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemySummon
{
    public float manaRequest;
    public GameObject enemyPrefab;
}

public class Necromancer : BaseEnemy
{
    [Header("Necromancer")]

    [Header("Mana")]
    public Slider manaSlider;
    public float mana;
    private float tempMana;

    [Header("Summon")]
    public Summon summon;
    public bool isSummoning = false;
    public List<EnemySummon> enemiesSummon;
   

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        manaSlider.maxValue = mana;
        manaSlider.value = mana;
    }

    // Update is called once per frame
    protected override void Update()
    {
        hpCanvas.transform.position = transform.position + offset;

        if(inDetectionRange && mana >= 1)
        {
            if (Vector2.Distance(playerTranforms.position, transform.position) <= attackRange)
            {
                if(!isSummoning)
                {
                    isAttacking = true;
                    isSummoning = true;
                }
                   
                speed = 0;
            }
            else
            {
                speed = tempSpeed;

                DirectionPlayerCaculate();
            }
            
        }
        else
        {
            HealMana();
            if (!isWating && Vector2.Distance(points[currentPoint].transform.position, transform.position) < .1f)
            {
                StartCoroutine(PointCaculate());
            }
            else
            {
                DirectionPointCaculate();
            }
        }

        UpdateAnimationState();
    }

    public override void Attack()
    {
        summon.gameObject.SetActive(true);
        summon.transform.position = new Vector3(playerManager.transform.position.x, playerManager.transform.position.y - playerManager._collider2d.bounds.extents.y, 0);
        RandomEnemySummon();
    }

    private void RandomEnemySummon()
    {
        int index;
        if(mana >= 1)
        {
            index = (int)Random.Range(1, mana);
            Debug.Log(index);
            foreach(EnemySummon enemy in enemiesSummon)
            {
                if(enemy.manaRequest == index)
                {
                    MinusMana(index);
                    summon.StartSummon(enemy.enemyPrefab, summon.transform);
                    break;
                }
            }
        }
    }


    private void HealMana()
    {
        if(mana >= tempMana)
        {
            mana += Time.deltaTime * 0.03f;
            manaSlider.value = mana;
        }
    }

    private void MinusMana(int numberOfManaMinus)
    {
        if (mana >= 0)
        {
            mana -= numberOfManaMinus;
            manaSlider.value = mana;
        }
    }

    protected override void CheckPlayer()
    {
        if (mana < 1)
            return;
        base.CheckPlayer();
    }
}
