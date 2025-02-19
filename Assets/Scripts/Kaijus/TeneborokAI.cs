using Mekaiju.Attacks;
using Mekaiju.Attribute;
using Mekaiju.Utils;
using Mekaiju.AI.Attack;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class TeneborokAI : BasicAI
    {
        [SerializeField]
        private TeneborokAttack lastAttack;

        #region Coup tranchant
        [Foldout("Coup tranchant")]
        [OverrideLabel("Damage")] public int sharpBlowDamage = 10;
        [OverrideLabel("Range")] public float sharpBlowRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int sharpBlowBody;
        [OverrideLabel("Attack zone center")] public Vector3 sharpBlowZoneCenter;
        [OverrideLabel("Attack zone size")] public Vector3 sharpBlowZoneSize;
        #endregion

        #region Vortex Abyssal
        [Foldout("Vortex Abyssal")]
        [OverrideLabel("Damage")] public int abyssalVortexDamage = 10;
        [OverrideLabel("Range")] public float abyssalVortexRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int abyssalVortexBody;
        [OverrideLabel("Gravitational zone prefab")][OpenPrefabButton] public GameObject gameObjectAbyssalVortex;
        [OverrideLabel("Kaillou prefab")][OpenPrefabButton] public GameObject gameObjectRock;
        [OverrideLabel("Vortex Radius")] public float abyssalVortexRadius = 10f;
        [OverrideLabel("Number of rock")] public int abyssalVortexNumberOfRock = 10;
        [OverrideLabel("CD")] public float abyssalVortexCooldown = 10f;

        private bool _canAbyssalVortex = true;
        #endregion

        #region Fissure du N�ant
        [Foldout("Fissure du N�ant")]
        [OverrideLabel("Damage")] public int rimVoidDamage = 10;
        [OverrideLabel("Range")] public float rimVoideRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int rimVoidBody;
        [OverrideLabel("Rim prefab")][OpenPrefabButton] public GameObject gameObjectRimVoid;
        [OverrideLabel("Fire prefab")][OpenPrefabButton] public GameObject gameObjectRimVoidFire;
        [OverrideLabel("Duration (sec)")] public int rimVoidDuration = 2;
        [OverrideLabel("Hit cooldown (sec)")] public float rimVoidHitCooldown = 0.1f;
        [OverrideLabel("Modifier")] [SOSelector] public Effect rimVoidEffect;
        [OverrideLabel("CD")] public float rimVoidCooldown = 10f;

        private bool _canRimVoid = true;
        #endregion

        #region Frappe du serpent
        [Foldout("Frappe du serpent")]
        [OverrideLabel("Damage")] public int snakeStrikeDamage = 10;
        [OverrideLabel("Range")] public float snakeStrikeRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int snakeStrikeBody;
        [OverrideLabel("Attack zone center")] public Vector3 snakeStrikeZoneCenter;
        [OverrideLabel("Attack zone size")] public Vector3 snakeStrikeZoneSize;
        #endregion

        #region Rayon Apocalypse
        [Foldout("Rayon Apocalypse")]
        [OverrideLabel("Damage")] public int doomsdayRayDamage = 10;
        [OverrideLabel("Range")] public float doomsdayRayRange = 2f;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int doomsdayRayBody;
        [OverrideLabel("CD")] public float doomsdayRayCooldown = 10f;
        [OverrideLabel("Prefab")][OpenPrefabButton] public GameObject gameObjectDoomsdayRay;
        [OverrideLabel("Start")][FocusObject] public Transform doomsdayRayStart;
        [OverrideLabel("Ray Speed")] public float doomsdayRaySpeed = 10f;
        [OverrideLabel("Duration (sec)")] public float doomsdayRayDuration = 5f;

        private bool _canDoomsdayRay = true;
        #endregion

        #region Pour les ld
        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Hit Cut")]
        public bool debugSharpBlow = false;
        [ConditionalField(nameof(debugSharpBlow))] public Color colorForSharpBlowRange;
        [OverrideLabel("Show Gizmo For Abyssal Vortex")]
        public bool debugAbyssalVortex = false;
        [ConditionalField(nameof(debugAbyssalVortex))] public Color colorForAbyssalVortexRange;
        [OverrideLabel("Show Gizmo For Rim Void")]
        public bool debugRimVoid = false;
        [ConditionalField(nameof(debugRimVoid))] public Color colorForRimVoidRange;
        [OverrideLabel("Show Gizmo For Snake Strike")]
        public bool debugSnakeStrike = false;
        [ConditionalField(nameof(debugSnakeStrike))] public Color colorForSnakeStrikeRange;
        [OverrideLabel("Show Gizmo For Doomsday Ray")]
        public bool debugDoomsdayRay = false;
        [ConditionalField(nameof(debugDoomsdayRay))] public Color colorForDoomsdayRayeRange;
        #endregion

        #region Time For Animation
        [Foldout("Time For Animation")]
        public float timeForSharpBlow = 0.2f;
        public float timeForSnakeStrike = 0.1f;
        #endregion

        public new void Start()
        {
            base.Start();
            lastAttack = TeneborokAttack.None;
        }

        private new void Update()
        {
            base.Update();

        }

        public override void Agro()
        {
            base.Agro();
            if(_currentPhase == 1)
            {
                FirstPhase();
            }
            
        }

        public void FirstPhase()
        {
            switch (lastAttack)
            {
                case TeneborokAttack.None:
                    {
                        if (GetTargetDistance() <= sharpBlowRange && _canAttack)
                        {
                            _animator.SetTrigger("CoupTranchant");
                            StartCoroutine(SharpBlow());
                        }
                        else
                        {
                            MoveTo(_target.transform.position, sharpBlowRange);
                        }
                        break;
                    }
                case TeneborokAttack.SharpBlow:
                    {
                        if (GetTargetDistance() <= abyssalVortexRange && _canAttack && _canAbyssalVortex
                            && GetTargetDistance() >= rimVoideRange)
                        {
                            AbyssalVortex();
                        }
                        else if (GetTargetDistance() <= rimVoideRange && _canAttack && _canRimVoid
                            && GetTargetDistance() >= snakeStrikeRange)
                        {
                            RimVoid();
                        }
                        else if (GetTargetDistance() <= snakeStrikeRange && _canAttack)
                        {
                            _animator.SetTrigger("FrappeSerpent");
                            StartCoroutine(SnakeStrik());
                        }
                        else
                        {
                            MoveTo(_target.transform.position, abyssalVortexRange);
                        }
                        break;
                    }
                case TeneborokAttack.SnakeStrike:
                    {
                        if(GetTargetDistance() <= doomsdayRayRange && _canAttack && _canDoomsdayRay)
                        {
                            StartCoroutine(DoomsdayRay());
                        }
                        else
                        {
                            MoveTo(_target.transform.position, doomsdayRayRange);
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

        IEnumerator SnakeStrik()
        {
            lastAttack = TeneborokAttack.Stop;
            _canAttack = false;
            yield return new WaitForSeconds(timeForSnakeStrike);
            lastAttack = TeneborokAttack.SnakeStrike;
            Attack(snakeStrikeDamage, snakeStrikeZoneCenter, snakeStrikeZoneSize);
        }

        IEnumerator SharpBlow()
        {
            lastAttack = TeneborokAttack.Stop;
            _canAttack = false;
            yield return new WaitForSeconds(timeForSharpBlow);
            lastAttack = TeneborokAttack.SharpBlow;
            Attack(sharpBlowDamage, sharpBlowZoneCenter, sharpBlowZoneSize);
        }

        public void AbyssalVortex()
        {
            _canAttack = false;
            _canAbyssalVortex = false;
            lastAttack = TeneborokAttack.Stop;
            GameObject t_zone = Instantiate(gameObjectAbyssalVortex, _target.transform.position, Quaternion.identity);
            GravitationalZone t_gz = t_zone.GetComponent<GravitationalZone>();
            t_gz.SetUp(this);
            StartCoroutine(CooldownRoutine(abyssalVortexCooldown, () => _canAbyssalVortex = true));
        }

        public void RimVoid()
        {
            _canAttack = false;
            _canRimVoid = false;
            lastAttack = TeneborokAttack.Stop;
            GameObject t_rim = Instantiate(gameObjectRimVoid, transform.position, Quaternion.identity);
            RimVoid t_rv = t_rim.GetComponent<RimVoid>();
            t_rv.SetUp(this);
            StartCoroutine(CooldownRoutine(rimVoidCooldown, () => _canRimVoid = true));
        }

        IEnumerator DoomsdayRay()
        {
            _canDoomsdayRay = false;
            _canAttack = false;
            lastAttack = TeneborokAttack.Stop;
            Vector3 t_pos = new Vector3(transform.position.x, UtilsFunctions.GetGround(transform.position), transform.position.z) + (transform.forward * 10);
            GameObject t_doomsday = Instantiate(gameObjectDoomsdayRay, t_pos, Quaternion.identity);
            DoomsdayRay t_dr = t_doomsday.GetComponent<DoomsdayRay>();
            t_dr.SetUp(doomsdayRayStart, this);
            yield return new WaitForSeconds(doomsdayRayDuration);
            lastAttack = TeneborokAttack.DoomsdayRay;
            Destroy(t_doomsday);
            StartCoroutine(CooldownRoutine(doomsdayRayCooldown, () => _canDoomsdayRay = true));
        }

        public void SetLastAttack(TeneborokAttack attack)
        {
            lastAttack = attack;
        }

        private new void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (debugSharpBlow)
            {
                Gizmos.color = colorForSharpBlowRange;
                Gizmos.DrawWireSphere(transform.position, sharpBlowRange);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + transform.rotation * sharpBlowZoneCenter, sharpBlowZoneSize);
            }
            if (debugAbyssalVortex)
            {
                Gizmos.color = colorForAbyssalVortexRange;
                Gizmos.DrawWireSphere(transform.position, abyssalVortexRange);
            }
            if (debugRimVoid)
            {
                Gizmos.color = colorForRimVoidRange;
                Gizmos.DrawWireSphere(transform.position, rimVoideRange);
            }
            if (debugSnakeStrike)
            {
                Gizmos.color = colorForSnakeStrikeRange;
                Gizmos.DrawWireSphere(transform.position, snakeStrikeRange);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + transform.rotation * snakeStrikeZoneCenter, snakeStrikeZoneSize);
            }
            if (debugDoomsdayRay)
            {
                Gizmos.color = colorForDoomsdayRayeRange;
                Gizmos.DrawWireSphere(transform.position, doomsdayRayRange);
            }
        }
    }

    public enum TeneborokAttack
    {
        SharpBlow, AbyssalVortex, RimVoid, SnakeStrike, DoomsdayRay, Move, Stop, None
    }
}

