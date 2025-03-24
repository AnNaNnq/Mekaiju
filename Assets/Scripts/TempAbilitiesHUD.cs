using Mekaiju;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempAbilitiesHUD : MonoBehaviour
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
            if (t_part.ability != null && t_part.ability.showInHUD)
            {
                GameObject t_obj = Instantiate(CapacityPrefab, CapacityContainerObj.transform);
                Image t_img = t_obj.GetComponent<Image>();
                CapacityImage t_capImg = t_obj.GetComponent<CapacityImage>();

                t_img.sprite = t_part.ability.icon;
                t_obj.name = t_part.ability.name + "_img";
                t_capImg.Init(t_part.ability.behaviour);
            }
        });
    }
}
