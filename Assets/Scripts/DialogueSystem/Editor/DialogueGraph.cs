using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class DialogueGraph : GraphView
    {
        public readonly Vector2 DefaultSize = new Vector2(150, 200);
        private NodeSearchWindow searchWindow;
        
        public DialogueGraph(EditorWindow editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Dialogue"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var grid = new GridBackground();
            Insert( 0, grid);
            grid.StretchToParentSize();
            
            
            AddElement(GenerateEntryPoint());
            AddSearchWindow(editorWindow);
        }

        private void AddSearchWindow(EditorWindow window)
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Init(window, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts;
        }

        private Port GeneratePort(Node node, Direction portDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(int));
        }

        private BaseNode GenerateEntryPoint()
        {
            var node = new BaseNode
            {
                title = "Start",
                Guid = Guid.NewGuid().ToString(),
                EntryPoint = true
            };

            var port = GeneratePort(node, Direction.Output);
            port.portName = "Next";
            node.outputContainer.Add(port);
            
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;
            
            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        public DialogueNode CreateDialogueNode(string nodeName, Vector2 position)
        {
            var node = new DialogueNode
            {
                title = nodeName,
                Guid = Guid.NewGuid().ToString(),
                DialogueText = nodeName
            };

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            
            var nodeCreateOutputButton = new Button(() => { AddChoicePort(node); })
            {
                text = "New Choice"
            };
            node.titleContainer.Add(nodeCreateOutputButton);
            
            
            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                node.DialogueText = evt.newValue;
                node.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.title);

            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            node.inputContainer.Add(inputPort);
            node.mainContainer.Add(textField);
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            node.SetPosition(new Rect(position, DefaultSize));

            return node;
        }

        public EventNode CreateEventNode(Vector2 position, string dataEvent = "")
        {
            var node = new EventNode
            {
                title = "Unity Event",
                Guid = Guid.NewGuid().ToString(),
                DataEvent = dataEvent
            };

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            
            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                node.DataEvent = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.DataEvent);
            node.mainContainer.Add(textField);

            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            node.inputContainer.Add(inputPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            node.SetPosition(new Rect(position, DefaultSize));

            return node;
        }

        public void CreateNode(string nodeName, Vector2 position)
        {
            AddElement(CreateDialogueNode(nodeName, position));
        }

        public void AddChoicePort(BaseNode node, string overriddenPortName = "")
        {
            var outputPort = GeneratePort(node, Direction.Output);

            var oldLabel = outputPort.contentContainer.Q<Label>("type");
            outputPort.contentContainer.Remove(oldLabel);
            
            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
            
            var choicePortName = string.IsNullOrEmpty(overriddenPortName) 
                ? $"Choice {outputPortCount + 1}" 
                : overriddenPortName;
            outputPort.portName = choicePortName;
            
            var textFiled = new TextField
            {
                name = string.Empty,
                value = choicePortName
            };

            textFiled.RegisterValueChangedCallback(evt => outputPort.portName = evt.newValue);
            outputPort.contentContainer.Add(new Label("  "));
            outputPort.contentContainer.Add(textFiled);

            var deleteButton = new Button(() => RemovePort(node, outputPort))
            {
                text = "X",
            };
            outputPort.contentContainer.Add(deleteButton);
            
            node.outputContainer.Add(outputPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        private void RemovePort(Node node, Port outputPort)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == outputPort.portName && x.output.node == outputPort.node)
                .ToArray();

            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(outputPort);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }
    }
}
