using Mekaiju;
using UnityEngine;
using UnityEngine.UI;

public class CapacityImage : MonoBehaviour
{
    public Image cooldownImage;

    IAbilityBehaviour _ability;

    public void Init(IAbilityBehaviour p_ability)
    {
        cooldownImage.sprite = GetComponent<Image>().sprite;
        _ability = p_ability;
    }

    void Update()
    {
        UpdateImage();
    }

    void UpdateImage()
    {
        switch (_ability.state)
        {
            case AbilityState.InCooldown: cooldownImage.fillAmount = _ability.cooldown; break;
            case AbilityState.Active: cooldownImage.fillAmount = 1; break;
            case AbilityState.Ready: cooldownImage.fillAmount = 0; break;
        }

    }
}
