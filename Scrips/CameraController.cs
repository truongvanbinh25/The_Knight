using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;

    CinemachineVirtualCamera cam;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + new Vector3(0, 0, -10);
    }
}
