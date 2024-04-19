using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagButtons : MonoBehaviour
{
    //Private
    private BagCanvas bagCanvas;

    private RectTransform _rectTransform;
    private Button _button;
   

    //Public
    public int index = 0;
    public Vector3 scaleChange = new Vector3(1, 1, 1);
    public Vector3 scaleNormal;

    //Getter and setter
    public RectTransform rectTransform { get => _rectTransform; set => _rectTransform = value; }
    public Button button { get => _button; set => _button = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();

        scaleNormal = rectTransform.localScale;
    }

    private void Start()
    {
        bagCanvas = BagCanvas.instance;
    }

    public void GetIndexButton()
    {
        bagCanvas.whichIndexButton = index;
    }
}
