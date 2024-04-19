using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemiesSummon
{
    public float manaRequest;
    public List<GameObject> enemiesPrefab;
}

public class Necromancer : BaseEnemy
{
    [Header("Necromancer")]

    [Header("Mana")]
    public Slider manaSlider;
    public float mana;
    public float healManaSpeed = 0.2f;
    private float tempMana;

    [Header("Summon")]
    public Summon summon;
    public bool isSummoning = false;
    public List<EnemiesSummon> enemiesSummon;
 
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
        summon.transform.position = new Vector3(playerManager.transform.position.x, playerManager.transform.position.y - playerManager._collider2d.bounds.extents.y + 0.1f, 0);
        RandomEnemySummon();
    }

    private void RandomEnemySummon()
    {
        int index;
        if (mana >= 1)
        {
            float maxManaRequest = enemiesSummon.Last().manaRequest; // lấy phần tử có manarequest lớn nhất

            if(mana > maxManaRequest)
            {
                index = (int)Random.Range(1, maxManaRequest);
            }
            else
            {
                index = (int)Random.Range(1, mana);
            }

            foreach (EnemiesSummon enemies in enemiesSummon)
            {
                if (enemies.manaRequest == index)
                {
                    MinusMana(index);
                    int enemyIndex = Random.Range(0, enemies.enemiesPrefab.Count);
                    summon.StartSummon(enemies.enemiesPrefab[enemyIndex]);
                    break;
                }
            }
        }
    }


    private void HealMana()
    {
        if(mana >= tempMana)
        {
            mana += Time.deltaTime * healManaSpeed;
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
