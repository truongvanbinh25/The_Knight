using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ItemsBag : MonoBehaviour
{
    public static ItemsBag instance;

    private List<ItemBox> _itemsList = new List<ItemBox>();
    private int numberOfItemsAvailable = 0;

    private GridLayoutGroup gridLayoutGroup;

    public GameObject itemsParent;

    public List<ItemBox> itemsList { get => _itemsList; set => _itemsList = value; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GetItems();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        ControllItemsBoxCellSize();
    }

    private void ControllItemsBoxCellSize()
    {
        gridLayoutGroup = itemsParent.GetComponent<GridLayoutGroup>();

        float witdh = itemsParent.GetComponent<RectTransform>().rect.width;
        float height = itemsParent.GetComponent<RectTransform>().rect.height;

        float cellSizeX = witdh / gridLayoutGroup.constraintCount;
        float cellSizeY = height / gridLayoutGroup.constraintCount;

        gridLayoutGroup.cellSize = new Vector2(cellSizeX, cellSizeY);


        Debug.Log(cellSizeX +  " " + cellSizeY);
    }

    private void GetItems()
    {
        foreach(Transform tf in itemsParent.transform)
        {
            itemsList.Add(tf.GetComponent<ItemBox>());
        }
    }

    public void AddItem(ItemBase item)
    {
        if(numberOfItemsAvailable < itemsList.Count)
        {
            numberOfItemsAvailable++;
            foreach(ItemBox itemBox in itemsList)
            {
                //Nếu đã có trong bag thì tăng số lượng
                if (!itemBox.isEmpty && itemBox.item.kindOfItem == item.kindOfItem)
                {
                    //Text
                    itemBox.itemQuantity++;
                    itemBox.LoadText();
                    return;
                }

                //Nếu chưa có thì thêm vào bag
                if (itemBox.isEmpty)
                { 
                    itemBox.isEmpty = false;
                    itemBox.item = item;

                    //Icon
                    itemBox.LoadIcon();

                    //Text
                    itemBox.itemQuantity++;
                    itemBox.LoadText();
                    return;
                }
            }
        }
    }
}
