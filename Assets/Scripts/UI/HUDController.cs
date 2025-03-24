using System.Collections;
using System.Collections.Generic;
using Mekaiju;
using UnityEngine;
using UnityEngine.UI;
using Mekaiju.Entity.Effect;
using Mekaiju.Utils;

public class HUDController : MonoBehaviour
{
    // Mecha Variables
    private MechaInstance _inst;

    // Mecha Health Variables
    private float _maxHealth;
    public Image healthBarUI;

    // Effects Variables
    public GameObject effectPrefab;
    public Transform effectsList;
    private Dictionary<StatefullEffect, GameObject> _effectsMapping;

    // Capacities Variables
    public GameObject capacityPrefab;
    public Transform capacitiesList;

    private void Awake()
    {
        _inst = GameObject.Find("Player").GetComponent<MechaInstance>();
        _effectsMapping = new();
    }

    void Start()
    {
        _inst.onAddEffect.AddListener(_SetEffects);
        _inst.onRemoveEffect.AddListener(_RemoveEffects);
        StartCoroutine(LateStart(1));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _maxHealth = _inst.health;
        _Capacities();
    }

    // Update is called once per frame
    void Update()
    {
        _SetHealthBar();

        foreach (var (key, value) in _effectsMapping)
        {
            var images = value.GetComponentsInChildren<Image>();
            images[1].fillAmount = key.remainingTime / key.time;
        }
    }

    #region Effects
    private void _SetEffects(StatefullEffect p_effect)
    {
        GameObject effectInstance = Instantiate(effectPrefab, effectsList);
        var images = effectInstance.GetComponentsInChildren<Image>();

        for (int i = 0; i < images.Length && i < p_effect.effect.effectImages.Length; i++)
        {
            images[i].sprite = p_effect.effect.effectImages[i];
        }

        _effectsMapping.Add(p_effect, effectInstance);
    }

    private void _RemoveEffects(StatefullEffect p_effect)
    {
        if (_effectsMapping.ContainsKey(p_effect))
        {
            Destroy(_effectsMapping[p_effect]);
            _effectsMapping.Remove(p_effect);
        }
    }
    #endregion

    #region Capacities
    private void _Capacities()
    {
        _inst.desc.parts.ForEach((t_part, t_desc) =>
        {
            if (t_part == MechaPart.Legs)
            {
                _CooldownCapacities(((LegsCompoundAbility)t_desc.ability.behaviour)[LegsSelector.Dash]);
                _CooldownCapacities(((LegsCompoundAbility)t_desc.ability.behaviour)[LegsSelector.Jump]);
            }
            else
            {
                _CooldownCapacities(t_desc.ability);
            }
        });
    }

    private void _CooldownCapacities(Ability p_ability)
    {
        float t_cooldown = p_ability.behaviour.cooldown;
        if (!p_ability.name.Equals("EmptyAbility"))
        {
            GameObject instance = Instantiate(capacityPrefab, capacitiesList);
            Image abilityImage = instance.GetComponent<Image>();
            abilityImage.sprite = p_ability.icon;
        }
    }
    #endregion

    #region Mecha Health Bar
    private void _SetHealthBar()
    {
        healthBarUI.fillAmount = _inst.health / _maxHealth;
    }
    #endregion
}
