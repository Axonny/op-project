using DialogueSystem;
using DialogueSystem.GraphData;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

public class Npc : MonoBehaviour, INpc
{
    public DialogueData dialogue;

    public void GiveQuest(Quest quest)
    {
        QuestSystem.Instance.AddQuest(quest);
    }

    public void Talk()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void PlayerEnter(bool flag)
    {
        if (flag)
        {
            InputSystem.Instance.UseAction += Talk;
        }
        else
        {
            InputSystem.Instance.UseAction -= Talk;
        }
    }
}