using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public Necromancer necromancer;

    public void StartSummon(GameObject whichEnemy)
    {
        GameObject enemy = Instantiate(whichEnemy, transform.position, transform.rotation);
        enemy.transform.position = transform.position + new Vector3(0, enemy.GetComponentInChildren<BoxCollider2D>().size.y * 0.6f, 0);
    }

    public void EndOfFrameSummon()
    {
        necromancer.isSummoning = false;
        gameObject.SetActive(false);
    }
}
