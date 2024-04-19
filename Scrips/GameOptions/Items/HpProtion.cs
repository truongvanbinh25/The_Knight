using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HpProtion : ItemBase
{
    private ItemsBag bag;

    private void Start()
    {
        bag = ItemsBag.instance;
        iconSprite = GetComponent<SpriteRenderer>().sprite;
        kindOfItem = eKindOfItem.hp.ToString();
    }

    public override void Effect()
    {
        Debug.Log("heal hp");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            bag.AddItem(this);
            Destroy(gameObject);
        }
    }
}