using Mekaiju;
using Mekaiju.AI;
using Mekaiju.AI.Objet;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(KaijuAnimatorController))]
public class LittleKaijuInstance : IEntityInstance
{
    [SerializeField]
    private float _health;
    public KaijuStats stats;
    [Tag] public string targetTag;
    public float timeBetweenTowAttack = 1f;

    [field: SerializeField]
    public List<StatefullEffect> effects { get; private set; }

    public List<KaijuAttack> attacks;
    public List<KaijuPassive> passives;

    Transform _target;
    NavMeshAgent _agent;
    public KaijuAnimatorController animator { get; private set; }

    bool canAttack;

    private void Start()
    {
        _health = stats.health;
        effects = new();

        foreach(KaijuPassive passive in passives)
        {
            passive.passive.OnStart();
        }
        
        _target = GameObject.FindGameObjectWithTag(targetTag).transform;
        _agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<KaijuAnimatorController>();

        canAttack = true;
    }

    private void Update()
    {
        effects.ForEach(effect => effect.Tick());
        effects.RemoveAll(effect =>
        {
            if (effect.state == EffectState.Expired)
            {
                effect.Dispose();
                return true;
            }
            return false;
        });

        if (canAttack)
        {
            bool asAttack = false;
            foreach (KaijuAttack attack in attacks)
            {
                if (attack.attack.CanUse(Vector3.Distance(transform.position, _target.position)))
                {
                    _agent.ResetPath();
                    attack.attack.Active(this);
                    asAttack = true;
                }
            }

            if (!asAttack)
            {
                MoveTo(_target.position, 100);
            }
        }
    }

    private void FixedUpdate()
    {
        effects.ForEach(effect => effect.FixedTick());
    }

    /// <summary>
    /// Moves the Kaiju to a given position
    /// </summary>
    /// <param name="p_pos"></param>
    /// <param name="p_stopping"></param>
    public void MoveTo(Vector3 p_pos, float p_speed, float p_stopping = 10f)
    {
        if (_agent.enabled == false) return;
        p_stopping = Mathf.Max(p_stopping, 10f);

        float t_speed = p_speed * (stats.speed / 100);

        _agent.speed = t_speed;
        _agent.destination = p_pos;
        _agent.stoppingDistance = p_stopping;
    }

    #region getter & setter
    /// <summary>
    /// Adds a new effect to the list of active effects without a timeout. 
    /// The effect will remain active indefinitely until it is manually removed.
    /// </summary>
    /// <param name="p_effect">The effect to be added.</param>
    public IDisposable AddEffect(Effect p_effect)
    {
        effects.Add(new(this, p_effect));
        return effects[^1];
    }

    /// <summary>
    /// Adds a new effect to the list of active effects, with a specified duration.
    /// </summary>
    /// <param name="p_effect">The effect to be added.</param>
    /// <param name="p_time">The duration of the effect in seconds.</param>
    public IDisposable AddEffect(Effect p_effect, float p_time)
    {
        effects.Add(new(this, p_effect, p_time));
        return effects[^1];
    }

    /// <summary>
    /// Remove an effect.
    /// </summary>
    /// <param name="p_effect">The effect to remove.</param>
    public void RemoveEffect(IDisposable p_effect)
    {
        if (typeof(StatefullEffect).IsAssignableFrom(p_effect.GetType()))
        {
            effects.Remove((StatefullEffect)p_effect);
            p_effect.Dispose();
        }
    }

    public float GetRealDamage(float p_amonunt)
    {
        var t_damage = modifiers[ModifierTarget.Damage].ComputeValue(p_amonunt);
        return stats.dmg * (t_damage / 100);
    }
    #endregion

    #region implemation of IEntityInstance
    public override bool isAlive => _health <= 0;

    public override float baseHealth => stats.health;

    public override void Heal(float p_amount)
    {
        _health += p_amount;
    }

    public override void TakeDamage(float p_damage)
    {
        _health -= p_damage;
        foreach(KaijuPassive passive in passives)
        {
            passive.passive.OnDamage();
        }
        if(_health <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion

}
