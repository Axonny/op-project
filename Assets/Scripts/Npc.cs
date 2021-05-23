using DialogueSystem;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

public class Npc : MonoBehaviour, INpc
{
    public LevelManager levelManager;

    private void Start()
    {
        Talk();
    }
    
    public void GiveQuest(Quest quest)
    {
        QuestSystem.Instance.AddQuest(quest);
    }

    public void Talk()
    {
        DialogueManager.Instance.StartDialogue(levelManager.Dialogue);
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