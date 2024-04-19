using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagCanvas : BaseCanvas
{
    public static BagCanvas instance;

    [Header("View UI")]
    public List<GameObject> viewsGOList = new List<GameObject>(); 
    private List<RectTransform> viewsRect = new List<RectTransform>();
    private int viewIndex = 0;

    [Header("Buttons")]
    public GameObject buttonsParent; 
    public int whichIndexButton = 0;
    private List<BagButtons> buttonsList;
    private RectTransform buttonsRect;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Button
        buttonsRect = buttonsParent.GetComponent<RectTransform>();
        GetButtonsIntoList();
        GetViewsRectTransform();

        buttonsList[0].rectTransform.localScale = buttonsList[0].scaleChange;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckOutCanvas(buttonsRect) && CheckOutCanvas(viewsRect[whichIndexButton])) // Kiểm tra xem chuột có nằm ngoài vùng canvas không
            {
                if (gameObject.activeSelf == true)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void GetButtonsIntoList()
    {
        buttonsList = new List<BagButtons>();
        int index = 0;

        foreach (Transform tf in buttonsParent.transform)
        {
            BagButtons bb = tf.GetComponent<BagButtons>();
            bb.index = index;
            buttonsList.Add(bb);
            index++;
        }
    }

    private void GetViewsRectTransform()
    {
        foreach(GameObject go in viewsGOList)
        {
            viewsRect.Add(go.GetComponent<RectTransform>());
        }
    }

    public void ButtonOnClick()
    {
        buttonsList[whichIndexButton].rectTransform.localScale = buttonsList[whichIndexButton].scaleChange;
        viewsGOList[whichIndexButton].SetActive(true);

        for(int i = 0; i < buttonsList.Count; i++)
        {
            if (buttonsList[i].index != whichIndexButton)
            {
                buttonsList[i].rectTransform.localScale = buttonsList[i].scaleNormal;
                viewsGOList[i].SetActive(false);
            }
        }
    }
}
