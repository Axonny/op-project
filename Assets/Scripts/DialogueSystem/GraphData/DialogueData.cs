using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogueSystem.GraphData
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 0)]
    public class DialogueData : ScriptableObject
    {
        public EntryPointNodeData entryPoint;
        public List<DialogueNodeData> dialogueNodes = new List<DialogueNodeData>();
        public List<EventNodeData> eventNodes = new List<EventNodeData>();
        
        public List<BaseNodeData> Nodes => dialogueNodes.Cast<BaseNodeData>().Union(eventNodes).ToList();

    }
}