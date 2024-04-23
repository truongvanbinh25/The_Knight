using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour
{
    protected enum eKindOfItem { hp, mana }
    public string nameItem;
    [HideInInspector] public Sprite iconSprite;
    public string description;
    public string kindOfItem;

    public virtual void Effect()
    {

    }    
}
