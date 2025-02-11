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
        private int lastAttack = 0;

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
        [OverrideLabel("Damage")] public int vortextDamage = 10;
        [OverrideLabel("Range")] public float vortexRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int vortexBody;
        [OverrideLabel("Attack zone center")] public Vector3 vortexZoneCenter;
        [OverrideLabel("Attack zone size")] public Vector3 vortexZoneSize;
        [OverrideLabel("Gravitational Zone Prefab")] public GameObject gameObjectVortex;
        [OverrideLabel("CD")] public float vortexCD = 10f;
        [SerializeField]
        private bool _canVortex = true;
        #endregion

        #region Pour les ld
        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Hit Cut")]
        public bool debugHitCut = false;
        [ConditionalField(nameof(debugHitCut))] public Color colorForHitCutRange;
        [OverrideLabel("Show Gizmo For Vortex")]
        public bool debugVortex = false;
        [ConditionalField(nameof(debugVortex))] public Color colorForVortexRange;
        #endregion


        public new void Start()
        {
            base.Start();
            lastAttack = -1;
        }

        public override void Agro()
        {
            base.Agro();
            switch (lastAttack)
            {
                case -1:
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
                case 1:
                    {
                        if(Vector3.Distance(_target.transform.position, transform.position) <= vortexRange && _canAttack && _canVortex)
                        {
                            Vortex();
                        }
                        else
                        {
                            MoveTo(_target.transform.position, vortexRange);
                        }
                        break;
                    }
                default: MoveTo(_target.transform.position, 8); break;
            }
        }

        public void HitCut()
        {
            _canAttack = false;
            lastAttack = 1;
            Attack(hitCutDamage, hitCutZoneCenter, hitCutZoneSize);
        }


        public void Vortex()
        {
            _canAttack = false;
            _canVortex = false;
            lastAttack = 2;
            GameObject t_zone = Instantiate(gameObjectVortex, _target.transform.position, Quaternion.identity);
            StartCoroutine(VortexCD());
            StartCoroutine(AttackCountdown());
        }
        

        IEnumerator VortexCD()
        {
            yield return new WaitForSeconds(vortexCD);
            _canVortex = true;
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
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + transform.rotation * vortexZoneCenter, vortexZoneSize);
            }
        }
    }
}

