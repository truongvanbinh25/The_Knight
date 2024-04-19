using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itembar : MonoBehaviour
{
    private PlayerManager playerManager;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;
        rectTransform = GetComponent<RectTransform>();
    }

    public void HealHPOnClick()
    {
        PlayerLifeCanvas.instante.HealHP();
    }

    public void MouseEnter()
    {
        rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void MouseExit()
    {
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
}
