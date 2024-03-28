using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public Necromancer necromancer;

    public void StartSummon(GameObject whichEnemy, Transform transform)
    {
        Instantiate(whichEnemy, transform.position, transform.rotation);
    }

    public void EndOfFrameSummon()
    {
        necromancer.isSummoning = false;
        gameObject.SetActive(false);
    }
}
