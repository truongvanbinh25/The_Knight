using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private PlayerManager playerManager;
    public GameObject explotion2;

    private void Start()
    {
        playerManager = PlayerManager.instance;
    }

    public void EndOfFrameBleed()
    {
        FindObjectOfType<PlayerLifeCanvas>().EndOfFrameBleed();
    }

    public void EndOfFrameDash()
    {
        playerManager.dashEffect.SetActive(false);
    }

    public void EndOfFrameSmoke()
    {
        playerManager.smokeEffect.SetActive(false);
    }

    public void EndOfFrameExplotion2()
    {
        explotion2.SetActive(false);
        Destroy(FindObjectOfType<DemonBoss>().transform.parent.gameObject);
    }
  
}
