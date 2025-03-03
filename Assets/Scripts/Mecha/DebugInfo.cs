using System.Collections;
using System.Collections.Generic;
using Mekaiju;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mekaiju.Entity.Effect;
using System.Linq;

public class DebugInfo : MonoBehaviour
{
    private MechaInstance _inst;

    [SerializeField]
    private TextMeshProUGUI _staminaField;
    [SerializeField]
    private TextMeshProUGUI _healthField;

    [SerializeField]
    private TextMeshProUGUI _lArmHealthField;
    [SerializeField]
    private TextMeshProUGUI _rArmHealthField;
    [SerializeField]
    private TextMeshProUGUI _headHealthField;
    [SerializeField]
    private TextMeshProUGUI _chestHealthField;
    [SerializeField]
    private TextMeshProUGUI _legsHealthField;

    [SerializeField]
    private TextMeshProUGUI _dps;

    [SerializeField]
    private TextMeshProUGUI _effectField;

    private float _maxHealth;
    public Image healthBarUI;

    public Image effectTimeRing;
    public Transform effectTimeList;
    private Dictionary<StatefullEffect, Image> _effectsMapping;

    private void Awake()
    {
        _inst = GameObject.Find("Player").GetComponent<MechaInstance>();
        _effectsMapping = new();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _SetStamina();
        _inst.onAddEffect.AddListener(_SetEffects);
        _inst.onRemoveEffect.AddListener(_RemoveEffects);
        _inst.onDealDamage.AddListener(_OnDealDamage);
        StartCoroutine(LateStart(1));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _maxHealth = _inst.health;
    }

    // Update is called once per frame
    void Update()
    {
        _SetStamina();
        _SetHealth();
        _SetHealthBar();

        foreach (var (key, value) in _effectsMapping)
        {
            value.fillAmount = key.remainingTime / key.time;
        }
    }

    private IEnumerator _SetTempValue(TextMeshProUGUI p_target, string p_text, float timout)
    {
        p_target.text = p_text;
        yield return new WaitForSeconds(timout);
        p_target.text = "";
    }

    public void SetTempValue(TextMeshProUGUI p_target, string p_text, float timout)
    {
        StartCoroutine(_SetTempValue(p_target, p_text, timout));
    }

    private void _SetStamina()
    {
        _staminaField.text = $"{_inst.stamina:0.00}";
    }

    private void _SetEffects(StatefullEffect p_effect)
    {
        var t_go  = Instantiate(effectTimeRing, effectTimeList);
        _effectsMapping.Add(p_effect, t_go);
    }

    private void _RemoveEffects(StatefullEffect p_effect)
    {
        Destroy(_effectsMapping[p_effect].gameObject);
        _effectsMapping.Remove(p_effect);
    }

    private void _OnDealDamage(float p_damage)
    {
        StartCoroutine(_SetTempValue(_dps, $"{p_damage:0.00}", 0.5f));
    }

    private void _SetHealthBar()
    {
        healthBarUI.fillAmount = _inst.health/_maxHealth;
    }

    private void _SetHealth()
    {
        _healthField.text = $"{_inst.health:0.00}";
        _lArmHealthField.text = $"{_inst[MechaPart.LeftArm].health:0.00}";
        _rArmHealthField.text = $"{_inst[MechaPart.RightArm].health:0.00}";
        _headHealthField.text = $"{_inst[MechaPart.Head].health:0.00}";
        _chestHealthField.text = $"{_inst[MechaPart.Chest].health:0.00}";
        _legsHealthField.text = $"{_inst[MechaPart.Legs].health:0.00}";
    }
}