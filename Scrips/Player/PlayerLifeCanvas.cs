using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeCanvas : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerManager playerManager;

    public static PlayerLifeCanvas instante;

    [HideInInspector]
    public List<GameObject> heathSegmentsList;

    public GameObject heathBarParent;
    public Animator borderAni;

    [Space(20)]

    public int heath;

    public void Awake()
    {
        instante = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        playerManager = PlayerManager.instance;

        heath = playerManager.numberOfHeath;

        heathSegmentsList = new List<GameObject>();

        for (int i = 0; i < heath; i++)
        {
            GameObject go = gameManager.heathSegments.GetObjectFromPool();
            go.SetActive(true);
            go.transform.SetParent(heathBarParent.transform, false);
            heathSegmentsList.Add(go);
        }    
    } 

    public void HealHP()
    {
        if(heathSegmentsList.Count >= heath)
        {
            return;
        }
        playerManager.numberOfHeath++;
        GameObject go = gameManager.heathSegments.GetObjectFromPool();
        go.SetActive(true);
        go.transform.SetParent(heathBarParent.transform, false);
        heathSegmentsList.Add(go);
    }

    public void HiddenHeatSegment(int index)
    {
        heathSegmentsList[index].SetActive(false);
        heathSegmentsList.RemoveAt(index);
        borderAni.SetInteger("state", 1);
    }    

    public void EndOfFrameBleed()
    {
        borderAni.SetInteger("state", 0);
    }
}
