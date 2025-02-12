using Mekaiju.Attacks;
using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class TeneborokAI : BasicAI
    {
        [SerializeField]
        private TeneborokAttack lastAttack;

        #region Coup tranchant
        [Foldout("Coup tranchant")]
        [OverrideLabel("Damage")] public int hitCutDamage = 10;
        [OverrideLabel("Range")] public float hitCutRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int hitCutBody;
        [OverrideLabel("Attack zone center")] public Vector3 hitCutZoneCenter;
        [OverrideLabel("Attack zone size")] public Vector3 hitCutZoneSize;
        #endregion

        #region Vortex Abyssal
        [Foldout("Vortex Abyssal")]
        [OverrideLabel("Damage")] public int vortexDamage = 10;
        [OverrideLabel("Range")] public float vortexRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int vortexBody;
        [OverrideLabel("Gravitational zone prefab")][OpenPrefabButton] public GameObject gameObjectVortex;
        [OverrideLabel("Kaillou prefab")][OpenPrefabButton] public GameObject gameObjectRock;
        [OverrideLabel("Vortex Radius")] public float vortexRadius = 10f;
        [OverrideLabel("Number of rock")] public int vortexNumberOfRock = 10;
        [OverrideLabel("CD")] public float vortexCD = 10f;
        
        private bool _canVortex = true;
        #endregion

        #region Fissure du Néant
        [Foldout("Fissure du Néant")]
        [OverrideLabel("Damage")] public int rimDamage = 10;
        [OverrideLabel("Range")] public float rimRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int rimBody;
        [OverrideLabel("Rim prefab")][OpenPrefabButton] public GameObject gameObjectRim;
        [OverrideLabel("Fire prefab")][OpenPrefabButton] public GameObject gameObjectRimFire;
        [OverrideLabel("CD")] public float rimCD = 10f;

        private bool _canRim = true;
        #endregion

        #region Pour les ld
        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Hit Cut")]
        public bool debugHitCut = false;
        [ConditionalField(nameof(debugHitCut))] public Color colorForHitCutRange;
        [OverrideLabel("Show Gizmo For Vortex")]
        public bool debugVortex = false;
        [ConditionalField(nameof(debugVortex))] public Color colorForVortexRange;
        [OverrideLabel("Show Gizmo For Vortex")]
        public bool debugRim = false;
        [ConditionalField(nameof(debugRim))] public Color colorForRimRange;
        #endregion


        public new void Start()
        {
            base.Start();
            lastAttack = TeneborokAttack.None;
        }

        public override void Agro()
        {
            base.Agro();
            switch (lastAttack)
            {
                case TeneborokAttack.None:
                {
                    if (Vector3.Distance(_target.transform.position, transform.position) <= hitCutRange && _canAttack)
                    {
                        HitCut();
                    }
                    else
                    {
                        MoveTo(_target.transform.position, hitCutRange);
                    }
                    break;
                }
                case TeneborokAttack.CoupTranchant:
                    {
                        if(Vector3.Distance(_target.transform.position, transform.position) <= vortexRange && _canAttack && _canVortex 
                            && Vector3.Distance(_target.transform.position, transform.position) >= rimRange)
                        {
                            Vortex();
                        }
                        else if(Vector3.Distance(_target.transform.position, transform.position) <= rimRange && _canAttack && _canRim)
                        {
                            RimVoid();
                        }
                        else
                        {
                            MoveTo(_target.transform.position, vortexRange);
                        }
                        break;
                    }
                case TeneborokAttack.Stop:
                    {
                        _agent.ResetPath();
                        LookTarget();
                        break;
                    }
                default: MoveTo(_target.transform.position, 10); break;
            }
        }

        public void HitCut()
        {
            _canAttack = false;
            lastAttack = TeneborokAttack.CoupTranchant;
            Attack(hitCutDamage, hitCutZoneCenter, hitCutZoneSize);
        }

        public void Vortex()
        {
            _canAttack = false;
            _canVortex = false;
            lastAttack = TeneborokAttack.Stop;
            GameObject t_zone = Instantiate(gameObjectVortex, _target.transform.position, Quaternion.identity);
            GravitationalZone t_gz = t_zone.GetComponent<GravitationalZone>();
            t_gz.SetUp(this);
            StartCoroutine(CooldownRoutine(vortexCD, () => _canVortex = true));
            AttackCooldown();
        }
        
        public void RimVoid()
        {
            _canAttack = false;
            _canRim = false;
            lastAttack = TeneborokAttack.Stop;
            GameObject t_rim = Instantiate(gameObjectRim, transform.position, Quaternion.identity);
            RimVoid t_rv = t_rim.GetComponent<RimVoid>();
            t_rv.SetUp(this);
            StartCoroutine(CooldownRoutine(rimCD, () => _canRim = true));
            AttackCooldown();
        }

        public void SetLastAttack(TeneborokAttack attack)
        {
            lastAttack = attack;
        }

        private new void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (debugHitCut)
            {
                Gizmos.color = colorForHitCutRange;
                Gizmos.DrawWireSphere(transform.position, hitCutRange);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + transform.rotation * hitCutZoneCenter, hitCutZoneSize);
            }
            if (debugVortex)
            {
                Gizmos.color = colorForVortexRange;
                Gizmos.DrawWireSphere(transform.position, vortexRange);
            }
            if (debugRim)
            {
                Gizmos.color = colorForRimRange;
                Gizmos.DrawWireSphere(transform.position, rimRange);
            }
        }
    }

    public enum TeneborokAttack
    {
        CoupTranchant, VortexAbyssal, Move, Stop, None
    }
}

