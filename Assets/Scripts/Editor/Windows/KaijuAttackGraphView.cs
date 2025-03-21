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

        CreateStart();

        AddSearchWindow(editorWindow);
    }


    public KaijuAttackNode CreateStartNode(Vector2 p_pos, string p_name = "Start")
    {
        var startNode = GenerateEntryPointNode(p_pos, p_name);
        AddElement(startNode); // Ajoute le n�ud � la vue
        return startNode;
    }

    public KaijuAttackNode CreateStart()
    {
        var startNode = GenerateFirstNode();
        AddElement(startNode); // Ajoute le n�ud � la vue
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
        t_generatedPort.portName = "Attack 1";
        t_node.outputContainer.Add(t_generatedPort);

        var t_button2 = new Button(() => { SwitchStartNodeToNode(t_node); });
        t_button2.text = "Node";
        t_node.titleContainer.Add(t_button2);

        var t_button = new Button(() => { AddLinkPort(t_node); });
        t_button.text = "New Attack";
        t_node.titleContainer.Add(t_button);

        t_node.RefreshExpandedState();
        t_node.RefreshPorts();

        t_node.SetPosition(new Rect(p_pos, new Vector2(100, 150)));

        return t_node;
    }

    private KaijuAttackNode GenerateFirstNode()
    {
        var t_node = new KaijuAttackNode
        {
            title = "Start",
            GUID = Guid.NewGuid().ToString(),
            Name = "Start",
            EntryPoint = true
        };

        var t_generatedPort = GeneratePort(t_node, Direction.Output);
        t_generatedPort.portName = "Next";
        t_node.outputContainer.Add(t_generatedPort);

        t_node.capabilities &= ~Capabilities.Movable;
        t_node.capabilities &= ~Capabilities.Deletable;

        t_node.RefreshExpandedState();
        t_node.RefreshPorts();

        t_node.SetPosition(new Rect(new Vector2(0, 0), new Vector2(100, 150)));

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

        var t_button2 = new Button(() => { SwitchNodeToStartNode(t_node); });
        t_button2.text = "Start Node";
        t_node.titleContainer.Add(t_button2);

        var t_button = new Button(() => { AddLinkPort(t_node); });
        t_button.text = "New Attack";
        t_node.titleContainer.Add(t_button);


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
        textField.SetEnabled(false);
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

    private void SwitchNodeToStartNode(KaijuAttackNode node)
    {
        string t_name = node.title;
        Vector2 t_pos = node.GetPosition().position;
        DeleteKaijuAttackNode(node);
        CreateStartNode(t_pos, t_name);
    }

    private void SwitchStartNodeToNode(KaijuAttackNode node)
    {
        string t_name = node.title;
        Vector2 t_pos = node.GetPosition().position;
        DeleteKaijuAttackNode(node);
        CreateNode(t_name, t_pos);
    }

    public void RemovePort(Node node, Port socket)
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

    public void DeleteKaijuAttackNode(KaijuAttackNode node)
    {
        if (node == null)
            return;

        // Supprime toutes les connexions aux autres nodes
        foreach (var port in node.inputContainer.Children())
        {
            if (port is Port inputPort)
            {
                foreach (var edge in inputPort.connections.ToList())
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    edge.RemoveFromHierarchy();
                }
            }
        }

        foreach (var port in node.outputContainer.Children())
        {
            if (port is Port outputPort)
            {
                foreach (var edge in outputPort.connections.ToList())
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    edge.RemoveFromHierarchy();
                }
            }
        }

        // Supprime le n�ud de la vue
        node.RemoveFromHierarchy();
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
