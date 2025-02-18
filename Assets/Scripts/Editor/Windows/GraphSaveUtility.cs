using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEditor;
using System;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private KaijuAttackGraphView _targetGraphView;
    private KaijuAttakContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<KaijuAttackNode> Nodes => _targetGraphView.nodes.ToList().Cast<KaijuAttackNode>().ToList();

    public static GraphSaveUtility GetInstance(KaijuAttackGraphView p_targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = p_targetGraphView
        };
    }

    public void SaveGraph(string p_fileName)
    {
        if(!Edges.Any()) return;
        
        var t_kaijuAttackContainer = ScriptableObject.CreateInstance<KaijuAttakContainer>();

        var t_connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (var i = 0; i < t_connectedPorts.Length; i++)
        {
            var t_outputNode = t_connectedPorts[i].output.node as KaijuAttackNode;
            var t_inputNode = t_connectedPorts[i].input.node as KaijuAttackNode;

            t_kaijuAttackContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGUID = t_outputNode.GUID,
                PortName = t_connectedPorts[i].output.portName,
                TargetNodeGUID = t_inputNode.GUID
            });
        }

        foreach(var node in Nodes.Where(node => !node.EntryPoint))
        {
            t_kaijuAttackContainer.NodeData.Add(new KaijuAttackNodeData()
            {
                GUID = node.GUID,
                Name = node.Name,
                Position = node.GetPosition().position
            });
        }

        AssetDatabase.CreateAsset(t_kaijuAttackContainer, $"Assets/Resources/Kaijus/{p_fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string p_fileName)
    {
        _containerCache = Resources.Load<KaijuAttakContainer>("Kaijus/" + p_fileName);
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exists!", "OK");
            return;
        }

        ClearGraph();
        CreateNode();
        ConnectNode();
    }

    private void ConnectNode()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(
                    _containerCache.NodeData.First(x => x.GUID == targetNodeGuid).Position,
                    _targetGraphView.defaultNodeSize
                ));
            }
        }
    }

    private void LinkNodes(Port p_output, Port p_input)
    {
        var t_edge = new Edge
        {
            output = p_output,
            input = p_input,
        };

        t_edge?.input.Connect(t_edge);
        t_edge?.output.Connect(t_edge);
        _targetGraphView.Add(t_edge);
    }

    private void CreateNode()
    {
        foreach (var nodeData in _containerCache.NodeData)
        {
            var t_node = _targetGraphView.CreateKaijuAttackNode(nodeData.Name);
            t_node.GUID = nodeData.GUID;
            _targetGraphView.AddElement(t_node);

            var t_nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.GUID).ToList();
            t_nodePorts.ForEach(x => _targetGraphView.AddLinkPort(t_node, x.PortName));
        }
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGUID;

        foreach(var node in Nodes)
        {
            if (node.EntryPoint) continue;

            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}