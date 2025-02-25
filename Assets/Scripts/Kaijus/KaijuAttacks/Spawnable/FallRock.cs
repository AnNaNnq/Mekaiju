using Mekaiju;
using Mekaiju.AI;
using UnityEngine;

public class FallRock : MonoBehaviour
{
    public float _dmg;
    AbyssalVortex _stat;
    public GameObject impactVFX;


    public void SetUp(AbyssalVortex p_stat)
    {
        _dmg = p_stat.damage;
        _stat = p_stat;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<MechaInstance>().TakeDamage(_dmg);
        }
        else
        {
            GameObject t_impactVFX = Instantiate(impactVFX, transform.position, Quaternion.identity);
            Destroy(t_impactVFX, 2);
            Destroy(gameObject);
        }
    }
}
