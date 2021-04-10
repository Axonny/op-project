using System;
using Interfaces;
using UnityEngine;

public class Npc : MonoBehaviour, INpc
{
    public Player player;
    public bool IsPlayerNear { get; set; }

    private void Awake()
    {
        player.kek += Talk;
    }

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
        if (IsPlayerNear)
            Debug.Log("Are you trying to speak with me?");
    }

    public void PlayerEnter(bool flag)
    {
        IsPlayerNear = flag;
    }
}