using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    public ItemBase item;
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public int itemQuantity = 0;

    public bool isEmpty = true;

    private void Start()
    {
        quantityText.gameObject.SetActive(false);
    }

    public void LoadIcon()
    {
        //Icon
        iconImage.sprite = item.iconSprite;
        
        Color color = iconImage.color;
        color.a = 1;
        iconImage.color = color;
    }

    public void LoadText()
    {
        //Text
        quantityText.gameObject.SetActive(true);
        quantityText.text = "x" + itemQuantity;
    }

    public void DeleteItem()
    {
        item = null;
        isEmpty = true;

        //Icon
        iconImage.sprite = null;
        Color color = iconImage.color;
        color.a = 0;
        iconImage.color = color;

        //Quantity text
        quantityText.gameObject.SetActive(false);
    }

    public void UseEffectBTN()
    {
        if (itemQuantity > 0)
        {
            item.Effect();

            itemQuantity--;
            LoadText();

            if(itemQuantity <= 0)
            {
                DeleteItem();
            }
            
        }
    }
}
