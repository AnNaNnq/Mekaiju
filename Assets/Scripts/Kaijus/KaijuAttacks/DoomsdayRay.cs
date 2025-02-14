using Mekaiju.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mekaiju.AI.Attack
{
    public class DoomsdayRay : MonoBehaviour
    {
        private TeneborokAI _ai;
        private LineRenderer _line;
        private Transform _start;
        private bool _damagable = true;


        public void SetUp(Transform p_start, TeneborokAI p_ai)
        {
            _ai = p_ai;
            _line = GetComponent<LineRenderer>();
            _start = p_start;
            SetRay();
        }

        public void Update()
        {
            if (_ai == null) return;
            Vector3 pos = new Vector3(_ai.GetTargetPos().x, UtilsFunctions.GetGround(_ai.GetTargetPos()), _ai.GetTargetPos().z);
            transform.position = Vector3.MoveTowards(transform.position, _ai.GetTargetPos(), _ai.doomsdayRaySpeed * Time.deltaTime);
            SetRay();
        }

        public void SetRay()
        {
            Vector3 direction = (transform.position - _start.position).normalized;
            float distance = Vector3.Distance(transform.position, _start.position);
            _line.positionCount = 2;
            _line.SetPosition(0, _start.position);
            _line.SetPosition(1, transform.position);
            if(Physics.Raycast(_start.position, direction, out RaycastHit hit, distance, LayerMask.GetMask("Player"))){
                if (_damagable)
                {
                    _damagable = false;
                    _ai.AddDps(_ai.doomsdayRayDamage);
                    MechaInstance _mecha = hit.collider.GetComponent<MechaInstance>();
                    _mecha.TakeDamage(_ai.doomsdayRayDamage);
                    StartCoroutine(DamagableCooldown());
                }
            }
        }

        IEnumerator DamagableCooldown()
        {
            yield return new WaitForSeconds(0.5f);
            _damagable = true;
        }
    }
}
