using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Dialog Canvas")]
    public bool isTalking = false;

    [Header("Object Pool")]
    public ObjectPool slowEffectPool; // Biến kiểu ObjectPool

    public void Awake()
    {
        instance = this;
    }
}
