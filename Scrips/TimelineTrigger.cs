using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject demonBoss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playableDirector.Play();
        demonBoss.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
