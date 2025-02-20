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

        [SerializeField]
        private KaijuAttack[] _allAttacks;

        private void Start()
        {
            _instance = GetComponent<KaijuInstance>();
            _attackGraph = _instance.attackGraph;
            _allAttacks = LoadAllAttacks();
        }

        public void StarFight()
        {
            string t_GUID = GetGUIDStartWithName("Start");
            
            List<string> t_startAttack = GetNextNodes("b5a74a81-edc1-4018-95f2-606b6fc917c3");
            Attack(t_startAttack);
        }

        public string GetGUIDStartWithName(string p_name)
        {
            return _attackGraph.StartNodeData
            .FirstOrDefault(attack => attack.name == p_name)?.guid;
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

                    var t_match = _attackGraph.NodeData.FirstOrDefault(item => item.guid == t_target);

                    if (t_match != null)
                    {
                        t_nodesGUID.Add(t_match.guid);
                    }
                }
            }
            return t_nodesGUID;
        }

        public List<KaijuAttack> PotentialAttacks(List<string> p_attacksGUID)
        {
            List<KaijuAttack> t_kaijuAttacks = new List<KaijuAttack>();
            foreach(string attack in p_attacksGUID)
            {
                t_kaijuAttacks.Add(GetAttack(GetNameWithGUID(attack)));
            }

            return t_kaijuAttacks;
        }

        public string GetNameWithGUID(string p_guid)
        {
            return _attackGraph.NodeData.FirstOrDefault(attack => attack.guid == p_guid)?.name;
        }

        public KaijuAttack[] LoadAllAttacks()
        {
            // Charger tous les ScriptableObjects de type Attack dans le dossier Resources/Kaijus/Attacks
            KaijuAttack[] t_attacks = Resources.LoadAll<KaijuAttack>("Kaijus/Attacks");

            if (t_attacks.Length == 0)
            {
                Debug.LogWarning("Aucun attack ScriptableObject trouvé dans Resources/Kaijus/Attacks.");
            }

            return t_attacks;
        }

        KaijuAttack GetAttack(string p_attackName)
        {
            return _allAttacks.FirstOrDefault(attack => attack.Name == p_attackName);
        }

        public void Attack(List<string> p_attackGUID)
        {
            List<KaijuAttack> t_kaijuAttacks = PotentialAttacks(p_attackGUID);
            Debug.Log(t_kaijuAttacks.Count());
            for (int i = 0; i < t_kaijuAttacks.Count; i++)
            {
                Debug.Log(i);
                if (i < t_kaijuAttacks.Count - 1)
                {
                    Debug.Log("First");
                }
                else
                {
                    Debug.Log("Last");
                }
            }

            //string t_name = _attackGraph.NodeData.FirstOrDefault(attack => attack.GUID == p_attackGUID)?.Name;

            //KaijuAttack t_attack = GetAttack(t_name);
            //Debug.Log(t_attack.Attack.CanUse(_instance).ToString());
        }

    }
}
