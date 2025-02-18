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
    private KaijuAttackContainer _containerCache;

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
        if (!Edges.Any()) return;

        var t_kaijuAttackContainer = ScriptableObject.CreateInstance<KaijuAttackContainer>();

        // Sauvegarde des connexions entre les nodes
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

        // Sauvegarde des nodes normales
        foreach (var node in Nodes.Where(node => !node.EntryPoint))
        {
            t_kaijuAttackContainer.NodeData.Add(new KaijuAttackNodeData()
            {
                GUID = node.GUID,
                Name = node.Name,
                Position = node.GetPosition().position
            });
        }

        // Sauvegarde des nodes de départ (GUID + Position)
        var entryPointNodes = Nodes.Where(node => node.EntryPoint).ToList();
        foreach (var entryPointNode in entryPointNodes)
        {
            t_kaijuAttackContainer.EntryPointNodeGUIDs.Add(entryPointNode.GUID);
            t_kaijuAttackContainer.EntryPointNodePositions.Add(entryPointNode.GetPosition().position);
        }

        AssetDatabase.CreateAsset(t_kaijuAttackContainer, $"Assets/Resources/Kaijus/AttackGraph/{p_fileName}.asset");
        AssetDatabase.SaveAssets();
    }




    public void LoadGraph(string p_fileName)
    {
        _containerCache = Resources.Load<KaijuAttackContainer>("Kaijus/AttackGraph/" + p_fileName);
        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist!", "OK");
            return;
        }

        ClearGraph();

        var savedEntryCount = _containerCache.EntryPointNodeGUIDs.Count;
        var entryPointNodes = Nodes.Where(node => node.EntryPoint).ToList();
        int currentEntryCount = entryPointNodes.Count;

        // Supprimer les nodes de départ en trop
        if (currentEntryCount > savedEntryCount)
        {
            var extraNodes = entryPointNodes.Skip(savedEntryCount).ToList();
            foreach (var extraNode in extraNodes)
            {
                // Supprimer les connexions des nodes supprimées
                Edges.Where(x => x.input.node == extraNode || x.output.node == extraNode).ToList()
                    .ForEach(edge => _targetGraphView.RemoveElement(edge));

                _targetGraphView.RemoveElement(extraNode);
            }
            entryPointNodes = entryPointNodes.Take(savedEntryCount).ToList();
        }

        // Ajouter les nodes de départ manquantes
        else if (currentEntryCount < savedEntryCount)
        {
            int missingNodes = savedEntryCount - currentEntryCount;
            for (int i = 0; i < missingNodes; i++)
            {
                var newEntryNode = _targetGraphView.CreateStartNode();
                _targetGraphView.AddElement(newEntryNode);
                entryPointNodes.Add(newEntryNode);
            }
        }

        // Mettre à jour les GUID et les positions des nodes de départ
        for (int i = 0; i < savedEntryCount; i++)
        {
            entryPointNodes[i].GUID = _containerCache.EntryPointNodeGUIDs[i];
            entryPointNodes[i].SetPosition(new Rect(_containerCache.EntryPointNodePositions[i], _targetGraphView.defaultNodeSize));
        }

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
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

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
            var t_node = _targetGraphView.CreateKaijuAttackNode(nodeData.Name, Vector2.zero);
            t_node.GUID = nodeData.GUID;
            _targetGraphView.AddElement(t_node);

            var t_nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.GUID).ToList();
            t_nodePorts.ForEach(x => _targetGraphView.AddLinkPort(t_node, x.PortName));
        }
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGUID;

        foreach (var node in Nodes)
        {
            if (node.EntryPoint) continue;

            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}