using DialogueSystem.GraphData;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/level data", order = 0)]
    public class LevelManager : ScriptableObject
    {
        public int index;
        public string[] levels;
        public DialogueData[] dialogues;

        public string NextLevel => levels[index];
        public DialogueData Dialogue => dialogues[index];

        public void ClearData()
        {
            index = 0;
        }
    }
}