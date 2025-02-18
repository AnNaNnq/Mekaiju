using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class KaijuAttakContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<KaijuAttackNodeData> NodeData = new List<KaijuAttackNodeData>();
}
