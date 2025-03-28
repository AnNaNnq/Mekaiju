using Mekaiju;
using Mekaiju.Entity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CapacityImage : MonoBehaviour
{
    public Image cooldownImage;

    IAbilityBehaviour _ability;

    Coroutine _cooldownRotine;

    public void Init(IAbilityBehaviour p_ability)
    {
        cooldownImage.sprite = GetComponent<Image>().sprite;
        _ability = p_ability;
        _ability.state.onChange.AddListener(UpdateImage);
    }

    private void OnDisable()
    {
        _ability?.state.onChange.RemoveListener(UpdateImage);
    }

    void UpdateImage(AbilityState p_prev_state, AbilityState p_current_state)
    {
        if(_cooldownRotine != null) StopCoroutine(_cooldownRotine);

        switch (p_current_state)
        {
            case AbilityState.InCooldown: _cooldownRotine = StartCoroutine(Cooldown()); break;
            case AbilityState.Active: cooldownImage.fillAmount = 1; break;
            case AbilityState.Ready: cooldownImage.fillAmount = 0; break;
        }
    }

    IEnumerator Cooldown()
    {
        while (true)
        {
            cooldownImage.fillAmount = _ability.cooldown;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
