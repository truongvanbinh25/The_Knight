using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemsBag : MonoBehaviour
{
    //Instance
    public static ItemsBag instance;

    [Header("Items")]
    public GameObject itemsParent;
    public List<ItemBox> itemsList { get => _itemsList; set => _itemsList = value; } 
    private List<ItemBox> _itemsList = new List<ItemBox>();
    private int numberOfItemsAvailable = 0;

    //Grid layout
    private GridLayoutGroup gridLayoutGroup;

    [Header("Swap items")]
    public Image tempImageSwap;
    public ItemBox whichItemSwap;
    public ItemBox whichItemWillSwap;

    [Header("Scroll")]
    public ScrollRect viewScrollRect;

    //Move item
    private float timeHold = 0.12f;
    private float tempTime = 0;

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

        if(Input.GetMouseButton(0) && whichItemSwap == null)
        {
            //Lấy thông tin item ô cần đổi
            tempTime += Time.deltaTime;
            if(tempTime > timeHold)
            {
                viewScrollRect.vertical = false;
                whichItemSwap = GetItemAtPosition(Input.mousePosition); 
                
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Lấy thông tin ô sẽ đổi
            whichItemWillSwap = GetItemAtPosition(Input.mousePosition);

            //Nếu ô sẽ đổi = null thì sẽ load lại thông tin ô cần đổi và không làm gì cả
            if(whichItemSwap != null && whichItemWillSwap == null)
            {
                whichItemSwap.LoadIcon();
                whichItemSwap.LoadText();
            }

            //Nếu ô sẽ đổi != null thì tiến hành swapitem
            if(whichItemSwap != null && whichItemWillSwap != null)
            {
                SwapItem(whichItemSwap, whichItemWillSwap);    
            }

            //Đặt các giá trị cần thiết về lại ban đầu
            tempTime = 0;
            whichItemSwap = null;
            whichItemWillSwap = null;
            viewScrollRect.vertical = true;
            tempImageSwap.gameObject.SetActive(false);
        }
        
        if (whichItemSwap != null)
        {
            //Load icon item vào trong tempimage để người dùng dễ nhìn
            tempImageSwap.gameObject.SetActive(true);
            tempImageSwap.transform.position = Input.mousePosition;
            tempImageSwap.sprite = whichItemSwap.item.iconSprite;
            
            //Tắt icon và text của ô cần đổi
            whichItemSwap.UnloadIcon();
            whichItemSwap.quantityText.gameObject.SetActive(false);
        }
    }

    private void ControllItemsBoxCellSize()
    {
        //Điều chỉnh tỷ lễ giữa các ô item
        gridLayoutGroup = itemsParent.GetComponent<GridLayoutGroup>();

        float witdh = itemsParent.GetComponent<RectTransform>().rect.width;

        float cellSizeX = witdh / gridLayoutGroup.constraintCount;
        float cellSizeY = witdh / gridLayoutGroup.constraintCount;

        gridLayoutGroup.cellSize = new Vector2(cellSizeX, cellSizeY);
    }

    private void GetItems()
    {
        int i = 0;
        foreach(Transform tf in itemsParent.transform)
        {
            ItemBox item = tf.GetComponent<ItemBox>();
            item.positionInBag = i;
            itemsList.Add(item);
            i++;
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

    public ItemBox GetItemAtPosition(Vector2 position)
    {
        foreach (ItemBox itemBox in itemsList)
        {
            RectTransform rectTransform = itemBox.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position))
            {
                return itemBox;
            }
        }
        return null;
    }

    public void SwapItem(ItemBox index, ItemBox target)
    {
        //Nếu ô đổi và ô sẽ đổi khác vị trí nhau
        if(!index.isEmpty && index.positionInBag != target.positionInBag)
        {
            //Nếu ô sẽ đổi là rỗng thì hoán đổi vị trí 2 ô
            if(target.isEmpty)
            {
                target.isEmpty = false;
                target.item = index.item;
                target.itemQuantity = index.itemQuantity;
                target.LoadIcon();
                target.LoadText();

                index.DeleteItem();
            }
            else //Nếu 2 ô đều có item thì hoán đổi 2 item
            {
                ItemBase temp = index.item;
                index.item = target.item;
                target.item = temp;

                index.LoadIcon();
                index.LoadText();

                target.LoadIcon();
                target.LoadText();
            }
        }
        //Nếu ô đổi và ô sẽ đổi bằng vị trí nhau thì không làm gì cả và load lại hình ảnh
        else if (!index.isEmpty && index.positionInBag == target.positionInBag)
        {
            index.LoadIcon();
            index.LoadText();
        }
    }

}
