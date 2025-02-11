using Mekaiju;
using Mekaiju.AI;
using UnityEngine;

public class FallRock : MonoBehaviour
{
    public int _dmg;
    TeneborokAI _ia;

    public void SetUp(int p_dmg, TeneborokAI p_ai)
    {
        _dmg = p_dmg;
        _ia = p_ai;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<MechaInstance>().TakeDamage(_dmg);
            _ia.AddDps(_dmg);
        }
    }
}
