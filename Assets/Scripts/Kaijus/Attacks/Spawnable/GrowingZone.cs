using Mekaiju.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class GrowingZone : MonoBehaviour
    {
        public Transform startPoint; // Premier objet (d�but du collider)
        public Transform endPoint;   // Deuxi�me objet (fin du collider)
        public Transform useEndPoint;
        public BoxCollider boxCollider;

        [HideInInspector]
        public RollDash stat;
        [HideInInspector]
        public KaijuInstance kaiju;

        bool _playerInside = false;

        IDisposable _effect;

        void Update()
        {
            if (startPoint == null || useEndPoint == null || boxCollider == null)
                return;

            // Direction entre les deux points
            Vector3 t_direction = useEndPoint.position - startPoint.position;
            float t_distance = t_direction.magnitude;

            // D�finir la position centrale entre les deux points
            boxCollider.transform.position = startPoint.position;

            // Faire pivoter l'objet pour qu'il s'aligne avec la direction
            if (t_direction != Vector3.zero)
            {
                boxCollider.transform.rotation = Quaternion.LookRotation(t_direction);
            }

            // Ajuster la taille du BoxCollider
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, t_distance);

            // D�caler le BoxCollider pour qu'il s'�tire bien dans la direction
            boxCollider.center = new Vector3(0, 0, t_distance / 2f);
        }

        private void OnEnable()
        {
            boxCollider.GetComponent<ChildCollider>().OnChildTriggeredEnter += OnChildTriggerEnter;
            boxCollider.GetComponent<ChildCollider>().OnChildTriggeredExit += OnChildTriggerExit;
        }

        private void OnDisable()
        {
            boxCollider.GetComponent<ChildCollider>().OnChildTriggeredEnter -= OnChildTriggerEnter;
            boxCollider.GetComponent<ChildCollider>().OnChildTriggeredExit -= OnChildTriggerExit;
        }

        private void OnChildTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                _playerInside = true;
                StartCoroutine(Damage(t_inst));
                _effect = t_inst.AddEffect(stat.effect);
            }
        }

        IEnumerator Damage(MechaPartInstance p_inst)
        {
            while (_playerInside)
            {
                float t_damage = kaiju.GetRealDamage(stat.zoneDamage);
                kaiju.AddDPS(t_damage);
                p_inst.TakeDamage(kaiju, t_damage, Entity.DamageKind.Direct);

                yield return new WaitForSeconds(stat.damageTick);
            }
        }

        private void OnChildTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                _playerInside = false;
                t_inst.RemoveEffect(_effect);
            }
        }
    }
}
