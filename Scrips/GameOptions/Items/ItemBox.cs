using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    private InfomationItem infomationItem;
    private ItemsBag bag;

    [Header("Item")]
    public ItemBase item;
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public int itemQuantity = 0;

    public int positionInBag = 0;
    public bool isEmpty = true;

    private void Start()
    {
        infomationItem = InfomationItem.instance;
        bag = ItemsBag.instance;

        quantityText.gameObject.SetActive(false);
    }

    public void LoadIcon()
    {
        if (item != null)
        {
            //Icon
            iconImage.sprite = item.iconSprite;
        
            Color color = iconImage.color;
            color.a = 1;
            iconImage.color = color;
        }
        
    }

    public void UnloadIcon()
    {
        //Icon
        iconImage.sprite = null;
        Color color = iconImage.color;
        color.a = 0;
        iconImage.color = color;
    }

    public void LoadText()
    {
        //Text
        quantityText.gameObject.SetActive(true);
        quantityText.text = "x" + itemQuantity;
    }

    public void LoadInfomationItem()
    {
        if(item != null && !infomationItem.gameObject.activeSelf)
        {
            infomationItem.gameObject.SetActive(true);
            infomationItem.UpdateInfomationItem(this);
        }
        else
        {
            infomationItem.gameObject.SetActive(false);
        } 
    }

    public void UseItem()
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

    public void DeleteItem()
    {
        item = null;
        isEmpty = true;

        UnloadIcon();

        //Quantity text
        quantityText.gameObject.SetActive(false);
    }
}
