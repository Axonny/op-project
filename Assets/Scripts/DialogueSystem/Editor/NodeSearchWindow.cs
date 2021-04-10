using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraph graph;
        private EditorWindow window;
        private Texture2D indentationIcon;

        public void Init(EditorWindow dialogueWindow, DialogueGraph dialogueGraph)
        {
            graph = dialogueGraph;
            window = dialogueWindow;
            
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0,0,0, 0));
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeEntry(new GUIContent("Dialogue Node", indentationIcon))
                {
                    userData = new DialogueNode(),
                    level = 1,
                },
                new SearchTreeEntry(new GUIContent("Event Node", indentationIcon))
                {
                    userData = new EventNode(),
                    level = 1,
                },
            };
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition =
                window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
                    context.screenMousePosition - window.position.position);
            var localMousePosition = graph.contentViewContainer.WorldToLocal(worldMousePosition);
            switch (searchTreeEntry.userData)
            {
                case DialogueNode _:
                    graph.CreateNode("Dialogue Node", localMousePosition);
                    return true;
                case EventNode _:
                    var eventNode = graph.CreateEventNode(localMousePosition);
                    graph.AddElement(eventNode);
                    return true;
                default:
                    return false;
            }
        }
    }
}