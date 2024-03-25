using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Player Life Canvas")]
    public GameObject playerLifeCanvas;

    [Header("Icon")]
    public Image icon;

    [Header("Text Mesh Pro")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI sentenceText;

    [Space(10)]
    [Header("Animator")]
    public Animator animator;

    [Space(10)]
    [Header("Setting")]
    public float readingSpeed = 0.2f;

    [Header("Timeline")]
    public PlayableDirector demonTimeline;

    private Queue<DialogueLine> dialogueLineQueue;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        dialogueLineQueue = new Queue<DialogueLine>();
        gameObject.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        playerLifeCanvas.SetActive(false);

        gameObject.SetActive(true);

        foreach(DialogueLine line in dialogue.sentences)
        {
            dialogueLineQueue.Enqueue(line);
        }

        DialogueLine line1 = dialogueLineQueue.Dequeue();

        if(line1.character.icon != null)
        {
            icon.sprite = line1.character.icon;
        }    
        nameText.text = line1.character.name;
        StartCoroutine(CombineSentences(line1.sentence, sentenceText));
    }

    public void NextLine()
    {
        if (dialogueLineQueue.Count <= 0)
        {
            animator.SetInteger("state", 1);
            return;
        }      

        StopAllCoroutines();

        DialogueLine line1 = dialogueLineQueue.Dequeue();

        if (line1.character.icon != null)
        {
            icon.sprite = line1.character.icon;
        }
        nameText.text = line1.character.name;
        StartCoroutine(CombineSentences(line1.sentence, sentenceText));
    }

    public IEnumerator CombineSentences(string str, TextMeshProUGUI textMesh)
    {
        string temp = string.Empty;
        foreach (char s in str)
        {
            temp += s;
            textMesh.text = temp;
            yield return new WaitForSeconds(readingSpeed);
        }
    }    

    public void EndOfLine()
    {
        playerLifeCanvas.SetActive(true);

        if(demonTimeline != null)
        {
            demonTimeline.Resume();
        }
        Destroy(transform.parent.gameObject);
    }    
}
