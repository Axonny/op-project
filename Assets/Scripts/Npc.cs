using DialogueSystem;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

public class Npc : MonoBehaviour, INpc
{
    public LevelManager levelManager;

    private void Start()
    {
        Invoke(nameof(Talk), 0.1f);
    }
    
    public void GiveQuest(Quest quest)
    {
        QuestSystem.Instance.AddQuest(quest);
    }

    public void Talk()
    {
        DialogueManager.Instance.StartDialogue(levelManager.Dialogue);
    }
}