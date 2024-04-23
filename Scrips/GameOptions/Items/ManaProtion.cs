using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaProtion : ItemBase
{
    private ItemsBag bag;

    private void Start()
    {
        bag = ItemsBag.instance;
        iconSprite = GetComponent<SpriteRenderer>().sprite;
        kindOfItem = eKindOfItem.mana.ToString();
    }

    public override void Effect()
    {
        Debug.Log("heal mana");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            bag.AddItem(this);
            gameObject.SetActive(false);
        }
    }
}
