using Mekaiju.AI.Objet;
using Mekaiju.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mekaiju.AI
{
    public class KaijuBrain : MonoBehaviour
    {
        KaijuInstance _instance;

        private KaijuAttackContainer _attackGraph;
        private KaijuMotor _motor;
        [Header("Pas touche c'est juste du debug")]
        [SerializeField]
        private string _lastAttack;

        [SerializeField]
        private bool _canAttack = true;

        public KaijuAttack[] allAttacks { get { return _allAttacks; } }
        private KaijuAttack[] _allAttacks;

        private void Start()
        {
            _motor = GetComponent<KaijuMotor>();
            _instance = GetComponent<KaijuInstance>();
            _attackGraph = _instance.GetGraph();
            _allAttacks = LoadAllAttacks();
            _lastAttack = "Start";
            StartCoroutine(CheckAttack());
            foreach(KaijuAttack attack in _allAttacks)
            {
                attack.attack.Init();
            }
        }

        public void StarFight()
        {
            _attackGraph = _instance.GetGraph();
            string t_GUID = GetGUIDStartWithName(_lastAttack);
            
            List<string> t_startAttack = GetNextNodes(t_GUID);
            
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
            // Charger tous les ScriptableObjects de type Attack dans Resources/Kaijus/Attacks
            KaijuAttack[] t_attacks = Resources.LoadAll<KaijuAttack>("Kaijus/Attacks");

            if (t_attacks == null || t_attacks.Length == 0)
            {
                Debug.LogWarning("Aucun attack ScriptableObject trouvé dans Resources/Kaijus/Attacks");
            }

            return t_attacks;
        }

        KaijuAttack GetAttack(string p_attackName)
        {
            foreach(KaijuAttack attack in _allAttacks)
            {
                if (p_attackName.Equals(attack.name)) return attack;
            }
            return null;
        }

        public void Attack(List<string> p_attackGUID)
        {
            List<KaijuPassive> t_activePassives = _instance.GetPassivesActive();
            if (t_activePassives.Count > 0)
            {
                foreach (KaijuPassive passive in t_activePassives)
                {
                    passive.passive.Run(_instance);
                }
                _canAttack = false;
                return;
            }

            if (!_canAttack)
            {
                if (_motor.agent.enabled == false && !_motor.agent.isOnNavMesh) return;
                _motor.MoveTo(_instance.target.transform.position, 100);
                return;
            }


            List<KaijuAttack> t_kaijuAttacks = PotentialAttacks(p_attackGUID);

            bool t_canAttack = false;
            for (int i = 0; i < t_kaijuAttacks.Count; i++)
            {
                if (i < t_kaijuAttacks.Count - 1)
                {
                    t_canAttack = t_kaijuAttacks[i].attack.CanUse(_instance, t_kaijuAttacks[i+1].attack.range);
                }
                else
                {
                    t_canAttack = t_kaijuAttacks[i].attack.CanUse(_instance);
                }

                if (t_canAttack)
                {
                    _motor.agent.ResetPath();
                    _motor.LookTarget();
                    t_kaijuAttacks[i].attack.Active(_instance);
                    _lastAttack = t_kaijuAttacks[i].name;
                    _canAttack = false;
                    return;
                }   
            }

            MakeAction();
            _motor.MoveTo(_instance.target.transform.position, 100);
        }

        public void MakeAction()
        {
            _canAttack = false;
            StartCoroutine(UtilsFunctions.CooldownRoutine(_instance.timeBetweenTowAction, () => { _canAttack = true; }));
        }

        IEnumerator CheckAttack()
        {
            while (true)
            {
                yield return new WaitForSeconds(_instance.timeBetweenTowAction + 1);
                _canAttack = true;
            }
        }
    }
}
