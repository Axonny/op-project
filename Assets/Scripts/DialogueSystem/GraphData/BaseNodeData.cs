using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.GraphData
{
    [Serializable]
    public class BaseNodeData
    {
        public string guid;
        public Vector2 position;
        public List<NodeLinkData> outputPorts = new List<NodeLinkData>();
    }

    [Serializable]
    public class EntryPointNodeData
    {
        public string guid;
        public string targetNodeGuid;
    }
}