using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(KaijuInstance))]
    public class KaijuBrain : MonoBehaviour
    {
        KaijuInstance _instance;

        private KaijuAttackContainer _attackGraph;
        private string _lastAttack;

        private KaijuAttack[] _allAttacks;

        private void Start()
        {
            _instance = GetComponent<KaijuInstance>();
            _attackGraph = _instance.attackGraph;
            _allAttacks = LoadAllAttacks();
        }

        public void StarFight()
        {
            string t_GUID = _attackGraph.StartNodeData
            .FirstOrDefault(attack => attack.Name == "Start")?.GUID;

            //The first attack necessarily has only one link (not 2 not 0)
            string t_startAttack = GetNextNodes(t_GUID)[0];
            Attack(t_startAttack);
        }

        public List<string> GetNextNodes(string p_NodeGUID)
        {
            List<string> t_nodesGUID = new List<string>();
            if (p_NodeGUID != null)
            {
                foreach (var link in _attackGraph.NodeLinks
                         .Where(link => link.BaseNodeGUID == p_NodeGUID))
                {
                    string t_target = link.TargetNodeGUID;

                    var t_match = _attackGraph.NodeData.FirstOrDefault(item => item.GUID == t_target);

                    if (t_match != null)
                    {
                        t_nodesGUID.Add(t_match.GUID);
                    }
                }
            }
            return t_nodesGUID;
        }

        public KaijuAttack[] LoadAllAttacks()
        {
            // Charger tous les ScriptableObjects de type Attack dans le dossier Resources/Kaijus/Attacks
            KaijuAttack[] attacks = Resources.LoadAll<KaijuAttack>("Kaijus/Attacks");

            if (attacks.Length == 0)
            {
                Debug.LogWarning("Aucun attack ScriptableObject trouvé dans Resources/Kaijus/Attacks.");
            }

            return attacks;
        }

        KaijuAttack GetAttack(string p_attackName)
        {
            return _allAttacks.FirstOrDefault(attack => attack.Name == p_attackName);
        }

        public void Attack(string p_attackName)
        {
            KaijuAttack t_attack = GetAttack(p_attackName);
            t_attack.Attack.Active();
            _lastAttack = p_attackName;
        }

    }
}
