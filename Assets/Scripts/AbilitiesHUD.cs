using System.Collections;
using Mekaiju;
using UnityEngine;
using UnityEngine.UI;
using Mekaiju.Utils;

public class AbilitiesHUD : MonoBehaviour
{
    private MechaInstance _inst;

    public GameObject CapacityContainerObj;
    public GameObject CapacityPrefab;

    private void Awake()
    {
        _inst = GameObject.Find("Player").GetComponent<MechaInstance>();
    }

    private void Start()
    {
        StartCoroutine(LateStart(.5f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        UpdateCapacity();
    }

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
                GameObject t_obj = Instantiate(CapacityPrefab, CapacityContainerObj.transform);
                Image t_img = t_obj.GetComponent<Image>();
                CapacityImage t_capImg = t_obj.GetComponent<CapacityImage>();

                t_img.sprite = t_desc.ability.icon;
                t_obj.name = t_desc.ability.name + "_img";
                t_capImg.Init(t_desc.ability.behaviour);
            }
        });
    }
}
