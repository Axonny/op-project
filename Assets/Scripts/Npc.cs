using System;
using Interfaces;
using UnityEngine;

public class Npc : MonoBehaviour, INpc
{
    private void Awake()
    {
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
        Debug.Log("Are you trying to speak with me?");
    }

    public void PlayerEnter(bool flag)
    {
        if (flag)
        {
            Player.Instance.kek += Talk;
        }
        else
        {
            Player.Instance.kek -= Talk;
        }
    }
}