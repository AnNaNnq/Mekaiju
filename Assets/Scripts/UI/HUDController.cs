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
        _inst.onAddEffect.AddListener(_SetEffects);
        _inst.onRemoveEffect.AddListener(_RemoveEffects);
        
        _effectsMapping = new();
    }

    void Start()
    {
        StartCoroutine(LateStart(1));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _maxHealth = _inst.health;
        UpdateCapacity();
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
    public void UpdateCapacity()
    {
        GetCapacities();
    }

    public void GetCapacities()
    {
        _inst.desc.parts.ForEach((t_part, t_desc) =>
        {
            if (t_desc.ability != null && t_part != MechaPart.Legs)
            {
                GameObject t_obj = Instantiate(capacityPrefab, capacitiesList.transform);
                Image t_img = t_obj.GetComponent<Image>();
                CapacityImage t_capImg = t_obj.GetComponent<CapacityImage>();

                t_img.sprite = t_desc.ability.icon;
                t_obj.name = t_desc.ability.name + "_img";
                t_capImg.Init(t_desc.ability.behaviour);
            }
        });
    }
    #endregion

    #region Mecha Health Bar
    private void _SetHealthBar()
    {
        healthBarUI.fillAmount = _inst.health / _maxHealth;
    }
    #endregion
}
