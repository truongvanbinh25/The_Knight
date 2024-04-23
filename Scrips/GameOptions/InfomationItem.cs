using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfomationItem : MonoBehaviour
{
    public static InfomationItem instance;

    public Image image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI descriptionText;

    private ItemBox whichItemBoxOn;

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateInfomationItem(ItemBox itemBox)
    {
        whichItemBoxOn = itemBox;
        image.sprite = itemBox.item.iconSprite;
        nameText.text = itemBox.item.nameItem.ToString();
        quantityText.text = "Số lượng: " + itemBox.itemQuantity.ToString();
        descriptionText.text = "Mô tả: " + itemBox.item.description.ToString();
    }

    public void UseItem()
    {
        if(whichItemBoxOn != null)
        {
            whichItemBoxOn.UseItem();
            quantityText.text = "Số lượng: " + whichItemBoxOn.itemQuantity.ToString();

            if(whichItemBoxOn.itemQuantity <= 0)
            {
                whichItemBoxOn = null;
                gameObject.SetActive(false);
            }
        }
    }
}
