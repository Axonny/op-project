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
        
        public DialogueData Dialogue => dialogues[index];
    }
}