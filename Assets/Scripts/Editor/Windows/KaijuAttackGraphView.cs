using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class KaijuAttackGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    
    private NodeSearchWindow _searchWindow;

    public KaijuAttackGraphView(EditorWindow editorWindow)
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        styleSheets.Add(Resources.Load<StyleSheet>("Styles/KaijuAttackGraph"));

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var p_grid = new GridBackground();
        Insert(0, p_grid);
        p_grid.StretchToParentSize();
        p_grid.AddToClassList("grid-background");

        CreateStartNode(Vector2.zero);

        AddSearchWindow(editorWindow);
    }


    public KaijuAttackNode CreateStartNode(Vector2 p_pos, string p_name = "Start")
    {
        var startNode = GenerateEntryPointNode(p_pos, p_name);
        AddElement(startNode); // Ajoute le nœud à la vue
        return startNode;
    }


    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();

        _searchWindow.Init(editorWindow, this);

        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    private void AddStartNodeButton()
    {
        var button = new Button(() => CreateNode("Start", new Vector2(200, 200)))
        {
            text = "Add Start Node"
        };
        this.Add(button);
    }

    private Port GeneratePort(KaijuAttackNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private KaijuAttackNode GenerateEntryPointNode(Vector2 p_pos, string p_name)
    {
        var t_node = new KaijuAttackNode
        {
            title = p_name,
            GUID = Guid.NewGuid().ToString(),
            Name = "Start Node",
            EntryPoint = true
        };

        var t_generatedPort = GeneratePort(t_node, Direction.Output);
        t_generatedPort.portName = "Next";
        t_node.outputContainer.Add(t_generatedPort);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            t_node.Name = evt.newValue;
            t_node.title = evt.newValue;
        });

        textField.SetValueWithoutNotify(t_node.title);
        t_node.mainContainer.Add(textField);

        t_node.RefreshExpandedState();
        t_node.RefreshPorts();

        t_node.SetPosition(new Rect(p_pos, new Vector2(100, 150)));

        return t_node;
    }

    public void CreateNode(string p_nodeName, Vector2 position)
    {
        AddElement(CreateKaijuAttackNode(p_nodeName, position));
    }

    public KaijuAttackNode CreateKaijuAttackNode(string p_nodeName, Vector2 position)
    {
        var t_node = new KaijuAttackNode
        {
            title = p_nodeName,
            Name = p_nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var t_inputPort = GeneratePort(t_node, Direction.Input, Port.Capacity.Multi);
        t_inputPort.portName = "Input";
        t_node.inputContainer.Add(t_inputPort);

        t_node.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Node"));
        t_node.AddToClassList("DialogueNode");

        var t_button = new Button(() => { AddLinkPort(t_node); });
        t_button.text = "New Attack";
        t_node.titleContainer.Add(t_button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            t_node.Name = evt.newValue;
            t_node.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(t_node.title);
        t_node.mainContainer.Add(textField);

        t_node.RefreshExpandedState();
        t_node.RefreshPorts();
        t_node.SetPosition(new Rect(position, defaultNodeSize));

        return t_node;
    }

    public void AddLinkPort(KaijuAttackNode p_node, string p_portName = "")
    {
        var t_generatedPort = GeneratePort(p_node, Direction.Output);

        var t_oldLabel = t_generatedPort.contentContainer.Q<Label>("type");
        t_generatedPort.contentContainer.Remove(t_oldLabel);

        var t_outputPortCount = p_node.outputContainer.Query("connector").ToList().Count;

        var t_portName = string.IsNullOrEmpty(p_portName) ? $"Attack {t_outputPortCount + 1}" : p_portName;

        var textField = new TextField()
        {
            name = string.Empty,
            value = t_portName
        };
        textField.RegisterValueChangedCallback(evt => t_generatedPort.portName = evt.newValue);
        t_generatedPort.contentContainer.Add(new Label(" "));
        t_generatedPort.contentContainer.Add(textField);
        var t_deleteButton = new Button(() => RemovePort(p_node, t_generatedPort)) 
        { 
            text = "x"
        };
        t_generatedPort.contentContainer.Add(t_deleteButton);

        t_generatedPort.portName = t_portName;

        p_node.outputContainer.Add(t_generatedPort);
        p_node.RefreshPorts();
        p_node.RefreshExpandedState();
    }

    private void RemovePort(Node node, Port socket)
    {
        var targetEdge = edges.ToList()
            .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        node.outputContainer.Remove(socket);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var t_ports = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                t_ports.Add(port);
            }
        });
        return t_ports;
    }
}
