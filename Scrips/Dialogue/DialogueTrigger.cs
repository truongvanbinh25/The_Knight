using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[Serializable]
public class DialogueLine
{
    public DialogueCharacter character;

    [TextArea(3, 10)]
    public string sentence;
}

[Serializable]
public class Dialogue
{
    public List<DialogueLine> sentences = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    private DialogueManager dialogueManager;
    public Dialogue dialogue;

    public bool wasTalk = false;

    private void Start()
    {
        dialogueManager = DialogueManager.instance;
    }

    public void StartDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !wasTalk)
        {
            StartDialogue();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            wasTalk = true;
        }
    }

    public void NextLine()
    {
        dialogueManager.NextLine();
    }
}

