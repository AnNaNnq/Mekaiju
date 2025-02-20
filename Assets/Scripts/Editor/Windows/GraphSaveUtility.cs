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

    public static KaijuAttackGraph GetActiveGraphView()
    {
        // Récupère la fenêtre active de l'éditeur
        var window = EditorWindow.focusedWindow;

        // Vérifie si la fenêtre active est de type KaijuAttackGraphView
        if (window is KaijuAttackGraph graphView)
        {
            return graphView;
        }

        Debug.LogWarning("No active KaijuAttackGraphView found.");
        return null;
    }

    public static void SaveCurrentGraph()
    {
        var graphView = GetActiveGraphView();
        if (graphView != null)
        {
            //GetInstance(graphView.GetGraph()).SaveGraph(graphView.name);
            Debug.Log(graphView.name);
        }
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
                guid = node.GUID,
                name = node.Name,
                position = node.GetPosition().position
            });
        }

        // Sauvegarde des nodes de départ (GUID + Position)
        var entryPointNodes = Nodes.Where(node => node.EntryPoint).ToList();
        foreach (var entryPointNode in entryPointNodes)
        {
            t_kaijuAttackContainer.StartNodeData.Add(new KaijuAttackNodeData()
            {
                guid = entryPointNode.GUID,
                name = entryPointNode.title,
                position = entryPointNode.GetPosition().position
            });
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
        AddAndDeleteStartNod();
        CreateNode();
        ConnectNode();
    }

    public void AddAndDeleteStartNod()
    {
        var savedEntryCount = _containerCache.StartNodeData.Count;
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
                var newEntryNode = _targetGraphView.CreateStartNode(_containerCache.StartNodeData[i].position, _containerCache.StartNodeData[i].name);
                _targetGraphView.AddElement(newEntryNode);
                entryPointNodes.Add(newEntryNode);
            }
        }

        // Mettre à jour les GUID, les positions et le nom des nodes de départ
        for (int i = 0; i < savedEntryCount; i++)
        {
            entryPointNodes[i].GUID = _containerCache.StartNodeData[i].guid;
            entryPointNodes[i].SetPosition(new Rect(
                _containerCache.StartNodeData[i].position,
                _targetGraphView.defaultNodeSize
            ));

            // Mise à jour du nom affiché
            entryPointNodes[i].Name = _containerCache.StartNodeData[i].name;
            entryPointNodes[i].title = _containerCache.StartNodeData[i].name;

            // Trouver le champ texte existant et mettre à jour sa valeur
            var textField = entryPointNodes[i].mainContainer.Q<TextField>();
            if (textField != null)
            {
                textField.SetValueWithoutNotify(_containerCache.StartNodeData[i].name);
            }

            var t_nodePorts = _containerCache.NodeLinks
                .Where(x => x.BaseNodeGUID == entryPointNodes[i].GUID)
                .Skip(1)
                .ToList();

            t_nodePorts.ForEach(x => _targetGraphView.AddLinkPort(entryPointNodes[i], x.PortName));
        }
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
                    _containerCache.NodeData.First(x => x.guid == targetNodeGuid).position,
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
            var t_node = _targetGraphView.CreateKaijuAttackNode(nodeData.name, Vector2.zero);
            t_node.GUID = nodeData.guid;
            _targetGraphView.AddElement(t_node);

            var t_nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.guid).ToList();
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