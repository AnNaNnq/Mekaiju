using Mekaiju.Utils;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class DoomsdayRaySpawnable : MonoBehaviour
    {
        private DoomsdayRay _stat;
        private LineRenderer _line;
        private KaijuInstance _instnace;
        private Transform _start;
        private bool _damagable = true;

        public void SetUp(Transform p_start, DoomsdayRay p_stat, KaijuInstance p_instnace)
        {
            _stat = p_stat;
            _instnace = p_instnace;
            _line = GetComponent<LineRenderer>();
            _start = p_start;
            SetRay();
        }

        public void Update()
        {
            if (_instnace == null) return;
            Vector3 pos = new Vector3(_instnace.GetTargetPos().x, UtilsFunctions.GetGround(_instnace.GetTargetPos()), _instnace.GetTargetPos().z);
            transform.position = Vector3.MoveTowards(transform.position, _instnace.GetTargetPos(), _stat.speed * Time.deltaTime);
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
                    if (hit.collider.TryGetComponent(out MechaInstance _mecha))
                    {
                        float p_damage = _instnace.GetRealDamage(_stat.damage);
                        _mecha.TakeDamage(p_damage);
                        _instnace.AddDPS(p_damage);
                        StartCoroutine(DamagableCooldown());
                    }
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
