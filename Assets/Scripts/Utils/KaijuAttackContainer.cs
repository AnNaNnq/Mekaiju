using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class KaijuAttackContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<KaijuAttackNodeData> NodeData = new List<KaijuAttackNodeData>();
    public List<KaijuAttackNodeData> StartNodeData = new List<KaijuAttackNodeData>();
}
