using Mekaiju;
using Mekaiju.AI;
using UnityEngine;
using UnityEngine.VFX;

public class FallRock : MonoBehaviour
{
    public int _dmg;
    TeneborokAI _ia;
    public GameObject impactVFX;


    public void SetUp(TeneborokAI p_ai)
    {
        _dmg = p_ai.abyssalVortexDamage;
        _ia = p_ai;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<MechaInstance>().TakeDamage(_dmg);
            _ia.AddDps(_dmg);
        }
        else
        {
            GameObject t_impactVFX = Instantiate(impactVFX, transform.position, Quaternion.identity);
            Destroy(t_impactVFX, 2);
            Destroy(gameObject);
        }
    }
}
