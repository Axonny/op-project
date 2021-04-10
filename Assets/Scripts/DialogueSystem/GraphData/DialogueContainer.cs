using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.GraphData
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public EntryPointNodeData entryPoint;
        [SerializeField] private List<DialogueNodeData> dialogueNodes = new List<DialogueNodeData>();
        [SerializeField] private List<EventNodeData> eventNodes = new List<EventNodeData>();
        
        public List<BaseNodeData> Nodes => dialogueNodes.Cast<BaseNodeData>().Union(eventNodes).ToList();

        public void Add(BaseNode node)
        {
            switch (node)
            {
                case DialogueNode dialogueNode:
                    AddDialogueNodeData(dialogueNode);
                    break;
                case EventNode eventNode:
                    AddEventNodeData(eventNode);
                    break;
                default:
                    if(node.EntryPoint)
                        AddEntryPoint(node);
                    else
                        throw new ArgumentException("Unexpected nodeData");
                    break;
            }
        }

        private void AddEntryPoint(BaseNode node)
        {
            var outputPort = (Port) node.outputContainer[0];
            var targetNode = (BaseNode) outputPort.connections.First().input.node;
            entryPoint = new EntryPointNodeData
            {
                guid = node.Guid, 
                targetNodeGuid = targetNode.Guid
            };
        }

        private void AddEventNodeData(EventNode node)
        {
            var data = new EventNodeData { dataEvent = node.DataEvent};
            AddBaseData(node, data);
            eventNodes.Add(data);
        }

        private void AddDialogueNodeData(DialogueNode node)
        {
            var data = new DialogueNodeData { dialogueText = node.DialogueText };
            AddBaseData(node, data);
            dialogueNodes.Add(data);
        }

        private void AddBaseData(BaseNode node, BaseNodeData toData)
        {
            toData.guid = node.Guid;
            toData.position = node.GetPosition().position;
            
            for (var i = 0; i < node.outputContainer.childCount; i++)
            {
                var outputPort = (Port) node.outputContainer[i];
                var linkData = new NodeLinkData
                {
                    portName = outputPort.portName
                };
                if (outputPort.connected)
                {
                    var targetNode = (BaseNode) outputPort.connections.First().input.node;
                    linkData.targetNodeGuid = targetNode.Guid;
                }
                toData.outputPorts.Add(linkData);
            }
        }
    }
}