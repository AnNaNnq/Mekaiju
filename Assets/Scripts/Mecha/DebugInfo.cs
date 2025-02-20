using System.Collections;
using Mekaiju;
using TMPro;
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
    public Image effectTimeLeft;

    public TextMeshProUGUI Sword;
    public TextMeshProUGUI Gun;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager exists.
        if (Instance == null)
        {
            Instance = this;
            _inst = GameObject.Find("Player").GetComponent<MechaInstance>();

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
        
    }

    // Update is called once per frame
    void Update()
    {
        _SetStamina();
        _SetHealth();
        _SetEffect();
        _SetHealthBar();

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

    private void _SetEffect()
    {
        _effectField.text = $"{_inst.effects.ToString(new[] {"Heal", "Stamina"} )}";
        if (_inst.effects[^1].time > 0)
        {
            effectTimeLeft.fillAmount = _inst.effects[^1].remainingTime  / _inst.effects[^1].time;
        }

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