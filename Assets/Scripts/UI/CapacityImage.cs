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
        if(_ability.state == AbilityState.InCooldown) cooldownImage.fillAmount = _ability.cooldown;
    }
}
