using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public void EndOfFrameBleed()
    {
        FindObjectOfType<PlayerLifeCanvas>().EndOfFrameBleed();
    }
}
