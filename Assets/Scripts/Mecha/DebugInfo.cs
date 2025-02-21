using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mekaiju;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{
    public static DebugInfo Instance { get; private set; }

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
    private TextMeshProUGUI _effectField;

    private float _maxHealth;
    public Image healthBarUI;

    public Image effectTimeRing;
    public Transform effectTimeList;
    private Dictionary<StatefullEffect, Image> _effectsMapping;

    public TextMeshProUGUI Sword;
    public TextMeshProUGUI Gun;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager exists.
        if (Instance == null)
        {
            Instance = this;
            _inst = GameObject.Find("Player").GetComponent<MechaInstance>();
            _effectsMapping = new();

            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _SetStamina();
        _maxHealth = _inst.health;
        _inst.onAddEffect.AddListener(_SetEffects);
        _inst.onRemoveEffect.AddListener(_RemoveEffects);
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