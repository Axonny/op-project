using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Editor.Nodes;
using DialogueSystem.GraphData;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private DialogueGraph targetGraph;
        private DialogueContainer containerCache;
        private List<Edge> Edges => targetGraph.edges.ToList();
        private List<BaseNode> Nodes => targetGraph.nodes.ToList().Cast<BaseNode>().ToList();

        public static GraphSaveUtility GetInstance(DialogueGraph graph)
        {
            return new GraphSaveUtility
            {
                targetGraph = graph
            };
        }

        public void SaveGraph(string fileName)
        {
            if (!Edges.Any()) return;
            var dialogueContainer = new DialogueContainer(ScriptableObject.CreateInstance<DialogueData>());

            foreach (var node in Nodes)
            {
                dialogueContainer.Add(node);
            }

            AssetDatabase.CreateAsset(dialogueContainer.Container, $"Assets/Resources/Dialogue Data/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            containerCache = new DialogueContainer(Resources.Load<DialogueData>($"Dialogue Data/{fileName}"));
                
            if (containerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target dialogue file doesn't exists!", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }

        private void ConnectNodes()
        {
            foreach (var node in Nodes.Where(x => !x.EntryPoint))
            {
                var connections = containerCache.Container.Nodes
                    .First(x => x.guid == node.Guid)
                    .outputPorts
                    .Where(x => !string.IsNullOrEmpty(x.targetNodeGuid));
                
                foreach (var connect in connections)
                {
                    var outputPort = (Port) node.outputContainer.Children()
                        .First(x => ((Port) x).portName == connect.portName);

                    var targetNode = Nodes.First(x => x.Guid == connect.targetNodeGuid);
                    var inputPort = (Port) targetNode.inputContainer[0];
                    LinkNodes(outputPort, inputPort);
                }
            }

            var outputEntryPoint = (Port) Nodes.First(x => x.EntryPoint).outputContainer[0];
            var inputEntryPoint = (Port) Nodes
                .First(x => x.Guid == containerCache.Container.entryPoint.targetNodeGuid)
                .inputContainer[0];
            LinkNodes(outputEntryPoint, inputEntryPoint);
        }

        private void LinkNodes(Port output, Port input)
        {
            var tmpEdge = new Edge
            {
                output = output,
                input = input
            };

            tmpEdge.input.Connect(tmpEdge);
            tmpEdge.output.Connect(tmpEdge);
            targetGraph.Add(tmpEdge);
        }

        private void CreateNodes()
        {
            Nodes.First().Guid = containerCache.Container.entryPoint.guid;
            
            foreach (var nodeData in containerCache.Container.Nodes)
            {
                BaseNode tmpNode;
                switch (nodeData)
                {
                    case DialogueNodeData dialogueNodeData:
                        tmpNode = targetGraph.CreateDialogueNode(dialogueNodeData.dialogueText, Vector2.zero);
                        break;
                    default:
                        throw new System.ArgumentException("Unexpected nodeData");
                }
                
                tmpNode.Guid = nodeData.guid;
                targetGraph.AddElement(tmpNode);
                tmpNode.SetPosition(new Rect(
                    nodeData.position,
                    targetGraph.DefaultSize
                ));
                nodeData.outputPorts.ForEach(x => targetGraph.AddChoicePort(tmpNode, x.portName));
            }
        }

        public void ClearGraph()
        {
            foreach (var node in Nodes.Where(node => !node.EntryPoint))
            {
                Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => targetGraph.RemoveElement(edge));
                
                targetGraph.RemoveElement(node);
            }
        }
    }
}