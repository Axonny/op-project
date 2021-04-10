using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class DialogueWindow : EditorWindow
    {
        private DialogueGraph graph;
        private string fileName = "New Dialogue";
        
        
        [MenuItem("Window/Graph/Dialogue")]
        public static void OpenWindow()
        {
            var window = GetWindow<DialogueWindow>();
            window.titleContent = new GUIContent("Dialogue graph");
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            
            var nodeFileName = new TextField("File Name:");
            nodeFileName.SetValueWithoutNotify(fileName);
            nodeFileName.MarkDirtyRepaint();
            nodeFileName.RegisterValueChangedCallback(evt =>
            {
                fileName = evt.newValue;
            });
            toolbar.Add(nodeFileName);
            
            toolbar.Add(new Button(() => RequestDataOperation(true))
            {
                text="Save Data"
            });
            toolbar.Add(new Button(() => RequestDataOperation(false))
            {
                text="Load Data"
            });
            toolbar.Add(new Button(() => GraphSaveUtility.GetInstance(graph).ClearGraph())
            {
                text="Clear graph"
            });

            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name! ", "Please enter a valid file name", "OK");
                return;
            }

            var saveUtility = GraphSaveUtility.GetInstance(graph);
            if (save)
                saveUtility.SaveGraph(fileName);
            else
                saveUtility.LoadGraph(fileName);
        }

        private void ConstructGraph()
        {
            graph = new DialogueGraph(this)
            {
                name = "Dialogue graph"
            };
            graph.StretchToParentSize();
            rootVisualElement.Add(graph);
        }

        private void OnEnable()
        {
            ConstructGraph();
            GenerateToolbar();
            GenerateMiniMap();
        }
        

        private void GenerateMiniMap()
        {
            var minimap = new MiniMap()
            {
                anchored = true,
            };

            minimap.SetPosition(new Rect(10, 30, 200, 140));
            graph.Add(minimap);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graph);
        }
    }
}
