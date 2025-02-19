using System.Collections;
using Mekaiju;
using TMPro;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
        _SetStamina();
        _SetHealth();
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
        _staminaField.text = $"{_inst.Stamina:0.00}";
    }

    private void _SetHealth()
    {
        _healthField.text = $"{_inst.Health:0.00}";
        _lArmHealthField.text = $"{_inst[MechaPart.LeftArm].Health:0.00}";
        _rArmHealthField.text = $"{_inst[MechaPart.RightArm].Health:0.00}";
        _headHealthField.text = $"{_inst[MechaPart.Head].Health:0.00}";
        _chestHealthField.text = $"{_inst[MechaPart.Chest].Health:0.00}";
        _legsHealthField.text = $"{_inst[MechaPart.Legs].Health:0.00}";
    }
}