using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeCanvas : MonoBehaviour
{
    public static PlayerLifeCanvas instante;

    [HideInInspector]
    public List<GameObject> heathSegmentsList;

    public GameObject heathSegmentsPrefab;
    public GameObject heathBarGO;
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
        heath = PlayerManager.instance.numberOfHeath;

        heathSegmentsList = new List<GameObject>();

        for (int i = 0; i < heath; i++)
        {
            heathSegmentsList.Add(Instantiate(heathSegmentsPrefab, heathBarGO.transform));
        }    
    } 
    public void EndOfFrameBleed()
    {
        borderAni.SetInteger("state", 0);
    }
}
