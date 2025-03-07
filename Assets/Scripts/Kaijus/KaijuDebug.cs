using Mekaiju.AI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class KaijuDebug : MonoBehaviour
{
    [Header("Pour l'affichage des UI pas modifiable")]
    private KaijuInstance _kaiju;
    public GameObject kaijuHealthPrefab;
    public GameObject kaijuHealtParent;
    public TextMeshProUGUI txtDPS;

    List<GameObject> _kaijuHealthInstances;

    private void Start()
    {
        _kaiju = FindFirstObjectByType<KaijuInstance>();
        _kaijuHealthInstances = new List<GameObject>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach(var health in _kaijuHealthInstances)
        {
            Destroy(health);
        }

        foreach(var part in _kaiju.bodyParts)
        {
            var health = Instantiate(kaijuHealthPrefab, kaijuHealtParent.transform);
            health.GetComponent<TextMeshProUGUI>().text = $"{part.nom} : {part.maxHealth}";
            _kaijuHealthInstances.Add(health);
        }

        txtDPS.text = _kaiju.dps.ToString();
    }
}
