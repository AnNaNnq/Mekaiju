using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

public class KaijuAttackGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150, 200);

    public KaijuAttackGraphView()
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

        AddElement(GenerateEntryPointNode());
    }

    private Port GeneratePort(KaijuAttackNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private KaijuAttackNode GenerateEntryPointNode()
    {
        var t_node = new KaijuAttackNode
        {
            title = "Start",
            GUID = Guid.NewGuid().ToString(),
            Description = "Start Node",
            EntryPoint = true
        };

        var t_generatedPort = GeneratePort(t_node, Direction.Output);
        t_generatedPort.portName = "Next";
        t_node.outputContainer.Add(t_generatedPort);

        t_node.RefreshExpandedState();
        t_node.RefreshPorts();

        t_node.SetPosition(new Rect(100, 200, 100, 150));

        return t_node;
    }

    public void CreateNode(string p_nodeName)
    {
        AddElement(CreateKaijuAttackNode(p_nodeName));
    }

    public KaijuAttackNode CreateKaijuAttackNode(string p_nodeName)
    {
        var t_node = new KaijuAttackNode
        {
            title = p_nodeName,
            Description = p_nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var t_inputPort = GeneratePort(t_node, Direction.Input, Port.Capacity.Multi);
        t_inputPort.portName = "Input";

        var t_button = new Button(() => { AddChoicePort(t_node); });
        t_button.text = "New Attack";
        t_node.titleContainer.Add(t_button);

        t_node.inputContainer.Add(t_inputPort);
        t_node.RefreshExpandedState();
        t_node.RefreshPorts();
        t_node.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return t_node;
    }

    private void AddChoicePort(KaijuAttackNode p_node)
    {
        var t_generatedPort = GeneratePort(p_node, Direction.Output);

        var t_outputPortCount = p_node.outputContainer.Query("connector").ToList().Count;
        t_generatedPort.portName = $"Option {t_outputPortCount}";

        p_node.outputContainer.Add(t_generatedPort);
        p_node.RefreshPorts();
        p_node.RefreshExpandedState();
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
