using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public Necromancer necromancer;

    public void StartSummon(GameObject whichEnemy)
    {
        Instantiate(whichEnemy, transform.position + new Vector3(0, 0.2f, 0), transform.rotation);
    }

    public void EndOfFrameSummon()
    {
        necromancer.isSummoning = false;
        gameObject.SetActive(false);
    }
}
