using System;
using DialogueSystem;
using DialogueSystem.GraphData;
using Interfaces;
using UnityEngine;

public class Npc : MonoBehaviour, INpc
{
    public DialogueContainer dialogue;

    public void GiveQuest()
    {
        throw new NotImplementedException();
    }

    public void CheckQuestCondition()
    {
        throw new NotImplementedException();
    }

    public void Talk()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void PlayerEnter(bool flag)
    {
        if (flag)
        {
            Player.Instance.UseAction += Talk;
        }
        else
        {
            Player.Instance.UseAction -= Talk;
        }
    }
}