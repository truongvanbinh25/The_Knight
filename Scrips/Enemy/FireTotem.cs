using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class FireTotem : MonoBehaviour
{
    private enum MovementState { idle, healMana, exit }
    private MovementState state;

    [Header("Demon")]
    public DemonBoss demonBoss;

    public bool hasDemonOn = false;
    public bool isDeadth = false;

    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        state = MovementState.idle;

        if(hasDemonOn)
        {
            state = MovementState.healMana;
        }

        if(isDeadth)
        {
            state = MovementState.exit;
        }

        animator.SetInteger("state", (int)state);
    }

    private void EndOfFrameExit()
    {
        Destroy(gameObject);
    }
}
